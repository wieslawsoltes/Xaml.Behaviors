// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
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
/// Sets a view model property to a specified value when invoked.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class SetViewModelPropertyAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the value to assign. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <inheritdoc />
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

        if (PropertyName is not { Length: > 0 } propertyName)
        {
            return false;
        }
        
        PropertyHelper.UpdatePropertyValue(target, propertyName, Value);
        return true;
    }
}
