// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the window state matches the specified value.
/// </summary>
public partial class WindowStateTrigger : AttachedToVisualTreeTriggerBase<Control>
{

    /// <summary>
    /// Gets or sets the window. If not set, the visual root window is used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Window? Window { get; set; }

    /// <summary>
    /// Gets or sets the window state to trigger on. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial WindowState State { get; set; }

    private IDisposable? _subscription;

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var window = Window ?? TopLevel.GetTopLevel(AssociatedObject) as Window;
        if (window is null)
        {
            return DisposableAction.Empty;
        }

        _subscription = window.GetObservable(Window.WindowStateProperty)
            .Subscribe(new AnonymousObserver<WindowState>(OnStateChanged));
        OnStateChanged(window.WindowState);

        return DisposableAction.Create(() => _subscription?.Dispose());
    }

    private void OnStateChanged(WindowState state)
    {
        if (!IsEnabled || state != State)
        {
            return;
        }

        Dispatcher.UIThread.Post(() => Interaction.ExecuteActions(AssociatedObject, Actions, state));
    }
}
