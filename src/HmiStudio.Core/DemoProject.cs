namespace HmiStudio.Core;

public static class DemoProject
{
    public static HmiProject Create() => new()
    {
        Name = "Offline pump station",
        Tags =
        [
            new() { Name = "PumpRunning", Type = TagValueType.Boolean, InitialValue = 0 },
            new() { Name = "TankLevel", Type = TagValueType.Number, InitialValue = 64.5 }
        ],
        Screens =
        [
            new()
            {
                Id = Guid.Parse("014169fe-9373-4e65-b383-3fd15d8ca001"),
                Name = "Overview", Width = 800, Height = 480, Background = "#101927",
                Widgets =
                [
                    Widget(1, WidgetKind.Rectangle, 24, 24, 752, 72, "", "#1C2A3C"),
                    Widget(2, WidgetKind.Text, 44, 38, 480, 34, "PUMP STATION / OFFLINE", "#1C2A3C"),
                    Widget(3, WidgetKind.Rectangle, 24, 116, 352, 280, "", "#1C2A3C"),
                    Widget(4, WidgetKind.Text, 44, 134, 270, 32, "CIRCULATION PUMP", "#1C2A3C"),
                    Widget(5, WidgetKind.Indicator, 54, 188, 64, 64, "Pump state", "#38D9A9", "PumpRunning"),
                    Widget(6, WidgetKind.Text, 144, 199, 196, 34, "RUN / STOP", "#1C2A3C"),
                    Widget(7, WidgetKind.Button, 44, 294, 312, 70, "TOGGLE PUMP", "#246FDC", "PumpRunning"),
                    Widget(8, WidgetKind.Rectangle, 396, 116, 380, 280, "", "#1C2A3C"),
                    Widget(9, WidgetKind.Text, 416, 134, 320, 32, "TANK LEVEL (%)", "#1C2A3C"),
                    Widget(10, WidgetKind.NumericDisplay, 416, 192, 320, 78, "Tank level", "#101927", "TankLevel"),
                    Widget(11, WidgetKind.Ellipse, 426, 304, 44, 44, "", "#49A8E8"),
                    Widget(12, WidgetKind.Text, 484, 305, 260, 36, "LOCAL SIMULATION", "#1C2A3C"),
                    Widget(13, WidgetKind.Text, 24, 422, 752, 32, "Independent project format | No device connection", "#101927")
                ]
            }
        ]
    };

    private static HmiWidget Widget(int number, WidgetKind kind, double x, double y,
        double width, double height, string text, string fill, string? tagName = null) => new()
        {
            Id = Guid.Parse($"014169fe-9373-4e65-b383-3fd15d8cb{number:000}"),
            Kind = kind,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Text = text,
            Fill = fill,
            Foreground = "#F0F4FA",
            TagName = tagName
        };
}
