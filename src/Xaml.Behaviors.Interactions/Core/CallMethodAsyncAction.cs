// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that calls an asynchronous method on a specified object when invoked.
/// </summary>
public class CallMethodAsyncAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="MethodName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MethodNameProperty =
        AvaloniaProperty.Register<CallMethodAsyncAction, string?>(nameof(MethodName));

    /// <summary>
    /// Identifies the <seealso cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetObjectProperty =
        AvaloniaProperty.Register<CallMethodAsyncAction, object?>(nameof(TargetObject));

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

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the method is called; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(TargetObjectProperty) is not null ? TargetObject : sender;
        if (target is null || string.IsNullOrEmpty(MethodName))
        {
            return false;
        }

        MethodInfo? methodInfo = null;
        ParameterInfo[]? parameters = null;

        foreach (var method in target.GetType().GetRuntimeMethods())
        {
            if (string.Equals(method.Name, MethodName, StringComparison.Ordinal))
            {
                var p = method.GetParameters();
                if (p.Length == 0 || (p.Length == 2 && p[0].ParameterType == typeof(object)))
                {
                    methodInfo = method;
                    parameters = p;
                    break;
                }
            }
        }

        if (methodInfo is null)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                "Cannot find method named {0} on object of type {1} that matches the expected signature.", MethodName, target.GetType()));
        }

        object? result = null;
        if (parameters!.Length == 0)
        {
            result = methodInfo.Invoke(target, null);
        }
        else if (parameters.Length == 2)
        {
            result = methodInfo.Invoke(target, [sender!, parameter!]);
        }

        if (result is Task task)
        {
            _ = Dispatcher.UIThread.InvokeAsync(async () => await task);
        }

        return true;
    }
}
