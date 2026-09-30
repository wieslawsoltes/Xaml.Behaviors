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
/// Toggles a specified <see cref="ToggleClassAction.ClassName"/> in the <see cref="StyledElement.Classes"/> collection when invoked.
/// </summary>
public partial class ToggleClassAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the class name that should be toggled. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the target styled element that class name that should be toggled on. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial StyledElement? StyledElement { get; set; }

    /// <inheritdoc />
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

        if (target.Classes.Contains(ClassName!))
        {
            target.Classes.Remove(ClassName!);
        }
        else
        {
            target.Classes.Add(ClassName!);
        }

        return true;
    }
}
