using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class ActionTests
{
    private class TestAction(object? returnValue) : Action
    {
        public TestAction() : this(null)
        {
        }

        public object? Sender { get; private set; }
        public object? Parameter { get; private set; }
        public int ExecuteCount { get; private set; }

        public override object? Execute(object? sender, object? parameter)
        {
            ExecuteCount++;
            Sender = sender;
            Parameter = parameter;
            return returnValue;
        }
    }

    [AvaloniaFact]
    public void IsEnabled_Default_True()
    {
        var action = new TestAction();
        Assert.True(action.IsEnabled);
    }

    [AvaloniaFact]
    public void IsEnabled_SetFalse()
    {
        var action = new TestAction { IsEnabled = false };
        Assert.False(action.IsEnabled);
    }

    [AvaloniaFact]
    public void Execute_SetsPropertiesAndReturnsValue()
    {
        var action = new TestAction("result");
        var sender = new object();
        var parameter = new object();

        var result = action.Execute(sender, parameter);

        Assert.Equal(1, action.ExecuteCount);
        Assert.Equal(sender, action.Sender);
        Assert.Equal(parameter, action.Parameter);
        Assert.Equal("result", result);
    }

    [AvaloniaFact]
    public void ActionCollection_AddNonAction_Throws()
    {
        var collection = new ActionCollection();
        collection.Add(new TestAction());

        Assert.Throws<InvalidOperationException>(() => collection.Add(new Button()));
    }

    [AvaloniaFact]
    public void ActionCollection_ReplaceWithNonAction_Throws()
    {
        var collection = new ActionCollection { new TestAction() };

        Assert.Throws<InvalidOperationException>(() => collection[0] = new Button());
    }

    [AvaloniaFact]
    public void ExecuteActions_WithPlainActions_AllExecuted()
    {
        var actions = new ActionCollection
        {
            new TestAction(),
            new TestAction(),
            new TestAction()
        };

        var sender = new object();
        var parameter = "param";

        Interaction.ExecuteActions(sender, actions, parameter);

        foreach (TestAction action in actions)
        {
            Assert.Equal(1, action.ExecuteCount);
            Assert.Equal(sender, action.Sender);
            Assert.Equal(parameter, action.Parameter);
        }
    }

    [AvaloniaFact]
    public void ExecuteActions_ReturnValues_InOrder()
    {
        string[] expected = ["A", "B", "C"];
        var actions = new ActionCollection();

        foreach (var value in expected)
        {
            actions.Add(new TestAction(value));
        }

        var results = Interaction.ExecuteActions(null, actions, null).ToList();

        Assert.Equal(expected.Length, results.Count);
        for (int i = 0; i < results.Count; i++)
        {
            Assert.Equal(expected[i], results[i]);
        }
    }
}
