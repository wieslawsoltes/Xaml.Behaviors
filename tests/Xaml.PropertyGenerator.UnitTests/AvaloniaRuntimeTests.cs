using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Xaml.PropertyGenerator.UnitTests;

public class AvaloniaRuntimeTests
{
    [AvaloniaFact]
    public void Styled_Defaults_Are_Registered()
    {
        var owner = new RuntimeOwner();

        Assert.True(owner.IsActive);
        Assert.Equal(RuntimeMode.Second, owner.Mode);
        Assert.Equal(TimeSpan.FromMilliseconds(500), owner.Delay);
        Assert.Null(owner.Text);
        Assert.Equal(BindingMode.TwoWay, RuntimeOwner.ModeProperty.GetMetadata(typeof(RuntimeOwner)).DefaultBindingMode);
    }

    [AvaloniaFact]
    public void Styled_Property_Uses_Value_Store()
    {
        var owner = new RuntimeOwner { IsActive = false };

        Assert.False(owner.GetValue(RuntimeOwner.IsActiveProperty));
        owner.SetValue(RuntimeOwner.IsActiveProperty, true);
        Assert.True(owner.IsActive);
    }

    [AvaloniaFact]
    public void Changed_Hook_Is_Invoked()
    {
        var owner = new RuntimeOwner();

        owner.Text = "a";
        owner.Text = "b";

        Assert.Equal([(null, "a"), ("a", "b")], owner.TextChanges);
    }

    [AvaloniaFact]
    public void Direct_Property_Raises_Changes_And_Keeps_Initializer()
    {
        var owner = new RuntimeOwner();
        var raised = 0;
        owner.PropertyChanged += (_, e) =>
        {
            if (e.Property == RuntimeOwner.CanExecuteProperty)
            {
                raised++;
            }
        };

        Assert.True(owner.CanExecute);
        owner.SetCanExecute(false);

        Assert.False(owner.CanExecute);
        Assert.False(owner.GetValue(RuntimeOwner.CanExecuteProperty));
        Assert.Equal(1, raised);
        Assert.True(RuntimeOwner.CanExecuteProperty.IsReadOnly);
        Assert.False(RuntimeOwner.CounterProperty.IsReadOnly);
    }

    [AvaloniaFact]
    public void Writable_Direct_Property_Accepts_Values_From_Property_System()
    {
        var owner = new RuntimeOwner();

        owner.SetValue(RuntimeOwner.CounterProperty, 42);

        Assert.Equal(42, owner.Counter);
    }

    [AvaloniaFact]
    public void Lazy_Direct_Property_Creates_Instance_Once()
    {
        var owner = new RuntimeOwner();

        var items = owner.Items;

        Assert.Same(items, owner.Items);
        Assert.Same(items, owner.GetValue(RuntimeOwner.ItemsProperty));
    }

    [AvaloniaFact]
    public void Attached_Property_Get_Set_And_Hook()
    {
        var owner = new RuntimeOwner();
        RuntimeAttachments.ChangeCount = 0;

        Assert.Equal("none", RuntimeAttachments.GetTag(owner));
        RuntimeAttachments.SetTag(owner, "x");

        Assert.Equal("x", RuntimeAttachments.GetTag(owner));
        Assert.Equal(1, RuntimeAttachments.ChangeCount);
    }
}
