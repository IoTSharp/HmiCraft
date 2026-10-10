using Avalonia;

namespace HmiStudio.Desktop;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}
