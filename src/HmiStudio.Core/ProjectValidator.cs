namespace HmiStudio.Core;

public static class ProjectValidator
{
    public const int CurrentFormatVersion = 1;
    public const int MaximumScreens = 256;
    public const int MaximumWidgetsPerScreen = 10_000;
    public const int MaximumWidgetsPerProject = 20_000;
    public const int MaximumTags = 20_000;

    public static IReadOnlyList<string> Validate(HmiProject project)
    {
        var errors = new List<string>();
        if (project is null)
            return ["Project is required."];
        if (project.FormatVersion != CurrentFormatVersion)
            errors.Add($"Unsupported format version {project.FormatVersion}; expected {CurrentFormatVersion}.");
        ValidateName(project.Name, "Project name", errors);
        if (project.Screens is null || project.Screens.Count is < 1 or > MaximumScreens)
            errors.Add($"Project requires 1 to {MaximumScreens} screens.");
        if (project.Tags is null || project.Tags.Count > MaximumTags)
            errors.Add($"Project tags must contain at most {MaximumTags} entries.");
        // Reject oversized collections before walking user-controlled input.
        if (project.Screens is null || project.Screens.Count > MaximumScreens ||
            project.Tags is null || project.Tags.Count > MaximumTags)
            return errors;

        var tags = new Dictionary<string, HmiTag>(StringComparer.Ordinal);
        var tagNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in project.Tags)
        {
            if (tag is null)
            {
                errors.Add("A tag entry is null.");
                continue;
            }
            ValidateName(tag.Name, "Tag name", errors);
            if (!string.IsNullOrWhiteSpace(tag.Name))
            {
                if (!tagNames.Add(tag.Name))
                    errors.Add($"Duplicate tag name '{tag.Name}'.");
                tags.TryAdd(tag.Name, tag);
            }
            if (!Enum.IsDefined(tag.Type))
                errors.Add($"Tag '{tag.Name}' has an unknown value type.");
            if (!double.IsFinite(tag.InitialValue))
                errors.Add($"Tag '{tag.Name}' initial value must be finite.");
            if (tag.Type == TagValueType.Boolean && tag.InitialValue is not (0 or 1))
                errors.Add($"Boolean tag '{tag.Name}' initial value must be 0 or 1.");
        }

        var ids = new HashSet<Guid>();
        var screenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var widgetCount = 0;
        foreach (var screen in project.Screens)
        {
            if (screen is null)
            {
                errors.Add("A screen entry is null.");
                continue;
            }
            ValidateId(screen.Id, "Screen", ids, errors);
            ValidateName(screen.Name, "Screen name", errors);
            if (!string.IsNullOrWhiteSpace(screen.Name) && !screenNames.Add(screen.Name))
                errors.Add($"Duplicate screen name '{screen.Name}'.");
            if (screen.Width is < 1 or > 16_384 || screen.Height is < 1 or > 16_384)
                errors.Add($"Screen '{screen.Name}' dimensions must be between 1 and 16384.");
            if (!IsColor(screen.Background))
                errors.Add($"Screen '{screen.Name}' background must be #RRGGBB or #AARRGGBB.");
            if (screen.Widgets is null || screen.Widgets.Count > MaximumWidgetsPerScreen)
            {
                errors.Add($"Screen '{screen.Name}' requires a widget collection of at most {MaximumWidgetsPerScreen} entries.");
                continue;
            }
            widgetCount += screen.Widgets.Count;
            if (widgetCount > MaximumWidgetsPerProject)
            {
                errors.Add($"Project supports at most {MaximumWidgetsPerProject} total widgets.");
                return errors;
            }
            foreach (var widget in screen.Widgets)
            {
                if (widget is null)
                {
                    errors.Add($"Screen '{screen.Name}' contains a null widget.");
                    continue;
                }
                ValidateId(widget.Id, "Widget", ids, errors);
                if (!Enum.IsDefined(widget.Kind))
                    errors.Add($"Widget '{widget.Id}' has an unknown kind.");
                if (!double.IsFinite(widget.X) || !double.IsFinite(widget.Y) ||
                    !double.IsFinite(widget.Width) || !double.IsFinite(widget.Height) ||
                    widget.X < 0 || widget.Y < 0 || widget.Width <= 0 || widget.Height <= 0 ||
                    widget.X + widget.Width > screen.Width || widget.Y + widget.Height > screen.Height)
                    errors.Add($"Widget '{widget.Id}' must have finite, positive dimensions and fit inside its screen.");
                if (widget.Text is null || widget.Text.Length > 4096)
                    errors.Add($"Widget '{widget.Id}' text must contain at most 4096 characters.");
                if (!IsColor(widget.Fill) || !IsColor(widget.Foreground))
                    errors.Add($"Widget '{widget.Id}' colors must be #RRGGBB or #AARRGGBB.");
                var requiredType = widget.Kind switch
                {
                    WidgetKind.Button or WidgetKind.Indicator => (TagValueType?)TagValueType.Boolean,
                    WidgetKind.NumericDisplay => TagValueType.Number,
                    _ => null
                };
                if (requiredType is null)
                {
                    if (widget.TagName is not null)
                        errors.Add($"Widget '{widget.Id}' kind '{widget.Kind}' does not support tag binding.");
                }
                else if (string.IsNullOrWhiteSpace(widget.TagName) || !tags.TryGetValue(widget.TagName, out var boundTag))
                    errors.Add($"Widget '{widget.Id}' requires an existing {requiredType} tag with matching letter case.");
                else if (boundTag.Type != requiredType)
                    errors.Add($"Widget '{widget.Id}' requires a {requiredType} tag, but '{boundTag.Name}' is {boundTag.Type}.");
            }
        }
        return errors;
    }

    private static void ValidateName(string? name, string label, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 128 || name != name.Trim() || name.Any(char.IsControl))
            errors.Add($"{label} must be 1 to 128 characters without leading/trailing whitespace or control characters.");
    }

    private static void ValidateId(Guid id, string label, HashSet<Guid> ids, List<string> errors)
    {
        if (id == Guid.Empty || !ids.Add(id))
            errors.Add($"{label} ID '{id}' must be nonempty and unique across all screens and widgets.");
    }

    private static bool IsColor(string? value) => value is { Length: 7 or 9 } &&
        value[0] == '#' && value.AsSpan(1).IndexOfAnyExcept("0123456789abcdefABCDEF") < 0;
}
