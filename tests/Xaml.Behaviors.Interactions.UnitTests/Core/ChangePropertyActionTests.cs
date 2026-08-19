using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class ChangePropertyActionTests
{
    private sealed class Grid
    {
    }

    private sealed class AttachedPropertyOwner : AvaloniaObject
    {
        public static readonly AttachedProperty<int> ValueProperty =
            AvaloniaProperty.RegisterAttached<AttachedPropertyOwner, Border, int>("Value");

        static AttachedPropertyOwner()
        {
        }
    }

    private sealed class SomeOwner
    {
    }

    private sealed class ThrowingReversibleAction : StyledElementAction, IReversibleAction
    {
        public override object Execute(object? sender, object? parameter)
        {
            throw new InvalidOperationException("Apply failed.");
        }

        public object? ExecuteReversibly(object? sender, object? parameter)
        {
            throw new InvalidOperationException("Apply failed.");
        }

        public object? Revert(object? sender, object? parameter)
        {
            return true;
        }
    }

    /// <summary>
    /// Regular property.
    /// </summary>
    [AvaloniaFact]
    public void ChangePropertyAction_001()
    {
        var window = new ChangePropertyAction001();

        window.Show();
        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_001_0.png");

        // Click
        window.TargetButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_001_1.png");

        Assert.Equal("Updated Text", window.TargetTextBox.Text);
    }

    /// <summary>
    /// Attached property.
    /// </summary>
    [AvaloniaFact]
    public void ChangePropertyAction_002()
    {
        var window = new ChangePropertyAction002();

        window.Show();
        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_002_0.png");

        // Click
        window.TargetButton.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        window.CaptureRenderedFrame()?.Save("ChangePropertyAction_002_1.png");

        Assert.Equal(12d, window.TargetTextBox.FontSize);
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_UpdatesAttachedPropertyWhenOwnerTypeNameCollides()
    {
        _ = Avalonia.Controls.Grid.ColumnProperty;
        var target = new Border();
        var action = new ChangePropertyAction
        {
            PropertyName = "(Grid.Column)",
            Value = 2,
        };

        var result = action.Execute(target, null);

        Assert.Equal(true, result);
        Assert.Equal(2, Avalonia.Controls.Grid.GetColumn(target));
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_FindsAttachedPropertyRegisteredForTargetType()
    {
        var target = new Border();
        var action = new ChangePropertyAction
        {
            PropertyName = "(AttachedPropertyOwner.Value)",
            Value = 42,
        };

        var result = action.Execute(target, null);

        Assert.Equal(true, result);
        Assert.Equal(42, target.GetValue(AttachedPropertyOwner.ValueProperty));
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_DoesNotMatchInheritedPropertyFromWrongOwner()
    {
        var originalDataContext = new object();
        var target = new Border { DataContext = originalDataContext };
        var action = new ChangePropertyAction
        {
            PropertyName = "(SomeOwner.DataContext)",
            Value = new object(),
        };

        var result = action.Execute(target, null);

        Assert.Equal(false, result);
        Assert.Same(originalDataContext, target.DataContext);
    }

    [AvaloniaFact]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility action.")]
    public void ChangePropertyAction_MatchesInheritedPropertyFromRequestedOwner()
    {
        var updatedDataContext = new object();
        var target = new Border();
        var action = new ChangePropertyAction
        {
            PropertyName = "(StyledElement.DataContext)",
            Value = updatedDataContext,
        };

        var result = action.Execute(target, null);

        Assert.Equal(true, result);
        Assert.Same(updatedDataContext, target.DataContext);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Revert_Preserves_Styled_Property_Source()
    {
        var target = new TextBlock();
        target.Classes.Add("first");

        var window = new Window { Content = target };
        window.Styles.Add(CreateTextStyle("first", "First"));
        window.Styles.Add(CreateTextStyle("second", "Second"));
        window.Show();

        var action = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Applied"
        };

        Assert.Equal("First", target.Text);
        Assert.True((bool)((IReversibleAction)action).ExecuteReversibly(target, null)!);
        Assert.Equal("Applied", target.Text);
        Assert.True((bool)action.Revert(target, null));
        Assert.Equal("First", target.Text);

        target.Classes.Remove("first");
        target.Classes.Add("second");

        Assert.Equal("Second", target.Text);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Revert_Restores_Latest_Styled_Property_Source_Value()
    {
        var target = new TextBlock();
        target.Classes.Add("first");

        var window = new Window { Content = target };
        window.Styles.Add(CreateTextStyle("first", "First"));
        window.Styles.Add(CreateTextStyle("second", "Second"));
        window.Show();
        var action = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Applied"
        };

        Assert.True((bool)((IReversibleAction)action).ExecuteReversibly(target, null)!);
        target.Classes.Remove("first");
        target.Classes.Add("second");

        Assert.Equal("Applied", target.Text);
        Assert.True((bool)action.Revert(target, null));
        Assert.Equal("Second", target.Text);
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void ExecuteReversibly_Does_Not_Apply_To_Direct_Avalonia_Property()
    {
        var source = new StyledPropertySource { Value = "Original" };
        var target = new DirectPropertyTarget();
        using var binding = target.Bind(
            DirectPropertyTarget.ValueProperty,
            source.GetObservable(StyledPropertySource.ValueProperty));
        var action = new ChangePropertyAction
        {
            TargetObject = target,
            PropertyName = nameof(DirectPropertyTarget.Value),
            Value = "Applied"
        };

        Assert.False((bool)((IReversibleAction)action).ExecuteReversibly(null, null)!);
        Assert.Equal("Original", target.Value);
        source.Value = "Latest";
        Assert.Equal("Latest", target.Value);
        Assert.False((bool)action.Revert(null, null));
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Execute_Preserves_Legacy_Local_Value_Semantics()
    {
        var target = new TextBlock();
        target.Classes.Add("first");

        var window = new Window { Content = target };
        window.Styles.Add(CreateTextStyle("first", "First"));
        window.Styles.Add(CreateTextStyle("second", "Second"));
        window.Show();

        var action = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Applied"
        };

        Assert.True((bool)action.Execute(target, null));
        target.Classes.Remove("first");
        target.Classes.Add("second");

        Assert.Equal("Applied", target.Text);
        Assert.False((bool)action.Revert(target, null));
        window.Close();
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Execute_Updates_WriteOnly_Clr_Property()
    {
        var target = new WriteOnlyTarget();
        var action = new ChangePropertyAction
        {
            TargetObject = target,
            PropertyName = nameof(WriteOnlyTarget.Value),
            Value = "Updated"
        };

        Assert.True((bool)action.Execute(null, null));
        Assert.Equal("Updated", target.WrittenValue);
        Assert.False((bool)action.Revert(null, null));
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Revert_Preserves_Remaining_Active_Property_Action()
    {
        var target = new TextBlock { Text = "Original" };
        var first = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "First"
        };
        var second = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Second"
        };

        Assert.True((bool)((IReversibleAction)first).ExecuteReversibly(target, null)!);
        Assert.True((bool)((IReversibleAction)second).ExecuteReversibly(target, null)!);
        Assert.Equal("Second", target.Text);

        Assert.True((bool)first.Revert(target, null));
        Assert.Equal("Second", target.Text);
        Assert.True((bool)second.Revert(target, null));
        Assert.Equal("Original", target.Text);

        Assert.True((bool)((IReversibleAction)first).ExecuteReversibly(target, null)!);
        Assert.True((bool)((IReversibleAction)second).ExecuteReversibly(target, null)!);
        first.Value = "UpdatedFirst";
        Assert.True((bool)((IReversibleAction)first).ExecuteReversibly(target, null)!);
        Assert.Equal("Second", target.Text);
        Assert.True((bool)second.Revert(target, null));
        Assert.Equal("UpdatedFirst", target.Text);
        Assert.True((bool)first.Revert(target, null));
        Assert.Equal("Original", target.Text);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Revert_Preserves_Remaining_Active_Clr_Property_Action()
    {
        var target = new ReadWriteTarget { Value = "Original" };
        var first = new ChangePropertyAction
        {
            TargetObject = target,
            PropertyName = nameof(ReadWriteTarget.Value),
            Value = "First"
        };
        var second = new ChangePropertyAction
        {
            TargetObject = target,
            PropertyName = nameof(ReadWriteTarget.Value),
            Value = "Second"
        };

        Assert.True((bool)((IReversibleAction)first).ExecuteReversibly(null, null)!);
        Assert.True((bool)((IReversibleAction)second).ExecuteReversibly(null, null)!);
        Assert.True((bool)first.Revert(null, null));
        Assert.Equal("Second", target.Value);
        Assert.True((bool)second.Revert(null, null));
        Assert.Equal("Original", target.Value);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void Revert_Coordinates_Reflection_And_Typed_Property_Actions()
    {
        var target = new TextBlock { Text = "Original" };
        var reflectionAction = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Reflection"
        };
        var typedChange = new ReversiblePropertyChange<TextBlock, string?>(nameof(TextBlock.Text));

        Assert.True((bool)((IReversibleAction)reflectionAction).ExecuteReversibly(target, null)!);
        Assert.True(typedChange.Apply(
            target,
            "Typed",
            static item => item.Text,
            static (item, value) => item.Text = value));
        Assert.Equal("Typed", target.Text);

        Assert.True((bool)reflectionAction.Revert(target, null));
        Assert.Equal("Typed", target.Text);
        Assert.True(typedChange.Revert(static (item, value) => item.Text = value));
        Assert.Equal("Original", target.Text);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Test intentionally exercises reflection-based property lookup.")]
    public void ReversibleActionExecution_Unwinds_Applied_Actions_When_Later_Action_Throws()
    {
        var target = new TextBlock { Text = "Original" };
        var propertyAction = new ChangePropertyAction
        {
            PropertyName = nameof(TextBlock.Text),
            Value = "Applied"
        };
        var actions = new ActionCollection
        {
            propertyAction,
            new ThrowingReversibleAction()
        };

        Assert.Throws<InvalidOperationException>(() =>
            ReversibleActionExecution.Execute(target, actions, parameter: null));
        Assert.Equal("Original", target.Text);

        Assert.True((bool)((IReversibleAction)propertyAction).ExecuteReversibly(target, null)!);
        Assert.True((bool)propertyAction.Revert(target, null));
        Assert.Equal("Original", target.Text);
    }

    private static Style CreateTextStyle(string className, string value)
    {
        return new Style(x => x.OfType<TextBlock>().Class(className))
        {
            Setters =
            {
                new Setter(TextBlock.TextProperty, value)
            }
        };
    }

    private sealed class WriteOnlyTarget
    {
        public string? Value
        {
            set => WrittenValue = value;
        }

        public string? WrittenValue { get; private set; }
    }

    private sealed class ReadWriteTarget
    {
        public string? Value { get; set; }
    }

    private sealed class DirectPropertyTarget : AvaloniaObject
    {
        private string? _value;

        public static readonly DirectProperty<DirectPropertyTarget, string?> ValueProperty =
            AvaloniaProperty.RegisterDirect<DirectPropertyTarget, string?>(
                nameof(Value),
                target => target.Value,
                (target, value) => target.Value = value);

        public string? Value
        {
            get => _value;
            set => SetAndRaise(ValueProperty, ref _value, value);
        }
    }

    private sealed class StyledPropertySource : AvaloniaObject
    {
        public static readonly StyledProperty<string?> ValueProperty =
            AvaloniaProperty.Register<StyledPropertySource, string?>(nameof(Value));

        public string? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
