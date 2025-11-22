using Avalonia.Headless;
using Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
    }
}
