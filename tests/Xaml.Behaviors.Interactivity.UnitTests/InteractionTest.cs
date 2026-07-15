using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class InteractionTest
{
    private sealed class LoadedTrigger : StyledElementTrigger<Button>
    {
        public int InitializedCount { get; private set; }
        public int LogicalAttachCount { get; private set; }
        public int VisualAttachCount { get; private set; }
        public int LoadedCount { get; private set; }

        protected override void OnInitializedEvent()
        {
            InitializedCount++;
        }

        protected override void OnAttachedToLogicalTree()
        {
            LogicalAttachCount++;
        }

        protected override void OnAttachedToVisualTree()
        {
            VisualAttachCount++;
        }

        protected override void OnLoaded()
        {
            LoadedCount++;
        }
    }

    private sealed class TopLevelLoadedTrigger : StyledElementTrigger<TopLevel>
    {
        public int InitializedCount { get; private set; }
        public int LogicalAttachCount { get; private set; }
        public int VisualAttachCount { get; private set; }
        public int LoadedCount { get; private set; }

        protected override void OnInitializedEvent()
        {
            InitializedCount++;
        }

        protected override void OnAttachedToLogicalTree()
        {
            LogicalAttachCount++;
        }

        protected override void OnAttachedToVisualTree()
        {
            VisualAttachCount++;
        }

        protected override void OnLoaded()
        {
            LoadedCount++;
        }
    }

    private sealed class SiblingAwareLoadedTrigger(IBehavior sibling) : StyledElementTrigger<Button>
    {
        public bool SiblingWasAttachedWhenLoaded { get; private set; }

        protected override void OnLoaded()
        {
            SiblingWasAttachedWhenLoaded = ReferenceEquals(AssociatedObject, sibling.AssociatedObject);
        }
    }

    private sealed class SelfRemovingLoadedTrigger : StyledElementTrigger<Button>
    {
        public int LoadedCount { get; private set; }

        protected override void OnLoaded()
        {
            LoadedCount++;
            if (AssociatedObject is not null)
            {
                Interaction.GetBehaviors(AssociatedObject).Remove(this);
            }
        }
    }

    private static void AssertCurrentLifecycle(LoadedTrigger trigger, Button button)
    {
        Assert.Equal(1, trigger.InitializedCount);
        Assert.Equal(1, trigger.LogicalAttachCount);
        Assert.Equal(1, trigger.VisualAttachCount);
        Assert.Equal(1, trigger.LoadedCount);
        Assert.Same(button, trigger.Parent);
    }

    [AvaloniaFact]
    public void SetBehaviors_MultipleBehaviors_AllAttached()
    {
        var behaviorCollection = new BehaviorCollection
        {
            new StubBehavior(),
            new StubBehavior(),
            new StubBehavior()
        };

        var button = new Button();
        Interaction.SetBehaviors(button, behaviorCollection);

        foreach (StubBehavior behavior in behaviorCollection)
        {
            Assert.Equal(1, behavior.AttachCount); // "Should only have called Attach once."
            Assert.Equal(0, behavior.DetachCount); // "Should not have called Detach."
            Assert.Equal(button, behavior.AssociatedObject); // "Should be attached to the host of the BehaviorCollection."
        }
    }

    [AvaloniaFact]
    public void SetBehaviors_MultipleSets_DoesNotReattach()
    {
        var behaviorCollection = new BehaviorCollection() { new StubBehavior() };

        var button = new Button();
        Interaction.SetBehaviors(button, behaviorCollection);
        Interaction.SetBehaviors(button, behaviorCollection);

        foreach (StubBehavior behavior in behaviorCollection)
        {
            Assert.Equal(1, behavior.AttachCount); // "Should only have called Attach once."
        }
    }

    [AvaloniaFact]
    public void SetBehaviors_CollectionThenNull_DeatchCollection()
    {
        var behaviorCollection = new BehaviorCollection() { new StubBehavior() };

        var button = new Button();
        Interaction.SetBehaviors(button, behaviorCollection);
        Interaction.SetBehaviors(button, null);

        foreach (StubBehavior behavior in behaviorCollection)
        {
            Assert.Equal(1, behavior.DetachCount); // "Should only have called Detach once."
            Assert.Null(behavior.AssociatedObject); // "AssociatedObject should be null after Detach."
        }
    }

    [AvaloniaFact]
    public void SetBehaviors_NullThenNull_NoOp()
    {
        // As long as this doesn't crash/assert, we're good.

        var button = new Button();
        Interaction.SetBehaviors(button, null);
        Interaction.SetBehaviors(button, null);
        Interaction.SetBehaviors(button, null);
    }

    [AvaloniaFact]
    public void SetBehaviors_ManualDetachThenNull_DoesNotDoubleDetach()
    {
        var behaviorCollection = new BehaviorCollection
        {
            new StubBehavior(),
            new StubBehavior(),
            new StubBehavior()
        };

        var button = new Button();
        Interaction.SetBehaviors(button, behaviorCollection);

        foreach (StubBehavior behavior in behaviorCollection)
        {
            behavior.Detach();
        }

        Interaction.SetBehaviors(button, null);

        foreach (StubBehavior behavior in behaviorCollection)
        {
            Assert.Equal(1, behavior.DetachCount); // "Setting BehaviorCollection to null should not call Detach on already Detached Behaviors."
            Assert.Null(behavior.AssociatedObject); // "AssociatedObject should be null after Detach."
        }
    }

    [AvaloniaFact]
    public void ExecuteActions_NullParameters_ReturnsEmptyEnumerable()
    {
        // Mostly just want to test that this doesn't throw any exceptions.
        var result = Interaction.ExecuteActions(null, null, null);

        Assert.NotNull(result);
        Assert.Empty(result); // "Calling ExecuteActions with a null ActionCollection should return an empty enumerable."
    }

    [AvaloniaFact]
    public void ExecuteActions_MultipleActions_AllActionsExecuted()
    {
        var actions = new ActionCollection
        {
            new StubAction(),
            new StubAction(),
            new StubAction()
        };

        var sender = new Button();
        var parameterString = "TestString";

        Interaction.ExecuteActions(sender, actions, parameterString);

        foreach (StubAction action in actions)
        {
            Assert.Equal(1, action.ExecuteCount); // "Each IAction should be executed once."
            Assert.Equal(sender, action.Sender); // "Sender is passed to the actions."
            Assert.Equal(parameterString, action.Parameter); // "Parameter is passed to the actions."
        }
    }

    [AvaloniaFact]
    public void ExecuteActions_ActionsWithResults_ResultsInActionOrder()
    {
        string[] expectedReturnValues = ["A", "B", "C"];

        var actions = new ActionCollection();

        foreach (var returnValue in expectedReturnValues)
        {
            actions.Add(new StubAction(returnValue));
        }

        var results = Interaction.ExecuteActions(null, actions, null).ToList();

        Assert.Equal(expectedReturnValues.Length, results.Count); // "Should have the same number of results as IActions."

        for (var resultIndex = 0; resultIndex < results.Count; resultIndex++)
        {
            Assert.Equal(expectedReturnValues[resultIndex], results[resultIndex]); // "Results should be returned in the order of the actions in the ActionCollection."
        }
    }

    [AvaloniaFact]
    public void SetBehaviorsBeforeShow_NotifiesLoadedOnce()
    {
        var trigger = new LoadedTrigger();
        var button = new Button();
        Interaction.SetBehaviors(button, new BehaviorCollection { trigger });
        var window = new Window { Content = button };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        AssertCurrentLifecycle(trigger, button);
    }

    [AvaloniaFact]
    public void SetBehaviorsAfterShow_NotifiesLoadedOnce()
    {
        var trigger = new LoadedTrigger();
        var button = new Button();
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        Interaction.SetBehaviors(button, new BehaviorCollection { trigger });

        AssertCurrentLifecycle(trigger, button);
    }

    [AvaloniaFact]
    public void SetBehaviorsAfterShow_AttachesAllSiblingsBeforeLoadedCallbacks()
    {
        var button = new Button();
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        var sibling = new StubBehavior();
        var trigger = new SiblingAwareLoadedTrigger(sibling);

        Interaction.SetBehaviors(button, new BehaviorCollection { trigger, sibling });

        Assert.True(trigger.SiblingWasAttachedWhenLoaded);
        Assert.Same(button, sibling.AssociatedObject);
        window.Close();
    }

    [AvaloniaFact]
    public void SetBehaviorsAfterShow_AllowsLifecycleCallbackToRemoveBehavior()
    {
        var button = new Button();
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        var trigger = new SelfRemovingLoadedTrigger();
        var sibling = new StubBehavior();
        var behaviors = new BehaviorCollection { trigger, sibling };

        Interaction.SetBehaviors(button, behaviors);

        Assert.Equal(1, trigger.LoadedCount);
        Assert.DoesNotContain(trigger, behaviors);
        Assert.Null(trigger.AssociatedObject);
        Assert.Same(button, sibling.AssociatedObject);
        window.Close();
    }

    [AvaloniaFact]
    public void AddBehaviorAfterShow_NotifiesLoadedOnce()
    {
        var button = new Button();
        var behaviors = Interaction.GetBehaviors(button);
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        var trigger = new LoadedTrigger();

        behaviors.Add(trigger);

        AssertCurrentLifecycle(trigger, button);
    }

    [AvaloniaFact]
    public void AddBehaviorAfterShow_AllowsBehaviorToRemoveItselfDuringLoaded()
    {
        var button = new Button();
        var behaviors = Interaction.GetBehaviors(button);
        var window = new Window { Content = button };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        var trigger = new SelfRemovingLoadedTrigger();

        behaviors.Add(trigger);

        Assert.Equal(1, trigger.LoadedCount);
        Assert.DoesNotContain(trigger, behaviors);
        Assert.Null(trigger.AssociatedObject);
        Assert.Empty(behaviors);
        window.Close();
    }

    [AvaloniaFact]
    public void AddBehaviorToOpenTopLevel_NotifiesCurrentLifecycleOnce()
    {
        var window = new Window();
        var behaviors = Interaction.GetBehaviors(window);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        var trigger = new TopLevelLoadedTrigger();

        behaviors.Add(trigger);

        Assert.Equal(1, trigger.InitializedCount);
        Assert.Equal(1, trigger.LogicalAttachCount);
        Assert.Equal(1, trigger.VisualAttachCount);
        Assert.Equal(1, trigger.LoadedCount);
        Assert.Same(window, trigger.Parent);

        window.Close();
    }
}
