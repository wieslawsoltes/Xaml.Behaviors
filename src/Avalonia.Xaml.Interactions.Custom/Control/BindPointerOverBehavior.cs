using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class BindPointerOverBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
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
