// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Executes actions when the dialog window (Uno Platform: <c>ContentDialog</c>) is opened.
/// </summary>
public partial class DialogOpenedTrigger : StyledElementTrigger<Control>
{

    /// <summary>
    /// Gets or sets the source object from which this behavior listens for events. This is an avalonia property.
    /// </summary>
#if UNO
    [StyledProperty(ResolveByName = true)]
    public partial ContentDialog? SourceObject { get; set; }
#else
    [StyledProperty(ResolveByName = true)]
    public partial Window? SourceObject { get; set; }
#endif

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        var window = SourceObject;
        if (window is not null)
        {
            window.Opened += OnOpened;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var window = SourceObject;
        if (window is not null)
        {
            window.Opened -= OnOpened;
        }
    }

#if UNO
    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs e)
    {
        Execute(e);
    }
#else
    private void OnOpened(object? sender, EventArgs e)
    {
        Execute(e);
    }
#endif

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
