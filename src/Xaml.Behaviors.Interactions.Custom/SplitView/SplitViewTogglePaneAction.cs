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
/// Toggles the <see cref="SplitView.IsPaneOpen"/> state of a <see cref="SplitView"/> when executed.
/// </summary>
public partial class SplitViewTogglePaneAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target <see cref="SplitView"/>. If not set, the sender is used.
    /// This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial SplitView? TargetSplitView { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var splitView = GetValue(TargetSplitViewProperty) is not null ? TargetSplitView : sender as SplitView;
        if (splitView is null)
        {
            return false;
        }

        splitView.IsPaneOpen = !splitView.IsPaneOpen;
        return true;
    }
}
