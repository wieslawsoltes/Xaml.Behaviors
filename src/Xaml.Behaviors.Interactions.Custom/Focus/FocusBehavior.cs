// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Keeps a control focused while the <see cref="IsFocused"/> property is set.
/// </summary>
public partial class FocusBehavior : DisposingBehavior<Control>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(DefaultBindingMode = PropertyBindingMode.TwoWay)]
    public partial bool IsFocused { get; set; }

    /// <summary>
    /// Subscribes to focus changes and updates <see cref="IsFocused"/> accordingly.
    /// </summary>
    /// <returns>A disposable used to detach the event handlers.</returns>
    protected override System.IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

#if UNO
        var associatedObject = AssociatedObject;

        void OnLostFocus(object? sender, RoutedEventArgs e)
        {
            // LostFocus bubbles on WinUI: only the associated object losing focus clears the flag.
            if (ReferenceEquals(e.OriginalSource, associatedObject))
            {
                SetCurrentValue(IsFocusedProperty, false);
            }
        }

        associatedObject.LostFocus += OnLostFocus;
        var associatedObjectIsFocusedObservableDispose = DisposableAction.Create(() => associatedObject.LostFocus -= OnLostFocus);
#else
        var associatedObjectIsFocusedObservableDispose = AssociatedObject.GetObservable(Avalonia.Input.InputElement.IsFocusedProperty)
            .Subscribe(new AnonymousObserver<bool>(
                focused =>
                {
                    if (!focused)
                    {
                        SetCurrentValue(IsFocusedProperty, false);
                    }
                }));
#endif

        var isFocusedObservableDispose = this.GetObservable<bool>(IsFocusedProperty)
            .Subscribe(new AnonymousObserver<bool>(
                focused =>
                {
                    if (focused)
                    {
                        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
                    }
                }));

        return DisposableAction.Create(() =>
        {
            associatedObjectIsFocusedObservableDispose.Dispose();
            isFocusedObservableDispose.Dispose();
        });
    }
}
