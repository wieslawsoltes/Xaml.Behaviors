using System.Collections.ObjectModel;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

/// <summary>
/// Avalonia regression tests for control behaviors whose properties were migrated to generated properties during
/// the Uno Platform port.
/// </summary>
public class UnoPortedControlBehaviorsTests
{
    [AvaloniaFact]
    public void AutomationNameBehavior_Sets_And_Clears_The_Automation_Name()
    {
        var button = new Button { Name = "button" };
        var behavior = new AutomationNameBehavior { AutomationName = "name" };

        Interaction.GetBehaviors(button).Add(behavior);
        Assert.Equal("name", AutomationProperties.GetName(button));
        Assert.Equal("name", behavior.AutomationName);
        Assert.Null(behavior.Name);

        behavior.AutomationName = "other";
        Assert.Equal("other", AutomationProperties.GetName(button));

        Interaction.GetBehaviors(button).Remove(behavior);
        Assert.Null(AutomationProperties.GetName(button));
    }

    [AvaloniaFact]
    public void ClearItemsControlAction_Owns_Its_ItemsControl_Property()
    {
        var items = new ObservableCollection<string> { "a", "b" };
        var itemsControl = new ItemsControl { ItemsSource = items };
        var action = new ClearItemsControlAction { ItemsControl = itemsControl };

        Assert.Equal(typeof(ClearItemsControlAction), ClearItemsControlAction.ItemsControlProperty.OwnerType);
        Assert.Equal(true, action.Execute(null, null));
        Assert.Empty(items);
    }
}
