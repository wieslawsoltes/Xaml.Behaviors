using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Automation;

/// <summary>
/// Sets <see cref="AutomationProperties.NameProperty"/> on the associated control when attached.
/// </summary>
public class AutomationNameBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Name"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> NameProperty =
        AvaloniaProperty.Register<AutomationNameBehavior, string?>(nameof(Name));

    /// <summary>
    /// Gets or sets the automation name. This is an avalonia property.
    /// </summary>
    public string? Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateName();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        SetAutomationName(null);
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == NameProperty)
        {
            UpdateName();
        }
    }

    private void UpdateName()
    {
        SetAutomationName(Name);
    }

    private void SetAutomationName(string? value)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.SetValue(AutomationProperties.NameProperty, value);
    }
}
