// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that calls an asynchronous method on a specified object when invoked.
/// </summary>
public partial class CallMethodAsyncAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the name of the method to invoke. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the object that exposes the method of interest. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial object? TargetObject { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the method is called; else false.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Reflection is used to invoke view-model members provided by the application.")]
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
