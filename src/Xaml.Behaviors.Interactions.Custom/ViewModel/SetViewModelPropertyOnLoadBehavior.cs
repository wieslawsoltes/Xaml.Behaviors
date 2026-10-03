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
/// Sets a view model property when the associated control is loaded.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public partial class SetViewModelPropertyOnLoadBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Gets or sets the property name to change. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the value to assign. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <inheritdoc />
    protected override void OnLoaded()
    {
        base.OnLoaded();

        var target = AssociatedObject?.DataContext;
        if (target is null)
        {
            return;
        }

        if (PropertyName is not { Length: > 0 } propertyName)
        {
            return;
        }

        PropertyHelper.UpdatePropertyValue(target, propertyName, Value);
    }
}
