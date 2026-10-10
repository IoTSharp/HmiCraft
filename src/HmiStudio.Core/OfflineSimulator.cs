namespace HmiStudio.Core;

/// <summary>In-memory tag values only; this simulator never connects to a device.</summary>
public sealed class OfflineSimulator
{
    private readonly Dictionary<string, RuntimeTag> _tags;

    public OfflineSimulator(HmiProject project)
    {
        var errors = ProjectValidator.Validate(project);
        if (errors.Count != 0)
            throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(project));
        _tags = project.Tags.ToDictionary(tag => tag.Name,
            tag => new RuntimeTag(tag.Type, tag.InitialValue), StringComparer.Ordinal);
    }

    public double GetValue(string tagName) => Find(tagName).Value;

    public void SetValue(string tagName, double value)
    {
        var tag = Find(tagName);
        if (!double.IsFinite(value) || (tag.Type == TagValueType.Boolean && value is not (0 or 1)))
            throw new ArgumentOutOfRangeException(nameof(value), "Values must be finite; Boolean values must be 0 or 1.");
        tag.Value = value;
    }

    public void Toggle(string tagName)
    {
        var tag = Find(tagName);
        if (tag.Type != TagValueType.Boolean)
            throw new InvalidOperationException($"Tag '{tagName}' is not Boolean.");
        tag.Value = tag.Value == 0 ? 1 : 0;
    }

    public void Reset()
    {
        foreach (var tag in _tags.Values)
            tag.Value = tag.InitialValue;
    }

    private RuntimeTag Find(string tagName) => _tags.TryGetValue(tagName, out var tag)
        ? tag : throw new KeyNotFoundException($"Unknown tag '{tagName}'.");

    private sealed class RuntimeTag(TagValueType type, double initialValue)
    {
        public TagValueType Type { get; } = type;
        public double InitialValue { get; } = initialValue;
        public double Value { get; set; } = initialValue;
    }
}
