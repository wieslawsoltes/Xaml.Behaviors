// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Executes actions when the dialog window is opened.
/// </summary>
public class DialogOpenedTrigger : StyledElementTrigger<Window>
{
    /// <summary>
    /// Identifies the <seealso cref="SourceObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Window?> SourceObjectProperty =
        AvaloniaProperty.Register<DialogOpenedTrigger, Window?>(nameof(SourceObject));

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Window? SourceObject
    {
        get => GetValue(SourceObjectProperty);
        set => SetValue(SourceObjectProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        var window = SourceObject ?? AssociatedObject;
        if (window is not null)
        {
            window.Opened += OnOpened;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var window = SourceObject ?? AssociatedObject;
        if (window is not null)
        {
            window.Opened -= OnOpened;
        }
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Execute(e);
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (AssociatedObject is not null)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
        }
    }
}
