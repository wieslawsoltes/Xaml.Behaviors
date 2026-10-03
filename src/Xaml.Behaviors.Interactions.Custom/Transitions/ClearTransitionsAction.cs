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
/// Clears the <see cref="Avalonia.Animation.Transitions"/> collection.
/// </summary>
public partial class ClearTransitionsAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target styled element. This is an avalonia property.
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

        StyledElement? target = (StyledElement?)GetValue(StyledElementProperty) ?? sender as StyledElement;
        return TransitionOperations.Clear(target);
    }
}
