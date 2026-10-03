using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class CollectionChangedTests
{
    [AvaloniaFact]
    public void CollectionChangedTrigger_Collection_Set_Before_Attach_Executes_Once_Per_Change()
    {
        var items = new ObservableCollection<int>();
        var action = new CountingAction();
        var trigger = new CollectionChangedTrigger { Collection = items };
        trigger.Actions.Add(action);

        items.Add(0);
        Assert.Equal(0, action.ExecutionCount);

        trigger.Attach(new Border());

        items.Add(1);
        Assert.Equal(1, action.ExecutionCount);

        trigger.Detach();

        items.Add(2);
        Assert.Equal(1, action.ExecutionCount);
    }

    [AvaloniaFact]
    public void CollectionChangedTrigger_Collection_Changed_While_Attached_Observes_The_New_Collection()
    {
        var oldItems = new ObservableCollection<int>();
        var newItems = new ObservableCollection<int>();
        var action = new CountingAction();
        var trigger = new CollectionChangedTrigger();
        trigger.Actions.Add(action);
        trigger.Attach(new Border());

        trigger.Collection = oldItems;
        oldItems.Add(1);
        Assert.Equal(1, action.ExecutionCount);

        trigger.Collection = newItems;
        oldItems.Add(2);
        newItems.Add(3);
        Assert.Equal(2, action.ExecutionCount);

        trigger.Detach();
        trigger.Collection = oldItems;

        oldItems.Add(4);
        newItems.Add(5);
        Assert.Equal(2, action.ExecutionCount);
    }

    [AvaloniaFact]
    public void CollectionChangedBehavior_Collection_Set_Before_Attach_Executes_Once_Per_Change()
    {
        var items = new ObservableCollection<int>();
        var added = new CountingAction();
        var removed = new CountingAction();
        var behavior = new CollectionChangedBehavior { Collection = items };
        behavior.AddedActions.Add(added);
        behavior.RemovedActions.Add(removed);

        items.Add(0);
        Assert.Equal(0, added.ExecutionCount);

        behavior.Attach(new Border());

        items.Add(1);
        items.Remove(1);
        Assert.Equal(1, added.ExecutionCount);
        Assert.Equal(1, removed.ExecutionCount);

        behavior.Detach();

        items.Add(2);
        items.Remove(2);
        Assert.Equal(1, added.ExecutionCount);
        Assert.Equal(1, removed.ExecutionCount);
    }

    [AvaloniaFact]
    public void CollectionChangedBehavior_Collection_Changed_While_Attached_Observes_The_New_Collection()
    {
        var oldItems = new ObservableCollection<int>();
        var newItems = new ObservableCollection<int>();
        var added = new CountingAction();
        var behavior = new CollectionChangedBehavior();
        behavior.AddedActions.Add(added);
        behavior.Attach(new Border());

        behavior.Collection = oldItems;
        oldItems.Add(1);
        Assert.Equal(1, added.ExecutionCount);

        behavior.Collection = newItems;
        oldItems.Add(2);
        newItems.Add(3);
        Assert.Equal(2, added.ExecutionCount);

        behavior.Detach();
        behavior.Collection = oldItems;

        oldItems.Add(4);
        newItems.Add(5);
        Assert.Equal(2, added.ExecutionCount);
    }

    private sealed class CountingAction : StyledElementAction
    {
        public int ExecutionCount { get; private set; }

        public override object Execute(object? sender, object? parameter)
        {
            ExecutionCount++;
            return true;
        }
    }
}
