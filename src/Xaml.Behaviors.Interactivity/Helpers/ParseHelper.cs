// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Avalonia.Xaml.Interactivity;

internal static class ParseHelper
{
    private static readonly Dictionary<Type, Func<string, object?>> s_parsers;

    static ParseHelper()
    {
        s_parsers = new Dictionary<Type, Func<string, object?>>
        {
            [typeof(Easing)] = s => Easing.Parse(s),
            [typeof(IEasing)] = s => Easing.Parse(s),
            [typeof(Cue)] = s => Cue.Parse(s, CultureInfo.InvariantCulture),
            [typeof(IterationCount)] = s => IterationCount.Parse(s),
            [typeof(KeySpline)] = s => KeySpline.Parse(s, CultureInfo.InvariantCulture),
            [typeof(Classes)] = s => Classes.Parse(s),
            [typeof(Cursor)] = s => Cursor.Parse(s),
            [typeof(KeyGesture)] = s => KeyGesture.Parse(s),
            [typeof(IEffect)] = s => Effect.Parse(s),
            [typeof(Effect)] = s => Effect.Parse(s),
            [typeof(TransformOperations)] = s => TransformOperations.Parse(s),
            [typeof(BoxShadow)] = s => BoxShadow.Parse(s),
            [typeof(BoxShadows)] = s => BoxShadows.Parse(s),
            [typeof(Brush)] = s => Brush.Parse(s),
            [typeof(IBrush)] = s => Brush.Parse(s),
            [typeof(Color)] = s => Color.Parse(s),
            [typeof(FontFamily)] = s => FontFamily.Parse(s),
            [typeof(FontFeature)] = s => FontFeature.Parse(s),
            [typeof(Geometry)] = s => Geometry.Parse(s),
            [typeof(PathGeometry)] = s => PathGeometry.Parse(s),
            [typeof(PathFigures)] = s => PathFigures.Parse(s),
            [typeof(SolidColorBrush)] = s => SolidColorBrush.Parse(s),
            [typeof(StreamGeometry)] = s => StreamGeometry.Parse(s),
            [typeof(TextDecorationCollection)] = s => TextDecorationCollection.Parse(s),
            [typeof(TextTrimming)] = s => TextTrimming.Parse(s),
            [typeof(Transform)] = s => Transform.Parse(s),
            [typeof(ITransform)] = s => Transform.Parse(s),
            [typeof(UnicodeRange)] = s => UnicodeRange.Parse(s),
            [typeof(UnicodeRangeSegment)] = s => UnicodeRangeSegment.Parse(s),
            [typeof(CornerRadius)] = s => CornerRadius.Parse(s),
            [typeof(Matrix)] = s => Matrix.Parse(s),
            [typeof(PixelPoint)] = s => PixelPoint.Parse(s),
            [typeof(PixelRect)] = s => PixelRect.Parse(s),
            [typeof(PixelSize)] = s => PixelSize.Parse(s),
            [typeof(Point)] = s => Point.Parse(s),
            [typeof(Rect)] = s => Rect.Parse(s),
            [typeof(RelativePoint)] = s => RelativePoint.Parse(s),
            [typeof(RelativeRect)] = s => RelativeRect.Parse(s),
            [typeof(RelativeScalar)] = s => RelativeScalar.Parse(s),
            [typeof(Size)] = s => Size.Parse(s),
            [typeof(Thickness)] = s => Thickness.Parse(s),
            [typeof(Vector3D)] = s => Vector3D.Parse(s),
            [typeof(ColumnDefinitions)] = s => ColumnDefinitions.Parse(s),
            [typeof(RowDefinitions)] = s => RowDefinitions.Parse(s),
        };
    }

    public static object? InvokeParse(string s, Type targetType)
    {
        if (s is null) throw new ArgumentNullException(nameof(s));
        if (targetType is null) throw new ArgumentNullException(nameof(targetType));

        return s_parsers.TryGetValue(targetType, out var parser)
            ? parser(s)
            : null;
    }
}
