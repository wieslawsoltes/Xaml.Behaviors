// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Avalonia.Xaml.Interactivity;

internal static class ComparisonConditionTypeHelper
{
    [RequiresUnreferencedCode("Calls Avalonia.Xaml.Interactivity.TypeConverterHelper.Convert(String, Type)")]
    public static bool Compare(object? leftOperand, ComparisonConditionType operatorType, object? rightOperand)
    {
        if (leftOperand is not null && rightOperand is not null)
        {
            if (rightOperand is string rightOperandString)
            {
                var leftOperandType = leftOperand.GetType();
                var convertedRightOperand = TypeConverterHelper.Convert(rightOperandString, leftOperandType);
                if (convertedRightOperand is not null)
                {
                    rightOperand = convertedRightOperand;
                }
            }
        }

        var leftComparableOperand = leftOperand as IComparable;
        var rightComparableOperand = rightOperand as IComparable;
        if (leftComparableOperand is not null && rightComparableOperand is not null)
        {
            return EvaluateComparable(leftComparableOperand, operatorType, rightComparableOperand);
        }

        switch (operatorType)
        {
            case ComparisonConditionType.Equal:
                return Equals(leftOperand, rightOperand);

            case ComparisonConditionType.NotEqual:
                return !Equals(leftOperand, rightOperand);

            case ComparisonConditionType.LessThan:
            case ComparisonConditionType.LessThanOrEqual:
            case ComparisonConditionType.GreaterThan:
            case ComparisonConditionType.GreaterThanOrEqual:
            {
                throw leftComparableOperand switch
                {
                    null when rightComparableOperand is null => new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture,
                        "Binding property of type {0} and Value property of type {1} cannot be used with operator {2}.",
                        leftOperand?.GetType().Name ?? "null", rightOperand?.GetType().Name ?? "null",
                        operatorType.ToString())),
                    null => new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Binding property of type {0} cannot be used with operator {1}.",
                        leftOperand?.GetType().Name ?? "null", operatorType.ToString())),
                    _ => new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Value property of type {0} cannot be used with operator {1}.",
                        rightOperand?.GetType().Name ?? "null", operatorType.ToString()))
                };
            }
        }

        return false;
    }

    /// <summary>
    /// Evaluates both operands that implement the IComparable interface.
    /// </summary>
    private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
    {
        object? convertedOperand = null;
        try
        {
            convertedOperand = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            // FormatException: Convert.ChangeType("hello", typeof(double), ...);
        }
        catch (InvalidCastException)
        {
            // InvalidCastException: Convert.ChangeType(4.0d, typeof(Rectangle), ...);
        }

        if (convertedOperand is null)
        {
            return operatorType == ComparisonConditionType.NotEqual;
        }

        var comparison = leftOperand.CompareTo((IComparable)convertedOperand);
        return operatorType switch
        {
            ComparisonConditionType.Equal => comparison == 0,
            ComparisonConditionType.NotEqual => comparison != 0,
            ComparisonConditionType.LessThan => comparison < 0,
            ComparisonConditionType.LessThanOrEqual => comparison <= 0,
            ComparisonConditionType.GreaterThan => comparison > 0,
            ComparisonConditionType.GreaterThanOrEqual => comparison >= 0,
            _ => false
        };
    }
}
