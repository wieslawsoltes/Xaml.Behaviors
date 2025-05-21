using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Binds the <see cref="InputElement.IsPointerOverProperty"/> to the <see cref="IsPointerOver"/> property.
/// </summary>
public class BindPointerOverBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// Gets or sets a value indicating whether the pointer is over the control.
    /// </summary>
	public static readonly StyledProperty<bool> IsPointerOverProperty =
		AvaloniaProperty.Register<BindPointerOverBehavior, bool>(nameof(IsPointerOver), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// 
    /// </summary>
	public bool IsPointerOver
	{
		get => GetValue(IsPointerOverProperty);
		set => SetValue(IsPointerOverProperty, value);
	}

    /// <summary>
    /// Called when the behavior is attached to the control.
    /// </summary>
    /// <returns>A disposable that removes the event handler.</returns>
        protected override IDisposable OnAttachedOverride()
	{
		if (AssociatedObject is null)
		{
			return DisposableAction.Empty;
		}

        var control = AssociatedObject;
        control.PropertyChanged += AssociatedObjectOnPropertyChanged;

        return DisposableAction.Create(() =>
        {
            control.PropertyChanged -= AssociatedObjectOnPropertyChanged;
            IsPointerOver = false;
        });

        void AssociatedObjectOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == InputElement.IsPointerOverProperty)
            {
                IsPointerOver = e.NewValue is true;
            }
        }
	}
}
