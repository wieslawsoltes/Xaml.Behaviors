// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Triggers actions when the <see cref="AutoCompleteBox"/> selection changes.
/// </summary>
/// <remarks>
/// On Uno Platform the trigger handles <c>AutoSuggestBox.SuggestionChosen</c> of the associated WinUI
/// <c>AutoSuggestBox</c> (a CLR event: the routing strategy does not apply).
/// </remarks>
#if UNO
public partial class AutoCompleteBoxSelectionChangedTrigger : RoutedEventTriggerBase
#else
public partial class AutoCompleteBoxSelectionChangedTrigger : RoutedEventTriggerBase<SelectionChangedEventArgs>
#endif
{
#if !UNO
    /// <inheritdoc />
    protected override RoutedEvent<SelectionChangedEventArgs> RoutedEvent
        => AutoCompleteBox.SelectionChangedEvent;
#endif

    static AutoCompleteBoxSelectionChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<AutoCompleteBoxSelectionChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(RoutingStrategies.Bubble));
    }
}
