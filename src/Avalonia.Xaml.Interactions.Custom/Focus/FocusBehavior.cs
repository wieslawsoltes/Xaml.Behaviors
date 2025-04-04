using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class FocusBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
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
