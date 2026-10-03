using System;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactivity.UnitTests;
#else
namespace Avalonia.Xaml.Interactivity.UnitTests;
#endif

#if UNO
public partial class ConditionTests
#else
public class ConditionTests
#endif
{
    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests intentionally exercise Avalonia Binding which uses reflection and is trimmer-unfriendly.")]
    public void Binding_Updates_BindingValue()
    {
        var source = new BindingSource { Value = "Initial" };
#if UNO
        // WinUI applies a binding to the Binding property (see Condition), BindingValue is the bound value.
        var condition = new Condition();
        BindingOperations.SetBinding(condition, Condition.BindingProperty, new Binding
        {
            Path = new PropertyPath(nameof(BindingSource.Value)),
            Source = source
        });
#else
        var condition = new Condition
        {
            Binding = new Binding
            {
                Path = nameof(BindingSource.Value),
                Source = source
            }
        };
#endif

        Assert.Equal("Initial", condition.BindingValue);

        source.Value = "Updated";

        Assert.Equal("Updated", condition.BindingValue);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests intentionally exercise Avalonia Binding which uses reflection and is trimmer-unfriendly.")]
    public void Setting_Property_After_Binding_Throws()
    {
        var condition = new Condition
        {
            Binding = new Binding
            {
#if UNO
                Path = new PropertyPath(nameof(BindingSource.Value)),
#else
                Path = nameof(BindingSource.Value),
#endif
                Source = new BindingSource()
            }
        };

        Assert.Throws<InvalidOperationException>(() => condition.Property = TextBlock.TextProperty);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests intentionally exercise Avalonia Binding which uses reflection and is trimmer-unfriendly.")]
    public void Setting_Binding_After_Property_Throws()
    {
        var condition = new Condition
        {
            Property = TextBlock.TextProperty
        };

        Assert.Throws<InvalidOperationException>(() =>
            condition.Binding = new Binding
            {
#if UNO
                Path = new PropertyPath(nameof(BindingSource.Value))
#else
                Path = nameof(BindingSource.Value)
#endif
            });
    }

#if WINUI
    // A native WinUI binding does not observe the dependency properties of a source type that is not in the XAML
    // type information of the application (a type that is not used in XAML): the source notifies its changes.
    private sealed class BindingSource : System.ComponentModel.INotifyPropertyChanged
    {
        private string? _value;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }
}
#else
#if UNO
    private partial class BindingSource : AvaloniaObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(BindingSource), new PropertyMetadata(null));
#else
    private class BindingSource : AvaloniaObject
    {
        public static readonly StyledProperty<string?> ValueProperty =
            AvaloniaProperty.Register<BindingSource, string?>(nameof(Value));
#endif

        public string? Value
        {
            get => (string?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
#endif

public class ConditionCollectionTests
{
    [AvaloniaFact]
    public void Can_Add_Conditions()
    {
        var collection = new ConditionCollection();
        var condition = new Condition();

        collection.Add(condition);

        Assert.Single(collection);
        Assert.Same(condition, collection[0]);
    }

    [AvaloniaFact]
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
