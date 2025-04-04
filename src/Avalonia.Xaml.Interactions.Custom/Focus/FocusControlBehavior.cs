using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class FocusControlBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<bool> FocusFlagProperty =
        AvaloniaProperty.Register<FocusControlBehavior, bool>(nameof(FocusFlag));

    /// <summary>
    /// 
    /// </summary>
    public bool FocusFlag
    {
        get => GetValue(FocusFlagProperty);
        set => SetValue(FocusFlagProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FocusFlagProperty)
        {
            var focusFlag = change.GetNewValue<bool>();
            if (focusFlag && IsEnabled)
            {
                Execute();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (FocusFlag && IsEnabled)
        {
            Execute();
        }
        
        return DisposableAction.Empty;
    }

    private void Execute()
    {
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
    }
}
