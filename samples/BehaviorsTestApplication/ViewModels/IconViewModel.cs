#if UNO
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#else
using Avalonia.Media;
#endif

namespace BehaviorsTestApplication.ViewModels;

public class IconViewModel : ViewModelBase
{
#if UNO
    // WinUI parses path markup with the XAML type converter.
    public Geometry PlusIcon { get; } = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), "M8 3v5H3v2h5v5h2V10h5V8h-5V3z");
    public Geometry MinusIcon { get; } = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), "M3 7v2h10V7z");
#else
    public Geometry PlusIcon { get; } = Geometry.Parse("M8 3v5H3v2h5v5h2V10h5V8h-5V3z");
    public Geometry MinusIcon { get; } = Geometry.Parse("M3 7v2h10V7z");
#endif
}
