using System.Text.Json.Serialization;

namespace HmiStudio.Core;

public sealed class HmiProject
{
    [JsonRequired]
    public int FormatVersion { get; set; } = 1;
    public string Name { get; set; } = "Untitled";
    [JsonRequired]
    public List<HmiScreen> Screens { get; set; } = [];
    [JsonRequired]
    public List<HmiTag> Tags { get; set; } = [];
}

public sealed class HmiScreen
{
    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Screen";
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 480;
    public string Background { get; set; } = "#101927";
    public List<HmiWidget> Widgets { get; set; } = [];
}

public enum WidgetKind { Rectangle, Ellipse, Text, Button, Indicator, NumericDisplay }

public sealed class HmiWidget
{
    [JsonRequired]
    public Guid Id { get; set; } = Guid.NewGuid();
    public WidgetKind Kind { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 100;
    public double Height { get; set; } = 50;
    public string Text { get; set; } = "";
    public string Fill { get; set; } = "#25354A";
    public string Foreground { get; set; } = "#F0F4FA";
    public string? TagName { get; set; }
}

public enum TagValueType { Boolean, Number }

public sealed class HmiTag
{
    public string Name { get; set; } = "Tag";
    public TagValueType Type { get; set; }
    public double InitialValue { get; set; }
}
