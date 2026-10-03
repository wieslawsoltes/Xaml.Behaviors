using Avalonia;
using Avalonia.Headless;
using Xaml.PropertyGenerator.UnitTests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Xaml.PropertyGenerator.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
