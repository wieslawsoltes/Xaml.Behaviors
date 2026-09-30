// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Specialized;
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
/// Executes associated actions whenever the bound collection raises a <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
/// </summary>
public sealed partial class CollectionChangedTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets the collection to observe.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial INotifyCollectionChanged? Collection { get; set; }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, e);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (Collection is not null)
        {
            Collection.CollectionChanged += CollectionChanged;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (Collection is not null)
        {
            Collection.CollectionChanged -= CollectionChanged;
        }

        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CollectionProperty)
        {
            if (change.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= CollectionChanged;
            }
            if (change.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += CollectionChanged;
            }
        }
    }
}
