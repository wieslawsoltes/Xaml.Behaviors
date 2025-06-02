// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Triggers when the specified view model property changes.
/// </summary>
public class ViewModelPropertyChangedTrigger : StyledElementTrigger<Control>
{
    /// <summary>
    /// Identifies the <see cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<ViewModelPropertyChangedTrigger, string?>(nameof(PropertyName));

    /// <summary>
    /// Gets or sets the name of the property to monitor. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    private INotifyPropertyChanged? _inpc;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        _inpc = AssociatedObject?.DataContext as INotifyPropertyChanged;
        if (_inpc is not null)
        {
            _inpc.PropertyChanged += OnPropertyChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (_inpc is not null)
        {
            _inpc.PropertyChanged -= OnPropertyChanged;
            _inpc = null;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, PropertyName, System.StringComparison.Ordinal))
        {
            Dispatcher.UIThread.Post(() =>
                Interaction.ExecuteActions(AssociatedObject!, Actions, e));
        }
    }
}
