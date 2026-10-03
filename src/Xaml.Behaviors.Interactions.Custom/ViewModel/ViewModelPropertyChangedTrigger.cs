// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.ComponentModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Triggers when the specified view model property changes.
/// </summary>
public partial class ViewModelPropertyChangedTrigger : StyledElementTrigger<Control>
{

    /// <summary>
    /// Gets or sets the name of the property to monitor. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? PropertyName { get; set; }

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
