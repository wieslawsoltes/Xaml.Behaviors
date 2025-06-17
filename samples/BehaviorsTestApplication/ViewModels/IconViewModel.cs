using Avalonia.Media;

namespace BehaviorsTestApplication.ViewModels;

public class IconViewModel : ViewModelBase
{
    public Geometry PlusIcon { get; } = Geometry.Parse("M8 3v5H3v2h5v5h2V10h5V8h-5V3z");
    public Geometry MinusIcon { get; } = Geometry.Parse("M3 7v2h10V7z");
}
