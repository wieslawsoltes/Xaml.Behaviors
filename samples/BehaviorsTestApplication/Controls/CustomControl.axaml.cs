using Avalonia;
using Avalonia.Controls.Primitives;

namespace BehaviorsTestApplication.Controls;

public class CustomControl : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMenuOpenProperty = 
        AvaloniaProperty.Register<CustomControl, bool>(nameof(IsMenuOpen));

    public bool IsMenuOpen
    {
        get => GetValue(IsMenuOpenProperty);
        set => SetValue(IsMenuOpenProperty, value);
    }
}
