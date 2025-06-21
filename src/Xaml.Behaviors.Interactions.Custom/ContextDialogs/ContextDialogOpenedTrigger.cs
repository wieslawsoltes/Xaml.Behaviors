// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <c>Opened</c> event of <see cref="ContextDialogBehavior"/>.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ContextDialogOpenedTrigger : EventTriggerBase
{
    static ContextDialogOpenedTrigger()
    {
        EventNameProperty.OverrideMetadata<ContextDialogOpenedTrigger>(
            new StyledPropertyMetadata<string?>("Opened"));
    }
}
