// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the associated control's <see cref="StyledElement.ActualThemeVariant"/>
/// matches the specified <see cref="ThemeVariant"/>.
/// </summary>
public partial class ThemeVariantTrigger : StyledElementTrigger<StyledElement>
{

    /// <summary>
    /// Gets or sets the theme variant to watch for. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ThemeVariant? ThemeVariant { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        Evaluate();
    }

    /// <inheritdoc />
    protected override void OnActualThemeVariantChangedEvent()
    {
        Evaluate();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeVariantProperty)
        {
            Evaluate();
        }
    }

    private void Evaluate()
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        if (AssociatedObject.ActualThemeVariant == ThemeVariant)
        {
            Dispatcher.UIThread.Post(() => Execute(null));
        }
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
