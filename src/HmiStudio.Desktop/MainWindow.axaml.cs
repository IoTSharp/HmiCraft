using System.Globalization;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HmiStudio.Core;

namespace HmiStudio.Desktop;

public sealed partial class MainWindow : Window
{
    private HmiProject _project = DemoProject.Create();
    private OfflineSimulator _simulator;
    private readonly List<HmiProject> _undo = [];
    private readonly List<HmiProject> _redo = [];
    private Guid? _selection;
    private string? _filePath;
    private bool _simulating;
    private bool _refreshing;
    private bool _dirty;
    private bool _allowClose;
    private bool _fileOperationInProgress;
    private DragState? _drag;
    private HmiScreen Screen => _project.Screens[0];
    private HmiWidget? Selected => Screen.Widgets.Find(widget => widget.Id == _selection);
    private static readonly FilePickerFileType ProjectFileType = new("HmiStudio 工程") { Patterns = ["*.hmistudio.json"] };

    public MainWindow() : this([]) { }

    public MainWindow(string[] args)
    {
        InitializeComponent();
        _simulator = new OfflineSimulator(_project);
        _selection = Screen.Widgets.FirstOrDefault()?.Id;
        RefreshAll();
        Closing += OnClosing;
        Deactivated += (_, _) => CompleteDrag();
        if (args.Length == 2 && args[0] == "--smoke-test")
        {
            Opened += async (_, _) => await RunSmokeTestAsync(args[1]);
        }
    }

    private void SetStatus(string message) => StatusText.Text = message;

    private void RefreshAll()
    {
        _refreshing = true;
        ProjectName.Text = _project.Name;
        ScreenName.Text = $"画面 / {Screen.Name}";
        CanvasCaption.Text = $"{Screen.Name}   ·   {Screen.Width} × {Screen.Height}";
        ModeButton.Content = _simulating ? "■  停止离线模拟" : "▶  运行离线模拟";
        Toolbox.IsEnabled = !_simulating;
        UndoButton.IsEnabled = !_simulating && _undo.Count > 0;
        RedoButton.IsEnabled = !_simulating && _redo.Count > 0;
        ObjectList.ItemsSource = Screen.Widgets.Select(widget => new ListBoxItem
        {
            Content = new TextBlock { Text = $"{KindName(widget.Kind)} · {widget.Text}", MaxWidth = 148, TextTrimming = TextTrimming.CharacterEllipsis },
            Tag = widget.Id,
            FontSize = 12
        }).ToArray();
        ObjectList.SelectedIndex = Screen.Widgets.FindIndex(widget => widget.Id == _selection);
        RenderCanvas();
        RefreshInspector();
        RenderTags();
        _refreshing = false;
        Title = $"{(_dirty ? "* " : "")}{_project.Name} — HmiStudio 离线工作台";
    }

    private static string KindName(WidgetKind kind) => kind switch
    {
        WidgetKind.Rectangle => "矩形",
        WidgetKind.Ellipse => "椭圆",
        WidgetKind.Text => "文本",
        WidgetKind.Button => "按钮",
        WidgetKind.Indicator => "指示灯",
        _ => "数值"
    };

