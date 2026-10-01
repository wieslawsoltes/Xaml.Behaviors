// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.Custom.General.UnitTests;

#pragma warning disable CS8620 // Rules typed with non nullable strings, as in XAML.

public class LogicTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    // ---------------------------------------------------------------- Logic

    [UnoHeadlessFact]
    public void ConditionalAction_Executes_Actions_Or_ElseActions()
    {
        var action = new ConditionalAction();
        var ifAction = new RecordingAction();
        var elseAction = new RecordingAction();
        action.Actions!.Add(ifAction);
        action.ElseActions!.Add(elseAction);

        action.Execute(null, "a");
        action.Condition = true;
        action.Execute(null, "b");

        Assert.Equal(["b"], ifAction.Parameters);
        Assert.Equal(["a"], elseAction.Parameters);
    }

    [UnoHeadlessFact]
    public void SwitchCaseAction_Executes_The_Matching_Case_Or_The_Default()
    {
        var action = new SwitchCaseAction();
        var one = new Case { Value = 1 };
        var oneAction = new RecordingAction();
        one.Actions!.Add(oneAction);
        var two = new Case { Value = 2 };
        var twoAction = new RecordingAction();
        two.Actions!.Add(twoAction);
        action.Cases!.Add(one);
        action.Cases.Add(two);
        var defaultAction = new RecordingAction();
        action.DefaultActions!.Add(defaultAction);

        action.Value = 2;
        Assert.Equal(true, action.Execute(null, "p"));
        action.Value = 3;
        action.Execute(null, "d");

        Assert.Empty(oneAction.Parameters);
        Assert.Equal(["p"], twoAction.Parameters);
        Assert.Equal(["d"], defaultAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task SwitchCaseAction_Hosts_Case_Actions_On_The_Trigger_Element()
    {
        var button = new Button { Content = "b" };
        var trigger = new ButtonClickEventTriggerBehavior();
        var switchAction = new SwitchCaseAction { Value = "x" };
        var match = new Case { Value = "x" };
        var styledAction = new RecordingStyledAction();
        match.Actions!.Add(styledAction);
        switchAction.Cases!.Add(match);
        trigger.Actions!.Add(switchAction);
        Interaction.GetBehaviors(button).Add(trigger);
        await Session.ShowAsync(button);

        TestInput.Click(button);

        Assert.Single(styledAction.Parameters);
        Assert.Same(button, styledAction.ObservedHost);

        Interaction.GetBehaviors(button).Remove(trigger);
        Assert.Null(styledAction.ObservedHost);
    }

    [UnoHeadlessFact]
    public async Task SwitchCaseBehavior_Executes_When_The_Value_Changes()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var behavior = new SwitchCaseBehavior();
        var zero = new Case { Value = 0 };
        var zeroAction = new RecordingAction();
        zero.Actions!.Add(zeroAction);
        var five = new Case { Value = 5 };
        var fiveAction = new RecordingAction();
        five.Actions!.Add(fiveAction);
        behavior.Cases!.Add(zero);
        behavior.Cases.Add(five);
        BindingOperations.SetBinding(behavior, SwitchCaseBehavior.ValueProperty, new Binding { Path = new PropertyPath(nameof(TestViewModel.Count)) });
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();
        Assert.NotEmpty(zeroAction.Parameters);

        vm.Count = 5;
        await Session.WaitForIdleAsync();

        Assert.Single(fiveAction.Parameters);
    }

    [UnoHeadlessFact]
    public async Task Case_Actions_Inherit_The_DataContext()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        var behavior = new SwitchCaseBehavior { Value = 1 };
        var match = new Case { Value = 1 };
        var action = new BindableRecordingAction();
        BindingOperations.SetBinding(action, BindableRecordingAction.ValueProperty, new Binding());
        match.Actions!.Add(action);
        behavior.Cases!.Add(match);
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();

        Assert.NotEmpty(action.Values);
        Assert.All(action.Values, value => Assert.Same(vm, value));
    }

    [UnoHeadlessFact]
    public async Task ConditionalBehavior_Executes_Actions_Or_ElseActions_When_The_Condition_Changes()
    {
        var border = new Border();
        var behavior = new ConditionalBehavior();
        var ifAction = new RecordingStyledAction();
        var elseAction = new RecordingStyledAction();
        behavior.Actions!.Add(ifAction);
        behavior.ElseActions!.Add(elseAction);
        Interaction.GetBehaviors(border).Add(behavior);
        await Session.ShowAsync(new StackPanel { Children = { border } });

        behavior.Condition = true;
        behavior.Condition = false;

        Assert.Single(ifAction.Parameters);
        Assert.Single(elseAction.Parameters);
        Assert.Same(border, elseAction.ObservedHost);
    }

    // ---------------------------------------------------------------- Validation

    [UnoHeadlessFact]
    public async Task TextBoxValidationBehavior_Validates_The_Text()
    {
        var textBox = new TextBox();
        var behavior = new TextBoxValidationBehavior();
        var minLength = new MinLengthValidationRule { Length = 3, ErrorMessage = "short" };
        behavior.Rules.Add(new RequiredTextValidationRule { ErrorMessage = "required" });
        behavior.Rules.Add(minLength);
        Interaction.GetBehaviors(textBox).Add(behavior);
        await Session.ShowAsync(textBox);
        await Session.WaitForIdleAsync();

        Assert.False(behavior.IsValid);
        Assert.Equal("required" + Environment.NewLine + "short", behavior.Error);

        textBox.Text = "abcd";
        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);

        minLength.Length = 10;
        Assert.False(behavior.IsValid);
        Assert.Equal("short", behavior.Error);

        behavior.Rules.Remove(minLength);
        Assert.True(behavior.IsValid);
    }

    [UnoHeadlessFact]
    public async Task PropertyValidationBehavior_Observes_A_Property_Assigned_After_Attaching()
    {
        // WinUI assigns x:Bind values after the behaviors of a view are attached.
        var textBox = new TextBox();
        var behavior = new PropertyValidationBehavior<TextBox, string>();
        behavior.Rules.Add(new MinLengthValidationRule { Length = 3, ErrorMessage = "short" });
        Interaction.GetBehaviors(textBox).Add(behavior);
        await Session.ShowAsync(textBox);
        await Session.WaitForIdleAsync();

        behavior.Property = TextBox.TextProperty;
        Assert.False(behavior.IsValid);
        Assert.Equal("short", behavior.Error);

        textBox.Text = "abcd";
        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);
    }

    [UnoHeadlessFact]
    public async Task PropertyValidationBehavior_Revalidates_When_A_Custom_Rule_Changes()
    {
        // Issue #396: custom rules opt in to revalidation through the public IValidationRuleChanged contract.
        var textBox = new TextBox { Text = "abcd" };
        var behavior = new PropertyValidationBehavior<TextBox, string> { Property = TextBox.TextProperty };
        var maxLength = new MaxLengthTestRule { MaxLength = 10 };
        var forbidden = new ForbiddenTextTestRule();
        behavior.Rules.Add(maxLength);
        behavior.Rules.Add(forbidden);
        Interaction.GetBehaviors(textBox).Add(behavior);
        await Session.ShowAsync(textBox);
        await Session.WaitForIdleAsync();
        Assert.True(behavior.IsValid);
        Assert.Equal(1, maxLength.SubscriberCount);

        // A dependency property of a user-defined DependencyObject rule.
        maxLength.MaxLength = 2;
        Assert.False(behavior.IsValid);
        Assert.Equal("too long", behavior.Error);

        maxLength.MaxLength = 4;
        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);

        // A CLR property of a user-defined plain rule.
        forbidden.Forbidden = "abcd";
        Assert.False(behavior.IsValid);
        Assert.Equal("forbidden", behavior.Error);

        forbidden.Forbidden = null;
        Assert.True(behavior.IsValid);

        // Detaching stops observing the rules.
        Interaction.GetBehaviors(textBox).Remove(behavior);
        Assert.Equal(0, maxLength.SubscriberCount);
        maxLength.MaxLength = 1;
        Assert.True(behavior.IsValid);
    }

    [UnoHeadlessFact]
    public async Task PropertyValidationBehavior_Revalidates_When_The_Rules_Collection_Changes()
    {
        var textBox = new TextBox { Text = "abcd" };
        var behavior = new PropertyValidationBehavior<TextBox, string> { Property = TextBox.TextProperty };
        Interaction.GetBehaviors(textBox).Add(behavior);
        await Session.ShowAsync(textBox);
        await Session.WaitForIdleAsync();
        Assert.True(behavior.IsValid);

        // Add: the new rule is evaluated and observed.
        var tooShort = new MaxLengthTestRule { MaxLength = 2, ErrorMessage = "first" };
        behavior.Rules.Add(tooShort);
        Assert.False(behavior.IsValid);
        Assert.Equal("first", behavior.Error);
        Assert.Equal(1, tooShort.SubscriberCount);

        // Replace: the replaced rule is no longer observed.
        var longEnough = new MaxLengthTestRule { MaxLength = 10, ErrorMessage = "second" };
        behavior.Rules[0] = longEnough;
        Assert.True(behavior.IsValid);
        Assert.Equal(0, tooShort.SubscriberCount);
        Assert.Equal(1, longEnough.SubscriberCount);

        longEnough.MaxLength = 3;
        Assert.False(behavior.IsValid);
        Assert.Equal("second", behavior.Error);

        // Remove: revalidates without the removed rule, which is no longer observed.
        behavior.Rules.Remove(longEnough);
        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);
        Assert.Equal(0, longEnough.SubscriberCount);

        // A built-in rule is evaluated when it is added.
        behavior.Rules.Add(new MinLengthValidationRule { Length = 10, ErrorMessage = "short" });
        Assert.False(behavior.IsValid);
        Assert.Equal("short", behavior.Error);

        // Clear.
        behavior.Rules.Clear();
        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);
    }

    [UnoHeadlessFact]
    public async Task SliderValidationBehavior_Uses_Generic_Range_Rules()
    {
        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 50 };
        var behavior = new SliderValidationBehavior();
        var range = new RangeValidationRule<double> { Minimum = 10, Maximum = 60 };
        behavior.Rules.Add(range);
        behavior.Rules.Add(new MinValueValidationRule<double> { MinValue = 5 });
        behavior.Rules.Add(new MaxValueValidationRule<double> { MaxValue = 90 });
        behavior.Rules.Add(new NotNullValidationRule<double>());
        Interaction.GetBehaviors(slider).Add(behavior);
        await Session.ShowAsync(slider);
        await Session.WaitForIdleAsync();
        Assert.True(behavior.IsValid);

        slider.Value = 70;
        Assert.False(behavior.IsValid);
        Assert.Equal("Value is out of range.", behavior.Error);

        range.Maximum = 80;
        Assert.True(behavior.IsValid);
    }

    [UnoHeadlessFact]
    public async Task ComboBox_And_DatePicker_Validation_Behaviors_Observe_Their_Property()
    {
        var comboBox = new ComboBox { ItemsSource = new[] { "a", "b" } };
        var comboBehavior = new ComboBoxValidationBehavior();
        comboBehavior.Rules.Add(new NotNullValidationRule<object?>());
        Interaction.GetBehaviors(comboBox).Add(comboBehavior);
        var datePicker = new DatePicker();
        var dateBehavior = new DatePickerValidationBehavior();
        dateBehavior.Rules.Add(new RequiredDateValidationRule());
        Interaction.GetBehaviors(datePicker).Add(dateBehavior);
        await Session.ShowAsync(new StackPanel { Children = { comboBox, datePicker } });
        await Session.WaitForIdleAsync();
        Assert.False(comboBehavior.IsValid);
        Assert.False(dateBehavior.IsValid);

        comboBox.SelectedIndex = 1;
        datePicker.SelectedDate = DateTimeOffset.Now;

        Assert.True(comboBehavior.IsValid);
        Assert.True(dateBehavior.IsValid);
    }

    [UnoHeadlessFact]
    public void Validation_Rules_Validate_Values()
    {
        Assert.True(new RegexValidationRule { Pattern = "^[0-9]+$" }.Validate("123"));
        Assert.False(new RegexValidationRule { Pattern = "^[0-9]+$" }.Validate("12a"));
        Assert.True(new RequiredDecimalValidationRule().Validate(1m));
        Assert.False(new RequiredDecimalValidationRule().Validate(null));
        Assert.Equal("Value is required.", new RequiredTextValidationRule().ErrorMessage);
    }

    // ---------------------------------------------------------------- ViewModel

    [UnoHeadlessFact]
    public void ViewModel_Actions_Update_DataContext_Properties()
    {
        var vm = new TestViewModel { Count = 1 };
        var border = new Border { DataContext = vm };

        Assert.True((bool)new IncrementViewModelPropertyAction { PropertyName = nameof(TestViewModel.Count), Delta = 2 }.Execute(border, null));
        Assert.Equal(3, vm.Count);

        Assert.True((bool)new ToggleViewModelBooleanAction { PropertyName = nameof(TestViewModel.Flag) }.Execute(border, null));
        Assert.True(vm.Flag);

        Assert.True((bool)new SetViewModelPropertyAction { PropertyName = nameof(TestViewModel.Name), Value = "name" }.Execute(border, null));
        Assert.Equal("name", vm.Name);

        Assert.False((bool)new SetViewModelPropertyAction { PropertyName = nameof(TestViewModel.Name) }.Execute(new Border(), null));
    }

    [UnoHeadlessFact]
    public async Task SetViewModelPropertyOnLoadBehavior_And_ViewModelPropertyChangedTrigger_Work_With_The_DataContext()
    {
        var vm = new TestViewModel();
        var border = new Border { DataContext = vm };
        Interaction.GetBehaviors(border).Add(new SetViewModelPropertyOnLoadBehavior { PropertyName = nameof(TestViewModel.Count), Value = 4 });
        var trigger = new ViewModelPropertyChangedTrigger { PropertyName = nameof(TestViewModel.Name) };
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(border).Add(trigger);
        await Session.ShowAsync(border);
        await Session.WaitForIdleAsync();

        Assert.Equal(4, vm.Count);

        vm.Name = "changed";
        await Session.WaitForIdleAsync();

        Assert.Single(action.Parameters);
    }

    // ---------------------------------------------------------------- Icon

    [UnoHeadlessFact]
    public async Task PathIcon_Behaviors_Set_And_Observe_The_Data()
    {
        var first = new RectangleGeometry { Rect = new Windows.Foundation.Rect(0, 0, 10, 10) };
        var second = new EllipseGeometry { RadiusX = 5, RadiusY = 5 };
        var icon = new PathIcon();
        var trigger = new PathIconDataChangedTrigger();
        var action = new RecordingAction();
        trigger.Actions!.Add(action);
        Interaction.GetBehaviors(icon).Add(trigger);
        var behavior = new PathIconDataBehavior { Data = first };
        Interaction.GetBehaviors(icon).Add(behavior);
        await Session.ShowAsync(new StackPanel { Children = { icon } });
        await Session.WaitForIdleAsync();

        Assert.Same(first, icon.Data);

        Assert.True((bool)new SetPathIconDataAction { Data = second }.Execute(icon, null));
        Assert.Same(second, icon.Data);
        Assert.NotEmpty(action.Parameters);

        Interaction.GetBehaviors(icon).Remove(behavior);
        Assert.Null(icon.Data);
    }
}
