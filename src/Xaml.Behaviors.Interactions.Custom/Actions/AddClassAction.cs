// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// Adds a specified <see cref="AddClassAction.ClassName"/> to the <see cref="StyledElement.Classes"/> collection when invoked. 
/// </summary>
public partial class AddClassAction : Avalonia.Xaml.Interactivity.StyledElementAction
{

    /// <summary>
    /// Gets or sets the class name that should be added. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string ClassName { get; set; }

    /// <summary>
    /// Gets or sets the target styled element that class name that should be added to. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial StyledElement? StyledElement { get; set; }

    /// <summary>
    /// Gets or sets the flag indicated whether to remove the class if already exists before adding. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial bool RemoveIfExists { get; set; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the class is successfully added; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(StyledElementProperty) is not null ? StyledElement : sender as StyledElement;
        if (target is null || string.IsNullOrEmpty(ClassName))
        {
            return false;
        }

        if (RemoveIfExists && target.Classes.Contains(ClassName))
        {
            target.Classes.Remove(ClassName);
        }

        target.Classes.Add(ClassName);

        return true;
    }
}
