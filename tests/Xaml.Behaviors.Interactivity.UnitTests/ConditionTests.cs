using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

public class ConditionTests
{
    [AvaloniaFact]
    public void Binding_Updates_BindingValue()
    {
        var source = new BindingSource { Value = "Initial" };
        var condition = new Condition
        {
            Binding = new Binding(nameof(BindingSource.Value))
            {
                Source = source
            }
        };

        Assert.Equal("Initial", condition.BindingValue);

        source.Value = "Updated";

        Assert.Equal("Updated", condition.BindingValue);
    }

    [AvaloniaFact]
    public void Setting_Property_After_Binding_Throws()
    {
        var condition = new Condition
        {
            Binding = new Binding(nameof(BindingSource.Value))
            {
                Source = new BindingSource()
            }
        };

        Assert.Throws<InvalidOperationException>(() => condition.Property = TextBlock.TextProperty);
    }

    [AvaloniaFact]
    public void Setting_Binding_After_Property_Throws()
    {
        var condition = new Condition
        {
            Property = TextBlock.TextProperty
        };

        Assert.Throws<InvalidOperationException>(() =>
            condition.Binding = new Binding(nameof(BindingSource.Value)));
    }

    private class BindingSource : AvaloniaObject
    {
        public static readonly StyledProperty<string?> ValueProperty =
            AvaloniaProperty.Register<BindingSource, string?>(nameof(Value));

        public string? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}

public class ConditionCollectionTests
{
    [Fact]
    public void Can_Add_Conditions()
    {
        var collection = new ConditionCollection();
        var condition = new Condition();

        collection.Add(condition);

        Assert.Single(collection);
        Assert.Same(condition, collection[0]);
    }

    [Fact]
    public void Clear_Removes_All_Items()
    {
        var collection = new ConditionCollection
        {
            new Condition(),
            new Condition()
        };

        collection.Clear();

        Assert.Empty(collection);
    }
}
