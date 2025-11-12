using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class InteractionExtensionMembersTests
{
    [Fact]
    public void BehaviorsExtensionProperty_ReturnsSameCollectionAsAttachedProperty()
    {
        var button = new Button();

        var behaviors = button.Behaviors;

        Assert.Same(behaviors, button.GetValue(Interaction.BehaviorsProperty));
    }

    [Fact]
    public void BehaviorsExtensionProperty_SetterReplacesCollection()
    {
        var button = new Button();
        var collection = new BehaviorCollection { new StubBehavior() };

        button.Behaviors = collection;

        Assert.Same(collection, Interaction.GetBehaviors(button));
    }
}
