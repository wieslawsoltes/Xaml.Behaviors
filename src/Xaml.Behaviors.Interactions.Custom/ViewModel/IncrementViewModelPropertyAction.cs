// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Increments a numeric view model property when invoked.
/// </summary>
public partial class IncrementViewModelPropertyAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the value to add. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = 1)]
    public partial double Delta { get; set; }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Reflection is used to reach view-model members provided by the application.")]
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not Control control)
        {
            return false;
        }

        var target = control.DataContext;
        if (target is null)
        {
            return false;
        }

        var propertyName = PropertyName;
        if (string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        var info = target.GetType().GetRuntimeProperty(propertyName);
        if (info is null || !info.CanWrite)
        {
            return false;
        }

        var current = info.GetValue(target);
        if (current is null)
        {
            return false;
        }

        double value;
        try
        {
            value = Convert.ToDouble(current, CultureInfo.InvariantCulture);
        }
        catch
        {
            return false;
        }

        value += Delta;
        object? result = Convert.ChangeType(value, info.PropertyType, CultureInfo.InvariantCulture);
        info.SetValue(target, result, null);
        return true;
    }
}
