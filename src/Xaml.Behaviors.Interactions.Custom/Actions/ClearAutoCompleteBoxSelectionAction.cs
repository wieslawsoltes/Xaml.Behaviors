using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Clears the selection and text of an <see cref="AutoCompleteBox"/>.
/// </summary>
public class ClearAutoCompleteBoxSelectionAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="AutoCompleteBox"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AutoCompleteBox?> AutoCompleteBoxProperty =
        AvaloniaProperty.Register<ClearAutoCompleteBoxSelectionAction, AutoCompleteBox?>(nameof(AutoCompleteBox));

    /// <summary>
    /// Gets or sets the target <see cref="AutoCompleteBox"/>. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public AutoCompleteBox? AutoCompleteBox
    {
        get => GetValue(AutoCompleteBoxProperty);
        set => SetValue(AutoCompleteBoxProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var box = AutoCompleteBox ?? sender as AutoCompleteBox;
        if (box is null)
        {
            return false;
        }

        box.SelectedItem = null;
        box.Text = string.Empty;

        return null;
    }
}
