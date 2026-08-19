using Avalonia.Headless.XUnit;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class ValidationRuleTests
{
    [AvaloniaFact]
    public void BoundRangeChanges_RevalidateAssociatedProperty()
    {
        var window = new BindableValidationRule001();
        var source = Assert.IsType<ValidationRuleBindingSource>(window.DataContext);
        var behavior = Assert.IsType<SliderValidationBehavior>(
            Assert.Single(Interaction.GetBehaviors(window.TargetSlider)));

        window.Show();

        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);

        source.Maximum = 40;

        Assert.False(behavior.IsValid);
        Assert.Equal(source.ErrorMessage, behavior.Error);

        source.ErrorMessage = "The configured maximum is too small.";

        Assert.Equal(source.ErrorMessage, behavior.Error);
    }

    [AvaloniaFact]
    public void RuleCollectionChanges_UpdateSubscriptionsAndValidation()
    {
        var window = new BindableValidationRule001();
        var behavior = Assert.IsType<SliderValidationBehavior>(
            Assert.Single(Interaction.GetBehaviors(window.TargetSlider)));

        window.Show();

        behavior.Rules.Clear();

        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);

        var rule = new MaxValueValidationRule<double>
        {
            MaxValue = 40,
            ErrorMessage = "Maximum is 40."
        };
        behavior.Rules.Add(rule);

        Assert.False(behavior.IsValid);
        Assert.Equal(rule.ErrorMessage, behavior.Error);

        rule.MaxValue = 100;

        Assert.True(behavior.IsValid);
        Assert.Null(behavior.Error);
    }

    [AvaloniaFact]
    public void ValidationRules_ExposeConfigurationAsAvaloniaProperties()
    {
        var maxValueRule = new MaxValueValidationRule<int>();
        Assert.Equal("Value is above maximum.", maxValueRule.ErrorMessage);
        maxValueRule.SetValue(MaxValueValidationRule<int>.MaxValueProperty, 10);
        maxValueRule.SetValue(MaxValueValidationRule<int>.ErrorMessageProperty, "max");
        Assert.Equal(10, maxValueRule.MaxValue);
        Assert.Equal("max", maxValueRule.ErrorMessage);

        var minLengthRule = new MinLengthValidationRule();
        Assert.Equal("Value is too short.", minLengthRule.ErrorMessage);
        minLengthRule.SetValue(MinLengthValidationRule.LengthProperty, 3);
        minLengthRule.SetValue(MinLengthValidationRule.ErrorMessageProperty, "length");
        Assert.Equal(3, minLengthRule.Length);
        Assert.Equal("length", minLengthRule.ErrorMessage);

        var minValueRule = new MinValueValidationRule<int>();
        Assert.Equal("Value is below minimum.", minValueRule.ErrorMessage);
        minValueRule.SetValue(MinValueValidationRule<int>.MinValueProperty, 1);
        minValueRule.SetValue(MinValueValidationRule<int>.ErrorMessageProperty, "min");
        Assert.Equal(1, minValueRule.MinValue);
        Assert.Equal("min", minValueRule.ErrorMessage);

        var notNullRule = new NotNullValidationRule<string>();
        Assert.Equal("Value is required.", notNullRule.ErrorMessage);
        notNullRule.SetValue(NotNullValidationRule<string>.ErrorMessageProperty, "null");
        Assert.Equal("null", notNullRule.ErrorMessage);

        var rangeRule = new RangeValidationRule<int>();
        Assert.Equal("Value is out of range.", rangeRule.ErrorMessage);
        rangeRule.SetValue(RangeValidationRule<int>.MinimumProperty, 2);
        rangeRule.SetValue(RangeValidationRule<int>.MaximumProperty, 8);
        rangeRule.SetValue(RangeValidationRule<int>.ErrorMessageProperty, "range");
        Assert.Equal(2, rangeRule.Minimum);
        Assert.Equal(8, rangeRule.Maximum);
        Assert.Equal("range", rangeRule.ErrorMessage);

        var regexRule = new RegexValidationRule();
        Assert.Equal("Invalid format.", regexRule.ErrorMessage);
        regexRule.SetValue(RegexValidationRule.PatternProperty, "^[0-9]+$");
        regexRule.SetValue(RegexValidationRule.ErrorMessageProperty, "regex");
        Assert.Equal("^[0-9]+$", regexRule.Pattern);
        Assert.Equal("regex", regexRule.ErrorMessage);

        var requiredDateRule = new RequiredDateValidationRule();
        Assert.Equal("Date is required.", requiredDateRule.ErrorMessage);
        requiredDateRule.SetValue(RequiredDateValidationRule.ErrorMessageProperty, "date");
        Assert.Equal("date", requiredDateRule.ErrorMessage);

        var requiredDecimalRule = new RequiredDecimalValidationRule();
        Assert.Equal("Value is required.", requiredDecimalRule.ErrorMessage);
        requiredDecimalRule.SetValue(RequiredDecimalValidationRule.ErrorMessageProperty, "decimal");
        Assert.Equal("decimal", requiredDecimalRule.ErrorMessage);

        var requiredTextRule = new RequiredTextValidationRule();
        Assert.Equal("Value is required.", requiredTextRule.ErrorMessage);
        requiredTextRule.SetValue(RequiredTextValidationRule.ErrorMessageProperty, "text");
        Assert.Equal("text", requiredTextRule.ErrorMessage);
    }
}
