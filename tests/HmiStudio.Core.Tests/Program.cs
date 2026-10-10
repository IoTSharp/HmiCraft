using System.Diagnostics;
using System.Text.Json.Nodes;
using HmiStudio.Core;

var testRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), $"hmistudio-core-tests-{Guid.NewGuid():N}"));
Directory.CreateDirectory(testRoot);
var clock = Stopwatch.StartNew();
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
var tests = new (string Name, Func<Task> Run)[]
{
    ("demo validates and JSON round-trip preserves the document", RoundTrip),
    ("undo snapshots deeply isolate mutable project objects", CloneIsolation),
    ("unknown format version and JSON members fail closed", FutureVersion),
    ("duplicate screen/widget IDs and tag names are rejected", Duplicates),
    ("widget geometry, colors and finite tag values are validated", InvalidGeometry),
    ("widget bindings require the correct tag type", Bindings),
    ("simulation is isolated and resets to its own initial snapshot", SimulationIsolation),
    ("simulation rejects invalid writes and unknown tags", SimulationErrors),
    ("invalid and cancelled saves preserve existing bytes", FailedSave),
    ("failed replacement preserves target and removes owned temporary file", FailedReplacement),
    ("null or oversized documents fail closed", InvalidDocument)
};
var failures = 0;
try
{
    foreach (var test in tests)
    {
        deadline.Token.ThrowIfCancellationRequested();
        try
        {
            await test.Run().WaitAsync(TimeSpan.FromSeconds(15), deadline.Token);
            Console.WriteLine($"PASS {test.Name}");
        }
        catch (Exception exception)
        {
            failures++;
            Console.Error.WriteLine($"FAIL {test.Name}: {exception}");
        }
    }
    Console.WriteLine($"{tests.Length - failures}/{tests.Length} contracts passed in {clock.Elapsed.TotalSeconds:F2}s.");
    if (failures == 0 && args is ["--write-demo", var demoPath])
    {
        await ProjectStore.SaveAsync(DemoProject.Create(), demoPath, deadline.Token);
        Console.WriteLine($"Independent demo written: {Path.GetFullPath(demoPath)}");
    }
    return failures == 0 ? 0 : 1;
}
finally
{
    var actual = Path.GetFullPath(testRoot);
    if (actual == testRoot && Path.GetFileName(actual).StartsWith("hmistudio-core-tests-", StringComparison.Ordinal))
        Directory.Delete(actual, recursive: true);
    Console.WriteLine($"Owned test directory removed: {!Directory.Exists(testRoot)}");
}

async Task RoundTrip()
{
    var original = DemoProject.Create();
    Check(ProjectValidator.Validate(original).Count == 0, "Demo is invalid.");
    var first = Path.Combine(testRoot, "roundtrip-1.hmistudio.json");
    var second = Path.Combine(testRoot, "roundtrip-2.hmistudio.json");
    await ProjectStore.SaveAsync(original, first, deadline.Token);
    var loaded = await ProjectStore.LoadAsync(first, deadline.Token);
    await ProjectStore.SaveAsync(loaded, second, deadline.Token);
    var firstBytes = await File.ReadAllBytesAsync(first, deadline.Token);
    var secondBytes = await File.ReadAllBytesAsync(second, deadline.Token);
    Check(firstBytes.SequenceEqual(secondBytes), "Round-trip changed the document.");
    Check(!ReferenceEquals(original.Screens[0], loaded.Screens[0]), "Load reused source instances.");
}

Task CloneIsolation()
{
    var original = DemoProject.Create();
    var snapshot = ProjectStore.Clone(original);
    original.Name = "Changed";
    original.Tags[0].InitialValue = 1;
    original.Screens[0].Widgets[0].X = 30;
    Check(snapshot.Name == "Offline pump station", "Clone shared project state.");
    Check(snapshot.Tags[0].InitialValue == 0, "Clone shared tag instances.");
    Check(snapshot.Screens[0].Widgets[0].X == 24, "Clone shared widget instances.");
    Check(snapshot.Screens[0].Id == original.Screens[0].Id, "Clone changed stable identifiers.");
    return Task.CompletedTask;
}

async Task FutureVersion()
{
    var path = await SaveDemo("future");
    var node = JsonNode.Parse(await File.ReadAllTextAsync(path, deadline.Token))!;
    node["formatVersion"] = 2;
    await File.WriteAllTextAsync(path, node.ToJsonString(), deadline.Token);
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.LoadAsync(path, deadline.Token));
    node["formatVersion"] = 1;
    node["futureDeviceProtocol"] = "unknown";
    await File.WriteAllTextAsync(path, node.ToJsonString(), deadline.Token);
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.LoadAsync(path, deadline.Token));
}

Task Duplicates()
{
    var project = DemoProject.Create();
    project.Screens[0].Widgets[0].Id = project.Screens[0].Id;
    Check(HasError(project, "ID"), "Cross-kind duplicate IDs accepted.");
    project = DemoProject.Create();
    project.Screens.Add(project.Screens[0]);
    Check(HasError(project, "Duplicate screen name"), "Duplicate screen names accepted.");
    project = DemoProject.Create();
    project.Tags.Add(new() { Name = "pumprunning", Type = TagValueType.Boolean });
    Check(HasError(project, "Duplicate tag name"), "Case-variant duplicate tag accepted.");
    return Task.CompletedTask;
}

