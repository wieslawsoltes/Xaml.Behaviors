using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Avalonia.Xaml.Interactivity.UnitTests;

[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Validates the reflection-based compatibility comparison helper.")]
public class ComparisonConditionTypeHelperTests
{
    private sealed class ThrowingComparable : IComparable
    {
        public int CompareCalls { get; private set; }

        public int CompareTo(object? obj)
        {
            CompareCalls++;
            throw new ArgumentException("Operands are not comparable.", nameof(obj));
        }
    }

    [Theory]
    [InlineData(ComparisonConditionType.Equal, false)]
    [InlineData(ComparisonConditionType.NotEqual, true)]
    [InlineData(ComparisonConditionType.LessThan, false)]
    [InlineData(ComparisonConditionType.LessThanOrEqual, false)]
    [InlineData(ComparisonConditionType.GreaterThan, false)]
    [InlineData(ComparisonConditionType.GreaterThanOrEqual, false)]
    public void Compare_NonConvertibleString_UsesNonEqualSemantics(
        ComparisonConditionType operatorType,
        bool expected)
    {
        var result = ComparisonConditionTypeHelper.Compare(42, operatorType, "not-an-integer");

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compare_OverflowingString_UsesNonEqualSemantics()
    {
        var result = ComparisonConditionTypeHelper.Compare(
            42,
            ComparisonConditionType.Equal,
            "999999999999999999999999999999");

        Assert.False(result);
    }

    [Theory]
    [InlineData(ComparisonConditionType.Equal, false)]
    [InlineData(ComparisonConditionType.NotEqual, true)]
    public void Compare_IncompatibleComparable_UsesNonEqualSemantics(
        ComparisonConditionType operatorType,
        bool expected)
    {
        var left = new ThrowingComparable();
        var right = new ThrowingComparable();

        var result = ComparisonConditionTypeHelper.Compare(left, operatorType, right);

        Assert.Equal(expected, result);
        Assert.Equal(1, left.CompareCalls);
    }
}
