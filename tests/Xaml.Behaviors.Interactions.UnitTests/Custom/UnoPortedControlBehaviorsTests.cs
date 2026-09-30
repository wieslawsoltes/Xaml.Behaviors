using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Automation;
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
#if !UNO
        // WinUI behaviors are dependency objects without a Name.
        Assert.Null(behavior.Name);
#endif

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

#if !UNO
        // WinUI dependency properties do not expose their owner type.
        Assert.Equal(typeof(ClearItemsControlAction), ClearItemsControlAction.ItemsControlProperty.OwnerType);
#endif
        Assert.Equal(true, action.Execute(null, null));
        Assert.Empty(items);
    }
}
