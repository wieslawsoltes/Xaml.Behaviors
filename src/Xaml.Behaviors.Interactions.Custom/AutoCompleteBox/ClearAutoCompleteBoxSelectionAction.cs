// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using AutoCompleteBox = Microsoft.UI.Xaml.Controls.AutoSuggestBox;
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
/// Clears the selection and text of an <see cref="AutoCompleteBox"/>.
/// </summary>
public partial class ClearAutoCompleteBoxSelectionAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the target <see cref="AutoCompleteBox"/>. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial AutoCompleteBox? AutoCompleteBox { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var box = AutoCompleteBox ?? sender as AutoCompleteBox;
        if (box is null)
        {
            return false;
        }

#if UNO
        // AutoSuggestBox has no selected item: the chosen suggestion only lives in the text.
        box.IsSuggestionListOpen = false;
#else
        box.SelectedItem = null;
#endif
        box.Text = string.Empty;

        return null;
    }
}
