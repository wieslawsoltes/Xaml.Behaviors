// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Shows a <see cref="Window"/> as a dialog.
/// </summary>
/// <remarks>
/// On Uno Platform the dialog is a <c>ContentDialog</c> shown in the <c>XamlRoot</c> of the <see cref="Owner"/>
/// window or of the sender. Content dialogs are always modal.
/// </remarks>
public partial class ShowDialogAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the dialog window instance. This is an avalonia property.
    /// </summary>
#if UNO
    [StyledProperty(ResolveByName = true)]
    public partial ContentDialog? Dialog { get; set; }
#else
    [StyledProperty(ResolveByName = true)]
    public partial Window? Dialog { get; set; }
#endif

    /// <summary>
    /// Gets or sets the owner window for the dialog. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Window? Owner { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var dialog = Dialog;
        if (dialog is null)
        {
            return false;
        }

#if UNO
        dialog.XamlRoot ??= Owner?.Content?.XamlRoot ?? (sender as Visual)?.XamlRoot;
        if (dialog.XamlRoot is null)
        {
            return false;
        }

        _ = dialog.ShowAsync();
#else
        var owner = Owner ?? TopLevel.GetTopLevel(sender as Visual) as Window;
        if (owner is not null)
        {
            dialog.ShowDialog(owner);
        }
        else
        {
            dialog.Show();
        }
#endif

        return true;
    }
}
