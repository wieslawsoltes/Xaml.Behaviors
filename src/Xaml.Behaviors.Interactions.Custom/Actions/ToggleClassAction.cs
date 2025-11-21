// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Toggles a specified <see cref="ToggleClassAction.ClassName"/> in the <see cref="StyledElement.Classes"/> collection when invoked.
/// </summary>
public class ToggleClassAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="ClassName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ClassNameProperty =
        AvaloniaProperty.Register<ToggleClassAction, string?>(nameof(ClassName));

    /// <summary>
    /// Identifies the <seealso cref="StyledElement"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StyledElement?> StyledElementProperty =
        AvaloniaProperty.Register<ToggleClassAction, StyledElement?>(nameof(StyledElement));

    /// <summary>
    /// Gets or sets the class name that should be toggled. This is an avalonia property.
    /// </summary>
    public string? ClassName
    {
        get => GetValue(ClassNameProperty);
        set => SetValue(ClassNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the target styled element that class name that should be toggled on. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public StyledElement? StyledElement
    {
        get => GetValue(StyledElementProperty);
        set => SetValue(StyledElementProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = GetValue(StyledElementProperty) is not null ? StyledElement : sender as StyledElement;
        if (target is null || string.IsNullOrEmpty(ClassName))
        {
            return false;
        }

        if (target.Classes.Contains(ClassName!))
        {
            target.Classes.Remove(ClassName!);
        }
        else
        {
            target.Classes.Add(ClassName!);
        }

        return true;
    }
}