Task InvalidGeometry()
{
    var project = DemoProject.Create();
    project.Screens[0].Widgets[0].X = double.NaN;
    project.Screens[0].Widgets[1].Width = 5000;
    project.Screens[0].Widgets[2].Fill = "red";
    project.Tags[1].InitialValue = double.PositiveInfinity;
    Check(ProjectValidator.Validate(project).Count >= 4, "Invalid values were accepted.");
    project = DemoProject.Create();
    project.Screens[0].Widgets[0].Id = Guid.Empty;
    project.Screens[0].Widgets[0].Kind = (WidgetKind)999;
    project.Tags[0].InitialValue = 0.5;
    Check(ProjectValidator.Validate(project).Count >= 3, "Unknown kind/ID/Boolean accepted.");
    return Task.CompletedTask;
}

Task Bindings()
{
    var project = DemoProject.Create();
    var button = project.Screens[0].Widgets.Single(widget => widget.Kind == WidgetKind.Button);
    button.TagName = "TankLevel";
    Check(HasError(project, "requires a Boolean tag"), "Button accepted numeric binding.");
    button.TagName = "pumprunning";
    Check(HasError(project, "existing Boolean tag"), "Nonexistent case-variant binding accepted.");
    project = DemoProject.Create();
    project.Screens[0].Widgets[0].TagName = "PumpRunning";
    project.Screens[0].Widgets.Single(widget => widget.Kind == WidgetKind.NumericDisplay).TagName = "PumpRunning";
    Check(ProjectValidator.Validate(project).Count == 2, "Unsupported binding accepted.");
    return Task.CompletedTask;
}

Task SimulationIsolation()
{
    var project = DemoProject.Create();
    var first = new OfflineSimulator(project);
    var second = new OfflineSimulator(project);
    first.Toggle("PumpRunning");
    first.SetValue("TankLevel", 75);
    Check(first.GetValue("PumpRunning") == 1 && first.GetValue("TankLevel") == 75, "Runtime write failed.");
    Check(project.Tags[0].InitialValue == 0 && project.Tags[1].InitialValue == 64.5, "Runtime mutated project.");
    Check(second.GetValue("PumpRunning") == 0 && second.GetValue("TankLevel") == 64.5, "Runtime state leaked.");
    project.Tags[1].InitialValue = 23;
    first.Reset();
    Check(first.GetValue("PumpRunning") == 0 && first.GetValue("TankLevel") == 64.5, "Reset did not use captured initial values.");
    first.Toggle("PumpRunning");
    first.Toggle("PumpRunning");
    Check(first.GetValue("PumpRunning") == 0, "Boolean toggling failed.");
    return Task.CompletedTask;
}

Task SimulationErrors()
{
    var simulator = new OfflineSimulator(DemoProject.Create());
    Throws<InvalidOperationException>(() => simulator.Toggle("TankLevel"));
    Throws<ArgumentOutOfRangeException>(() => simulator.SetValue("PumpRunning", 2));
    Throws<ArgumentOutOfRangeException>(() => simulator.SetValue("TankLevel", double.NaN));
    Throws<KeyNotFoundException>(() => simulator.GetValue("Missing"));
    Check(simulator.GetValue("PumpRunning") == 0 && simulator.GetValue("TankLevel") == 64.5, "Failed operations changed values.");
    return Task.CompletedTask;
}

async Task FailedSave()
{
    var path = await SaveDemo("preserved");
    var original = await File.ReadAllBytesAsync(path, deadline.Token);
    var project = DemoProject.Create();
    project.FormatVersion = 99;
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.SaveAsync(project, path, deadline.Token));
    using var cancelled = new CancellationTokenSource();
    cancelled.Cancel();
    await ThrowsAsync<OperationCanceledException>(() => ProjectStore.SaveAsync(DemoProject.Create(), path, cancelled.Token));
    var current = await File.ReadAllBytesAsync(path, deadline.Token);
    Check(original.SequenceEqual(current), "Failed save changed existing bytes.");
}

async Task FailedReplacement()
{
    var destination = Path.Combine(testRoot, "existing-directory.hmistudio.json");
    Directory.CreateDirectory(destination);
    var marker = Path.Combine(destination, "keep.txt");
    await File.WriteAllTextAsync(marker, "keep", deadline.Token);
    var replacementFailed = false;
    try
    {
        await ProjectStore.SaveAsync(DemoProject.Create(), destination, deadline.Token);
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
    {
        replacementFailed = true;
    }
    Check(replacementFailed, "Replacing an existing directory unexpectedly succeeded.");
    Check(await File.ReadAllTextAsync(marker, deadline.Token) == "keep", "Failed replacement changed destination.");
    Check(!Directory.EnumerateFiles(testRoot, "*.tmp").Any(), "Failed save left a temporary file.");
}

async Task InvalidDocument()
{
    var path = Path.Combine(testRoot, "null.hmistudio.json");
    await File.WriteAllTextAsync(path, "null", deadline.Token);
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.LoadAsync(path, deadline.Token));
    await File.WriteAllTextAsync(path, "{\"formatVersion\":1,\"name\":\"Bad\",\"screens\":[null],\"tags\":null}", deadline.Token);
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.LoadAsync(path, deadline.Token));
    await using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        stream.SetLength(ProjectStore.MaximumDocumentBytes + 1L);
    await ThrowsAsync<InvalidDataException>(() => ProjectStore.LoadAsync(path, deadline.Token));
}

async Task<string> SaveDemo(string name)
{
    var path = Path.Combine(testRoot, $"{name}.hmistudio.json");
    await ProjectStore.SaveAsync(DemoProject.Create(), path, deadline.Token);
    return path;
}

static bool HasError(HmiProject project, string text) => ProjectValidator.Validate(project)
    .Any(error => error.Contains(text, StringComparison.Ordinal));

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}

static async Task ThrowsAsync<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}
