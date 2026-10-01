// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace BehaviorsTestApplication.Converters;

/// <summary>
/// Values for compiled bindings (<c>{x:Bind converters:SampleValues.X}</c>) where the Uno XAML generator cannot
/// convert the literal of the Avalonia view.
/// </summary>
public static class SampleValues
{
    /// <summary>
    /// Gets the minimum date of DatePickerValidationBehaviorView (Avalonia literal <c>Minimum="2020-01-01"</c>).
    /// </summary>
    public static DateTimeOffset? MinimumValidatedDate { get; } = new DateTimeOffset(new DateTime(2020, 1, 1));

    /// <summary>
    /// Gets the maximum date of DatePickerValidationBehaviorView (Avalonia literal <c>Maximum="2030-12-31"</c>).
    /// </summary>
    public static DateTimeOffset? MaximumValidatedDate { get; } = new DateTimeOffset(new DateTime(2030, 12, 31));
}
