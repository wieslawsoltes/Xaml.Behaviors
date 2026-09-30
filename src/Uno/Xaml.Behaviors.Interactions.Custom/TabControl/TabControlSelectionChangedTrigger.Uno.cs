// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="TabView.SelectionChanged"/> subscription of <see cref="TabControlSelectionChangedTrigger"/>.
/// </content>
public partial class TabControlSelectionChangedTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not TabView tabView)
        {
            return DisposableAction.Empty;
        }

        tabView.SelectionChanged += OnSelectionChanged;
        return DisposableAction.Create(() => tabView.SelectionChanged -= OnSelectionChanged);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }
}