    private void RenderCanvas()
    {
        DesignCanvas.Width = Screen.Width;
        DesignCanvas.Height = Screen.Height;
        DesignCanvas.Background = Brush.Parse(Screen.Background);
        DesignCanvas.Children.Clear();
        foreach (var widget in Screen.Widgets)
        {
            var selected = widget.Id == _selection && !_simulating;
            var frame = new Border
            {
                Width = widget.Width,
                Height = widget.Height,
                BorderBrush = selected ? Brush.Parse("#16846D") : Brushes.Transparent,
                BorderThickness = new Thickness(selected ? 2 : 0),
                Cursor = new Cursor(_simulating ? StandardCursorType.Arrow : StandardCursorType.SizeAll),
                Tag = widget.Id
            };
            var text = widget.Kind == WidgetKind.NumericDisplay && widget.TagName is { Length: > 0 } tag
                ? _simulator.GetValue(tag).ToString("0.##", CultureInfo.InvariantCulture)
                : widget.Text;
            var label = new TextBlock
            {
                Text = text,
                Foreground = Brush.Parse(widget.Foreground),
                FontSize = widget.Kind == WidgetKind.NumericDisplay ? 30 : 17,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            if (widget.Kind is WidgetKind.Ellipse or WidgetKind.Indicator)
            {
                var active = widget.Kind == WidgetKind.Indicator && widget.TagName is { Length: > 0 } name && _simulator.GetValue(name) != 0;
                var grid = new Grid();
                grid.Children.Add(new Avalonia.Controls.Shapes.Ellipse
                {
                    Fill = Brush.Parse(widget.Kind == WidgetKind.Indicator ? active ? "#21A47B" : "#B8C4CD" : widget.Fill)
                });
                grid.Children.Add(label);
                frame.Child = grid;
            }
            else if (_simulating && widget.Kind == WidgetKind.Button)
            {
                var button = new Button
                {
                    Content = label,
                    Background = Brush.Parse(widget.Fill),
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    IsEnabled = !string.IsNullOrWhiteSpace(widget.TagName)
                };
                button.Click += (_, _) =>
                {
                    if (widget.TagName is not null)
                    {
                        _simulator.Toggle(widget.TagName);
                        RefreshAll();
                        SetStatus($"离线模拟：{widget.TagName} = {_simulator.GetValue(widget.TagName)}");
                    }
                };
                frame.Child = button;
            }
            else
            {
                frame.Child = new Border
                {
                    Background = widget.Kind == WidgetKind.Text ? Brushes.Transparent : Brush.Parse(widget.Fill),
                    CornerRadius = new CornerRadius(widget.Kind == WidgetKind.Button ? 5 : 0),
                    Child = label
                };
            }
            Canvas.SetLeft(frame, widget.X);
            Canvas.SetTop(frame, widget.Y);
            if (!_simulating)
            {
                frame.PointerPressed += OnWidgetPressed;
                frame.PointerMoved += OnWidgetMoved;
                frame.PointerReleased += OnWidgetReleased;
                frame.PointerCaptureLost += (_, _) => CompleteDrag();
            }
            DesignCanvas.Children.Add(frame);
        }
    }

    private void RefreshInspector()
    {
        var widget = Selected;
        PropertyPanel.IsEnabled = widget is not null && !_simulating;
        SelectionCaption.Text = widget is null ? "选择一个画面对象" : $"{KindName(widget.Kind)} · {widget.Id.ToString()[..8]}";
        WidgetTextInput.Text = widget?.Text ?? "";
        XInput.Text = widget?.X.ToString(CultureInfo.InvariantCulture) ?? "";
        YInput.Text = widget?.Y.ToString(CultureInfo.InvariantCulture) ?? "";
        WidthInput.Text = widget?.Width.ToString(CultureInfo.InvariantCulture) ?? "";
        HeightInput.Text = widget?.Height.ToString(CultureInfo.InvariantCulture) ?? "";
        FillInput.Text = widget?.Fill ?? "";
        ForegroundInput.Text = widget?.Foreground ?? "";
        BindingInput.Text = widget?.TagName ?? "";
    }

    private void RenderTags()
    {
        TagPanel.Children.Clear();
        foreach (var tag in _project.Tags.Take(4))
        {
            var panel = new StackPanel { Spacing = 4 };
            panel.Children.Add(new TextBlock { Text = $"{tag.Name} · {tag.Type}", FontSize = 11, Foreground = Brush.Parse("#6A7B8B") });
            if (tag.Type == TagValueType.Boolean)
            {
                var control = new CheckBox { Content = "运行状态", IsChecked = _simulator.GetValue(tag.Name) != 0, IsEnabled = _simulating };
                control.IsCheckedChanged += (_, _) =>
                {
                    if (_refreshing || !_simulating) return;
                    _simulator.SetValue(tag.Name, control.IsChecked == true ? 1 : 0);
                    RenderCanvas();
                };
                panel.Children.Add(control);
            }
            else
            {
                var row = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 5 };
                var input = new TextBox { Text = _simulator.GetValue(tag.Name).ToString("0.##", CultureInfo.InvariantCulture), Width = 80, IsEnabled = _simulating };
                var apply = new Button { Content = "设置", IsEnabled = _simulating, Padding = new Thickness(8, 4) };
                apply.Click += (_, _) =>
                {
                    if (!double.TryParse(input.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
                    { SetStatus("请输入有效的有限数值。"); return; }
                    _simulator.SetValue(tag.Name, value);
                    RenderCanvas();
                };
                row.Children.Add(input); row.Children.Add(apply); panel.Children.Add(row);
            }
            TagPanel.Children.Add(panel);
        }
    }

    private void PushHistory(HmiProject snapshot)
    {
        _undo.Add(snapshot);
        if (_undo.Count > 100) _undo.RemoveAt(0);
        _redo.Clear(); _dirty = true;
    }

    private bool Mutate(Action change)
    {
        if (_simulating || _fileOperationInProgress) return false;
        var before = ProjectStore.Clone(_project);
        change();
        var errors = ProjectValidator.Validate(_project);
        if (errors.Count > 0)
        {
            _project = before;
            SetStatus($"修改未应用：{errors[0]}");
            RefreshAll();
            return false;
        }
        PushHistory(before);
        _simulator = new OfflineSimulator(_project);
        RefreshAll();
        return true;
    }

    private void AddWidget(WidgetKind kind)
    {
        var id = Guid.NewGuid();
        if (Mutate(() =>
        {
            var bindingType = kind == WidgetKind.NumericDisplay ? TagValueType.Number : TagValueType.Boolean;
            Screen.Widgets.Add(new HmiWidget
            {
                Id = id,
                Kind = kind,
                X = Math.Min(40 + Screen.Widgets.Count * 8, Screen.Width - 140),
                Y = 60,
                Width = 130,
                Height = 56,
                Text = KindName(kind),
                Fill = "#DCECE6",
                Foreground = "#173B32",
                TagName = kind is WidgetKind.Button or WidgetKind.Indicator or WidgetKind.NumericDisplay
                    ? _project.Tags.FirstOrDefault(tag => tag.Type == bindingType)?.Name : null
            });
        })) { _selection = id; RefreshAll(); SetStatus($"已添加{KindName(kind)}。"); }
    }

    private void AddWidgetClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string name } && Enum.TryParse<WidgetKind>(name, out var kind)) AddWidget(kind);
    }

    private void ApplyPropertiesClick(object? sender, RoutedEventArgs e)
    {
        var widget = Selected;
        if (widget is null || _simulating) return;
        if (!double.TryParse(XInput.Text, CultureInfo.InvariantCulture, out var x) ||
            !double.TryParse(YInput.Text, CultureInfo.InvariantCulture, out var y) ||
            !double.TryParse(WidthInput.Text, CultureInfo.InvariantCulture, out var width) ||
            !double.TryParse(HeightInput.Text, CultureInfo.InvariantCulture, out var height))
        { SetStatus("位置和尺寸必须是数字。"); return; }
        if (Mutate(() =>
        {
            widget.X = x; widget.Y = y; widget.Width = width; widget.Height = height;
            widget.Text = WidgetTextInput.Text ?? ""; widget.Fill = FillInput.Text ?? ""; widget.Foreground = ForegroundInput.Text ?? "";
            widget.TagName = string.IsNullOrWhiteSpace(BindingInput.Text) ? null : BindingInput.Text.Trim();
        })) SetStatus("属性已更新。");
    }

    private void DeleteWidgetClick(object? sender, RoutedEventArgs e)
    {
        if (Selected is { } widget && Mutate(() => Screen.Widgets.Remove(widget)))
        { _selection = Screen.Widgets.FirstOrDefault()?.Id; RefreshAll(); SetStatus("对象已删除，可撤销。"); }
    }

    private void ObjectSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_refreshing || ObjectList.SelectedItem is not ListBoxItem { Tag: Guid id }) return;
        _selection = id; RenderCanvas(); RefreshInspector();
    }

    private void OnWidgetPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_simulating || sender is not Border { Tag: Guid id } frame || !e.GetCurrentPoint(frame).Properties.IsLeftButtonPressed) return;
        _selection = id;
        var widget = Selected!;
        _drag = new DragState(ProjectStore.Clone(_project), e.GetPosition(DesignCanvas), widget.X, widget.Y, frame, e.Pointer);
        frame.BorderBrush = Brush.Parse("#16846D"); frame.BorderThickness = new Thickness(2);
        e.Pointer.Capture(frame); RefreshInspector(); e.Handled = true;
    }

    private void OnWidgetMoved(object? sender, PointerEventArgs e)
    {
        if (_drag is not { } drag || Selected is not { } widget) return;
        var point = e.GetPosition(DesignCanvas);
        widget.X = Math.Clamp(Math.Round(drag.X + point.X - drag.Start.X), 0, Screen.Width - widget.Width);
        widget.Y = Math.Clamp(Math.Round(drag.Y + point.Y - drag.Start.Y), 0, Screen.Height - widget.Height);
        Canvas.SetLeft(drag.Frame, widget.X); Canvas.SetTop(drag.Frame, widget.Y); RefreshInspector();
    }

    private void OnWidgetReleased(object? sender, PointerReleasedEventArgs e)
    {
        CompleteDrag();
    }

    private void CompleteDrag()
    {
        if (_drag is not { } drag) return;
        _drag = null; drag.Pointer?.Capture(null);
        if (Selected is { } widget && (widget.X != drag.X || widget.Y != drag.Y)) PushHistory(drag.Snapshot);
        RefreshAll();
    }

    private void UndoClick(object? sender, RoutedEventArgs e)
    {
        if (_simulating || _undo.Count == 0) return;
        _redo.Add(ProjectStore.Clone(_project)); _project = _undo[^1]; _undo.RemoveAt(_undo.Count - 1);
        _simulator = new OfflineSimulator(_project); _dirty = true; RefreshAll(); SetStatus("已撤销。");
    }

    private void RedoClick(object? sender, RoutedEventArgs e)
    {
        if (_simulating || _redo.Count == 0) return;
        _undo.Add(ProjectStore.Clone(_project)); _project = _redo[^1]; _redo.RemoveAt(_redo.Count - 1);
        _simulator = new OfflineSimulator(_project); _dirty = true; RefreshAll(); SetStatus("已重做。");
    }

    private void ModeClick(object? sender, RoutedEventArgs e)
    {
        _simulating = !_simulating; _simulator = new OfflineSimulator(_project); RefreshAll();
        SetStatus(_simulating ? "离线模拟中：点击开关或修改变量，观察画面。" : "已停止模拟，恢复工程初始变量。");
    }

    private async void NewProjectClick(object? sender, RoutedEventArgs e)
    {
        await RunFileOperationAsync(async () =>
        {
            if (!await CanReplaceAsync()) return;
            ReplaceProject(DemoProject.Create(), null); SetStatus("已新建离线示例。");
        });
    }

    private void ReplaceProject(HmiProject project, string? path)
    {
        _project = project; _filePath = path; _simulating = false; _simulator = new OfflineSimulator(project);
        _undo.Clear(); _redo.Clear(); _selection = Screen.Widgets.FirstOrDefault()?.Id; _dirty = false; RefreshAll();
    }

    private async void OpenClick(object? sender, RoutedEventArgs e)
    {
        await RunFileOperationAsync(async () =>
        {
            if (!await CanReplaceAsync()) return;
            try
            {
                var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "打开 HmiStudio 工程", AllowMultiple = false, FileTypeFilter = [ProjectFileType] });
                if (files.Count == 0) return;
                var path = files[0].TryGetLocalPath() ?? throw new IOException("仅支持本地文件。");
                var project = await ProjectStore.LoadAsync(path);
                ReplaceProject(project, path); SetStatus($"已打开 {Path.GetFileName(path)}；当前编辑第一个画面，共 {project.Screens.Count} 个画面。");
            }
            catch (Exception exception) { SetStatus($"打开失败：{exception.Message}"); }
        });
    }

    private async void SaveClick(object? sender, RoutedEventArgs e) => await RunFileOperationAsync(async () => { await SaveProjectAsync(false); });
    private async void SaveAsClick(object? sender, RoutedEventArgs e) => await RunFileOperationAsync(async () => { await SaveProjectAsync(true); });

    private async Task RunFileOperationAsync(Func<Task> operation)
    {
        if (_fileOperationInProgress) return;
        CompleteDrag();
        _fileOperationInProgress = true;
        WorkspaceRoot.IsEnabled = false;
        try { await operation(); }
        finally { _fileOperationInProgress = false; WorkspaceRoot.IsEnabled = true; }
    }

    private async Task<bool> SaveProjectAsync(bool choosePath)
    {
        try
        {
            var path = _filePath;
            if (choosePath || path is null)
            {
                var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                { Title = "保存 HmiStudio 工程", SuggestedFileName = "offline-project.hmistudio.json", DefaultExtension = "hmistudio.json", FileTypeChoices = [ProjectFileType], ShowOverwritePrompt = true });
                if (file is null) return false;
                path = file.TryGetLocalPath() ?? throw new IOException("仅支持本地文件。");
            }
            await ProjectStore.SaveAsync(_project, path); _filePath = path; _dirty = false; RefreshAll(); SetStatus($"已保存：{path}"); return true;
        }
        catch (Exception exception) { SetStatus($"保存失败，原文件保留：{exception.Message}"); return false; }
    }

    private async Task<bool> CanReplaceAsync()
    {
        if (!_dirty) return true;
        var dialog = new Window { Title = "未保存的修改", Width = 420, Height = 180, CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner };
        var actions = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };
        foreach (var (text, value) in new[] { ("保存", 1), ("放弃修改", 2), ("取消", 0) })
        { var button = new Button { Content = text }; button.Click += (_, _) => dialog.Close(value); actions.Children.Add(button); }
        dialog.Content = new StackPanel { Margin = new Thickness(24), Spacing = 24, Children = { new TextBlock { Text = "工程有未保存的修改。" }, actions } };
        var answer = await dialog.ShowDialog<int>(this);
        return answer == 2 || (answer == 1 && await SaveProjectAsync(false));
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_fileOperationInProgress) { e.Cancel = true; SetStatus("工程读写尚未结束，请稍候。"); return; }
        CompleteDrag();
        if (_allowClose || !_dirty) return;
        e.Cancel = true;
        var close = false;
        await RunFileOperationAsync(async () => { close = await CanReplaceAsync(); });
        if (close) { _allowClose = true; Close(); }
    }

    private async Task RunSmokeTestAsync(string outputDirectory)
    {
        var exitCode = 0;
        try
        {
            Directory.CreateDirectory(outputDirectory);
            await Task.Delay(700);
            var initial = Screen.Widgets.Count;
            AddWidget(WidgetKind.Rectangle);
            if (Screen.Widgets.Count != initial + 1) throw new InvalidOperationException("Add failed");
            UndoClick(null, new RoutedEventArgs());
            if (Screen.Widgets.Count != initial) throw new InvalidOperationException("Undo failed");
            RedoClick(null, new RoutedEventArgs());
            if (Screen.Widgets.Count != initial + 1) throw new InvalidOperationException("Redo failed");
            UndoClick(null, new RoutedEventArgs());
            var snapshot = ProjectStore.Clone(_project);
            _selection = Screen.Widgets.First(widget => widget.Kind == WidgetKind.Indicator).Id;
            var dragged = Selected!;
            _drag = new DragState(snapshot, new Point(0, 0), dragged.X, dragged.Y, new Border(), null);
            dragged.X += 2;
            CompleteDrag();
            if (!_dirty || _undo.Count == 0 || _drag is not null) throw new InvalidOperationException("Interrupted drag finalization failed");
            UndoClick(null, new RoutedEventArgs());
            await RunFileOperationAsync(async () =>
            {
                await Task.Yield();
                if (WorkspaceRoot.IsEnabled || Mutate(() => Screen.Widgets.Clear())) throw new InvalidOperationException("File-operation editing gate failed");
                var nestedRan = false;
                await RunFileOperationAsync(() => { nestedRan = true; return Task.CompletedTask; });
                if (nestedRan) throw new InvalidOperationException("File-operation reentry gate failed");
            });
            var projectPath = Path.Combine(outputDirectory, "smoke.hmistudio.json");
            await ProjectStore.SaveAsync(_project, projectPath);
            var reloaded = await ProjectStore.LoadAsync(projectPath);
            if (reloaded.Screens[0].Widgets.Count != initial) throw new InvalidOperationException("Roundtrip failed");
            ModeClick(null, new RoutedEventArgs());
            var boolean = _project.Tags.First(tag => tag.Type == TagValueType.Boolean);
            var before = _simulator.GetValue(boolean.Name);
            _simulator.Toggle(boolean.Name);
            if (_simulator.GetValue(boolean.Name) == before) throw new InvalidOperationException("Simulation failed");
            RefreshAll();
            SetStatus("离线编辑、撤销/重做、工程读写与模拟验证通过。");
            await Task.Delay(300);
            var size = new PixelSize((int)Bounds.Width, (int)Bounds.Height);
            using var image = new RenderTargetBitmap(size, new Vector(96, 96));
            image.Render(this); image.Save(Path.Combine(outputDirectory, "desktop-smoke.png"), new PngBitmapEncoderOptions());
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, "desktop-smoke.json"),
                JsonSerializer.Serialize(new { status = "pass", scope = "Windows desktop local smoke", initialWidgets = initial, checks = new[] { "window-render", "add", "undo", "redo", "interrupted-drag-finalization", "file-operation-gate", "save-load", "offline-toggle" }, hardwareVerified = false }));
        }
        catch (Exception exception)
        { exitCode = 1; await File.WriteAllTextAsync(Path.Combine(outputDirectory, "desktop-smoke-error.txt"), exception.ToString()); }
        finally
        { _allowClose = true; if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime lifetime) lifetime.Shutdown(exitCode); else Close(); }
    }

    private sealed record DragState(HmiProject Snapshot, Point Start, double X, double Y, Border Frame, IPointer? Pointer);
}
