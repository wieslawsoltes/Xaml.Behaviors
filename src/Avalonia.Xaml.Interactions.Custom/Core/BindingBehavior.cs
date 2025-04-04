using System;
using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class BindingBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<AvaloniaProperty?> TargetPropertyProperty =
        AvaloniaProperty.Register<BindingBehavior, AvaloniaProperty?>(nameof(TargetProperty));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<AvaloniaObject?> TargetObjectProperty =
        AvaloniaProperty.Register<BindingBehavior, AvaloniaObject?>(nameof(TargetObject));

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<IBinding?> BindingProperty =
        AvaloniaProperty.Register<BindingBehavior, IBinding?>(nameof(Binding));

    /// <summary>
    /// 
    /// </summary>
    public AvaloniaProperty? TargetProperty
    {
        get => GetValue(TargetPropertyProperty);
        set => SetValue(TargetPropertyProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [ResolveByName]
    public AvaloniaObject? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [AssignBinding]
    public IBinding? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (TargetObject is not null && TargetProperty is not null && Binding is not null)
        {
            return TargetObject.Bind(TargetProperty, Binding);
        }

        return DisposableAction.Empty;
    }
}
