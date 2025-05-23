using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Keeps a control focused while the <see cref="IsFocused"/> property is set.
/// </summary>
public class FocusBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// Gets or sets a value indicating whether the control should be focused.
    /// </summary>
    public static readonly StyledProperty<bool> IsFocusedProperty =
        AvaloniaProperty.Register<FocusBehavior, bool>(nameof(IsFocused), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// 
    /// </summary>
    public bool IsFocused
    {
        get => GetValue(IsFocusedProperty);
        set => SetValue(IsFocusedProperty, value);
    }

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

        var associatedObjectIsFocusedObservableDispose = AssociatedObject.GetObservable(Avalonia.Input.InputElement.IsFocusedProperty)
            .Subscribe(new AnonymousObserver<bool>(
                focused =>
                {
                    if (!focused)
                    {
                        SetCurrentValue(IsFocusedProperty, false);
                    }
                }));

        var isFocusedObservableDispose = this.GetObservable(IsFocusedProperty)
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
