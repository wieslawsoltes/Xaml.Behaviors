using System;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using ReactiveUI.Avalonia;

namespace BehaviorsTestApplication;

class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        GC.KeepAlive(typeof(Interaction).Assembly);
        GC.KeepAlive(typeof(ComparisonConditionType).Assembly);
        return AppBuilder.Configure<App>()
            .WithInterFont()
            .UsePlatformDetect()
            .UseReactiveUI()
            .LogToTrace();
    }
}
