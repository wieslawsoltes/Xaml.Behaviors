using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public partial class BindableValidationRule001 : Window
{
    public BindableValidationRule001()
    {
        InitializeComponent();
    }
}

public class ValidationRuleBindingSource : AvaloniaObject
{
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, double>(nameof(Minimum));

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, double>(nameof(Maximum));

    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<ValidationRuleBindingSource, string?>(nameof(ErrorMessage));

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }
}
