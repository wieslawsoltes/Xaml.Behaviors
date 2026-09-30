#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public partial class BindableValidationRule001 : Window
{
    public BindableValidationRule001()
    {
        InitializeComponent();
    }
#if UNO

    // WinUI validation rules are not in the element tree and inherit no DataContext: the page binds them with x:Bind.
    internal ValidationRuleBindingSource Source => (ValidationRuleBindingSource)DataContext;
#endif
}

#if UNO
/// <summary>
/// A <see cref="Xaml.Interactions.Custom.RangeValidationRule{T}"/> of <see cref="double"/> values: WinUI XAML has no
/// <c>x:TypeArguments</c>.
/// </summary>
public partial class DoubleRangeValidationRule : Xaml.Interactions.Custom.RangeValidationRule<double>
{
}
#endif

public partial class ValidationRuleBindingSource : AvaloniaObject
{
#if UNO
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(ValidationRuleBindingSource), new PropertyMetadata(0d));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(ValidationRuleBindingSource), new PropertyMetadata(0d));

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(ValidationRuleBindingSource), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, double>(nameof(Minimum));

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, double>(nameof(Maximum));

    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, string?>(nameof(ErrorMessage));
#endif

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public string? ErrorMessage
    {
        get => (string?)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }
}
