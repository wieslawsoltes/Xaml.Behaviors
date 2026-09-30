// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="AutoSuggestBox.SuggestionChosen"/> subscription of <see cref="AutoCompleteBoxSelectionChangedTrigger"/>.
/// </content>
public partial class AutoCompleteBoxSelectionChangedTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not AutoSuggestBox autoSuggestBox)
        {
            return DisposableAction.Empty;
        }

        autoSuggestBox.SuggestionChosen += OnSuggestionChosen;
        return DisposableAction.Create(() => autoSuggestBox.SuggestionChosen -= OnSuggestionChosen);
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
