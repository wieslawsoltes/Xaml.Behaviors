// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI <see cref="Selector.SelectionChanged"/> subscription of <see cref="CarouselSelectionChangedTrigger"/>.
/// </content>
public partial class CarouselSelectionChangedTrigger
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is not Selector selector)
        {
            return DisposableAction.Empty;
        }

        selector.SelectionChanged += OnSelectionChanged;
        return DisposableAction.Create(() => selector.SelectionChanged -= OnSelectionChanged);
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
