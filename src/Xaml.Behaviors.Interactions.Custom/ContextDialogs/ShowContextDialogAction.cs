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
/// Opens a <see cref="ContextDialogBehavior"/> when executed.
/// </summary>
public partial class ShowContextDialogAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target dialog behavior. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ContextDialogBehavior? TargetDialog { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var dialog = TargetDialog;
        if (dialog is null)
        {
            return false;
        }

        dialog.SetCurrentValue(ContextDialogBehavior.IsOpenProperty, true);
        return true;
    }
}
