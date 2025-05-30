using System.Collections.Specialized;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Collections;

/// <summary>
/// Executes associated actions whenever the bound collection raises a <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
/// </summary>
public sealed class CollectionChangedTrigger : StyledElementTrigger
{
    /// <summary>
    /// Identifies the <see cref="Collection"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<INotifyCollectionChanged?> CollectionProperty =
        AvaloniaProperty.Register<CollectionChangedTrigger, INotifyCollectionChanged?>(nameof(Collection));

    /// <summary>
    /// Gets or sets the collection to observe.
    /// </summary>
    [ResolveByName]
    public INotifyCollectionChanged? Collection
    {
        get => GetValue(CollectionProperty);
        set => SetValue(CollectionProperty, value);
    }

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
        Collection?.CollectionChanged += CollectionChanged;
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        Collection?.CollectionChanged -= CollectionChanged;
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
