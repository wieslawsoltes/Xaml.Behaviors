#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Avalonia;
using Avalonia.Controls.Primitives;
#endif

namespace BehaviorsTestApplication.Controls;

public class CustomControl : TemplatedControl
{
#if UNO
    public static readonly DependencyProperty IsMenuOpenProperty =
        DependencyProperty.Register(nameof(IsMenuOpen), typeof(bool), typeof(CustomControl), new PropertyMetadata(false));
#else
    public static readonly StyledProperty<bool> IsMenuOpenProperty = 
        AvaloniaProperty.Register<CustomControl, bool>(nameof(IsMenuOpen));
#endif

    public bool IsMenuOpen
    {
        get => (bool)GetValue(IsMenuOpenProperty);
        set => SetValue(IsMenuOpenProperty, value);
    }
}
