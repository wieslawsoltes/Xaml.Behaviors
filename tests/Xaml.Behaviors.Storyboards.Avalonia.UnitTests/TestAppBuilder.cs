using Avalonia.Headless;
using Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
