using Avalonia;
using Avalonia.Markup.Xaml;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
