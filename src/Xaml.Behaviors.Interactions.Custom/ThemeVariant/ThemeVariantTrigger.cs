// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the associated control's <see cref="StyledElement.ActualThemeVariant"/>
/// matches the specified <see cref="ThemeVariant"/>.
/// </summary>
public class ThemeVariantTrigger : StyledElementTrigger<StyledElement>
{
    /// <summary>
    /// Identifies the <see cref="ThemeVariant"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ThemeVariant?> ThemeVariantProperty =
        AvaloniaProperty.Register<ThemeVariantTrigger, ThemeVariant?>(nameof(ThemeVariant));

    /// <summary>
    /// Gets or sets the theme variant to watch for. This is an avalonia property.
    /// </summary>
    public ThemeVariant? ThemeVariant
    {
        get => GetValue(ThemeVariantProperty);
        set => SetValue(ThemeVariantProperty, value);
    }

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
