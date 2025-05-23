using System;
using System.Collections.Specialized;
using Avalonia.Collections;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Represents a collection of <see cref="IAction"/>'s.
/// </summary>
public class ActionCollection : AvaloniaList<AvaloniaObject>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCollection"/> class.
    /// </summary>
    public ActionCollection()
    {
        CollectionChanged += ActionCollection_CollectionChanged;
    }

    private void ActionCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        var collectionChangedAction = eventArgs.Action;

        switch (collectionChangedAction)
        {
            case NotifyCollectionChangedAction.Reset:
            {
                foreach (var item in this)
                {
                    VerifyType(item);
                }

                break;
            }
            case NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace:
            {
                var changedItem = eventArgs.NewItems?[0] as AvaloniaObject;
                VerifyType(changedItem);
                break;
            }
        }
    }

    private static void VerifyType(AvaloniaObject? item)
    {
        if (item is null)
        {
            return;
        }

        if (item is not IAction)
        {
            throw new InvalidOperationException(
                $"Only {nameof(IAction)} types are supported in an {nameof(ActionCollection)}.");
        }
    }
}
