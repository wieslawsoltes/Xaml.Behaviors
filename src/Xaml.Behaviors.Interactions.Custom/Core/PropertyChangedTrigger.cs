// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Represents a trigger that performs actions when the bound data have changed.
/// </summary>
public class PropertyChangedTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> BindingProperty =
        AvaloniaProperty.Register<PropertyChangedTrigger, object?>(nameof(Binding));

    /// <summary>
    /// Gets or sets a binding object that the trigger will listen to. This is an avalonia property.
    /// </summary>
    public object? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BindingProperty)
        {
            OnBindingChanged(change);
        }
    }

    private void OnBindingChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is not PropertyChangedTrigger behavior)
        {
            return;
        }

        Dispatcher.UIThread.Post(() => behavior.Execute(args));
    }

    private void Execute(object? parameter)
    {
        if (AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }
}
