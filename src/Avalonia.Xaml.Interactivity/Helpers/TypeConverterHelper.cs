using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A helper class that enables converting values specified in markup (strings) to their object representation.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
internal static class TypeConverterHelper
{
    /// <summary>
    /// Converts string representation of a value to its object representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationType">The destination type.</param>
    /// <returns>Object representation of the string value.</returns>
    /// <exception cref="ArgumentNullException">destinationType cannot be null.</exception>
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DynamicallyAccessedMembers handles most of the problems.")]
    public static object? Convert(string value, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        var destinationTypeFullName = destinationType.FullName;
        if (destinationTypeFullName is null)
        {
            return null;
        }

        var scope = GetScope(destinationTypeFullName);

        // Value types in the "System" namespace must be special cased due to a bug in the xaml compiler
        if (string.Equals(scope, "System", StringComparison.Ordinal))
        {
            if (string.Equals(destinationTypeFullName, typeof(string).FullName, StringComparison.Ordinal))
            {
                return value;
            }

            if (string.Equals(destinationTypeFullName, typeof(bool).FullName, StringComparison.Ordinal))
            {
                return bool.Parse(value);
            }

            if (string.Equals(destinationTypeFullName, typeof(int).FullName, StringComparison.Ordinal))
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }

            if (string.Equals(destinationTypeFullName, typeof(double).FullName, StringComparison.Ordinal))
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        try
        {
            if (destinationType.BaseType == typeof(Enum))
                return Enum.Parse(destinationType, value);

            if (destinationType.GetInterfaces().Any(t => t == typeof(IConvertible)))
            {
                return (value as IConvertible).ToType(destinationType, CultureInfo.InvariantCulture);
            }

            var converter = TypeDescriptor.GetConverter(destinationType);
            return converter.ConvertFromInvariantString(value);
        }
        catch (ArgumentException)
        {
            // not an enum
        }
        catch (InvalidCastException)
        {
            // not able to convert to anything
        }
        catch (NotSupportedException)
        {
            // not able to convert from string
            try
            {
                var parseResult = InvokeParse(value, destinationType);
                if (parseResult is not null)
                {
                    return parseResult;
                }
            }
            catch (Exception)
            {
                // not able to parse
                return null;
            }
        }

        return null;
    }

    private static string GetScope(string name)
    {
        var indexOfLastPeriod = name.LastIndexOf('.');
#if !NET6_0_OR_GREATER
        if (indexOfLastPeriod != name.Length - 1)
        {
            return name.Substring(0, indexOfLastPeriod);
        }

        return name;
#else
        return indexOfLastPeriod != name.Length - 1 ? name[..indexOfLastPeriod] : name;
#endif
    }

    private static object? InvokeParse(string s, Type targetType)
    {
        if (targetType == typeof(IEasing) || targetType == typeof(Easing))
        {
            return Easing.Parse(s);
        }
        else if (targetType == typeof(Cue))
        {
            return Cue.Parse(s, CultureInfo.InvariantCulture);
        }
        else if (targetType == typeof(IterationCount))
        {
            return IterationCount.Parse(s);
        }
        else if (targetType == typeof(KeySpline))
        {
            return KeySpline.Parse(s, CultureInfo.InvariantCulture);
        }
        else if (targetType == typeof(Classes))
        {
            return Classes.Parse(s);
        }
        else if (targetType == typeof(Cursor))
        {
            return Cursor.Parse(s);
        }
        else if (targetType == typeof(KeyGesture))
        {
            return KeyGesture.Parse(s);
        }
        else if (targetType == typeof(IEffect) || targetType == typeof(Effect))
        {
            return Effect.Parse(s);
        }
        else if (targetType == typeof(TransformOperations))
        {
            return TransformOperations.Parse(s);
        }
        else if (targetType == typeof(BoxShadow))
        {
            return BoxShadow.Parse(s);
        }
        else if (targetType == typeof(BoxShadows))
        {
            return BoxShadows.Parse(s);
        }
        else if (targetType == typeof(IBrush) || targetType == typeof(Brush))
        {
            return Brush.Parse(s);
        }
        else if (targetType == typeof(Color))
        {
            return Color.Parse(s);
        }
        else if (targetType == typeof(FontFamily))
        {
            return FontFamily.Parse(s);
        }
        else if (targetType == typeof(FontFeature))
        {
            return FontFeature.Parse(s);
        }
        else if (targetType == typeof(Geometry))
        {
            return Geometry.Parse(s);
        }
        else if (targetType == typeof(PathGeometry))
        {
            return PathGeometry.Parse(s);
        }
        else if (targetType == typeof(PathFigures))
        {
            return PathFigures.Parse(s);
        }
        else if (targetType == typeof(SolidColorBrush))
        {
            return SolidColorBrush.Parse(s);
        }
        else if (targetType == typeof(StreamGeometry))
        {
            return StreamGeometry.Parse(s);
        }
        else if (targetType == typeof(TextDecorationCollection))
        {
            return TextDecorationCollection.Parse(s);
        }
        else if (targetType == typeof(TextTrimming))
        {
            return TextTrimming.Parse(s);
        }
        else if (targetType == typeof(ITransform) || targetType == typeof(Transform))
        {
            return Transform.Parse(s);
        }
        else if (targetType == typeof(UnicodeRange))
        {
            return UnicodeRange.Parse(s);
        }
        else if (targetType == typeof(UnicodeRangeSegment))
        {
            return UnicodeRangeSegment.Parse(s);
        }
        else if (targetType == typeof(CornerRadius))
        {
            return CornerRadius.Parse(s);
        }
        else if (targetType == typeof(Matrix))
        {
            return Matrix.Parse(s);
        }
        else if (targetType == typeof(PixelPoint))
        {
            return PixelPoint.Parse(s);
        }
        else if (targetType == typeof(PixelRect))
        {
            return PixelRect.Parse(s);
        }
        else if (targetType == typeof(PixelSize))
        {
            return PixelSize.Parse(s);
        }
        else if (targetType == typeof(Point))
        {
            return Point.Parse(s);
        }
        else if (targetType == typeof(Rect))
        {
            return Rect.Parse(s);
        }
        else if (targetType == typeof(RelativePoint))
        {
            return RelativePoint.Parse(s);
        }
        else if (targetType == typeof(RelativeRect))
        {
            return RelativeRect.Parse(s);
        }
        else if (targetType == typeof(RelativeScalar))
        {
            return RelativeScalar.Parse(s);
        }
        else if (targetType == typeof(Size))
        {
            return Size.Parse(s);
        }
        else if (targetType == typeof(Thickness))
        {
            return Thickness.Parse(s);
        }
        else if (targetType == typeof(Vector3D))
        {
            return Vector3D.Parse(s);
        }
        else if (targetType == typeof(ColumnDefinitions))
        {
            return ColumnDefinitions.Parse(s);
        }
        else if (targetType == typeof(RowDefinitions))
        {
            return RowDefinitions.Parse(s);
        }

        return null;
    }
}
