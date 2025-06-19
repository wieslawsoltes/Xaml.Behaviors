// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that calls a method on a specified object when invoked.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class CallMethodAction : StyledElementAction
{
    internal MethodHelper MethodHelper { get; } = new();

    /// <summary>
    /// Identifies the <seealso cref="MethodName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MethodNameProperty =
        AvaloniaProperty.Register<CallMethodAction, string?>(nameof(MethodName));

    /// <summary>
    /// Identifies the <seealso cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetObjectProperty =
        AvaloniaProperty.Register<CallMethodAction, object?>(nameof(TargetObject));

    /// <summary>
    /// Gets or sets the name of the method to invoke. This is an avalonia property.
    /// </summary>
    public string? MethodName
    {
        get => GetValue(MethodNameProperty);
        set => SetValue(MethodNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the object that exposes the method of interest. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public object? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == MethodNameProperty)
        {
            var methodName = change.GetNewValue<string?>();
            MethodHelper.UpdateMethodDescriptors(methodName);
        }

        if (change.Property == TargetObjectProperty)
        {
            var newValue = change.GetNewValue<object?>();
            if (newValue is not null)
            {
                var newType = newValue.GetType();
                MethodHelper.UpdateTargetType(newType, MethodName);
            }
        }
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the method is called; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var methodName = MethodName;
        if (string.IsNullOrEmpty(methodName))
        {
            return false;
        }

        var target = GetValue(TargetObjectProperty) is not null ? TargetObject : sender;
        if (target is null)
        {
            return false;
        }

        MethodHelper.UpdateTargetType(target.GetType(), methodName);

        var methodDescriptor = MethodHelper.FindBestMethod(parameter);
        if (methodDescriptor is null)
        {
            if (TargetObject is not null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find method named {0} on object of type {1} that matches the expected signature.",
                    methodName,
                    MethodHelper.TargetObjectType));
            }

            return false;
        }

        var parameters = methodDescriptor.Parameters;
        switch (parameters.Length)
        {
            case 0:
                methodDescriptor.MethodInfo.Invoke(target, null);
                return true;
            case 2:
                methodDescriptor.MethodInfo.Invoke(target, [target, parameter!]);
                return true;
            default:
                return false;
        }
    }
}
