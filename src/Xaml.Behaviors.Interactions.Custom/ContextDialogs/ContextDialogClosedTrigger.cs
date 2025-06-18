// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using Avalonia.Xaml.Interactions.Core;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <c>Closed</c> event of <see cref="ContextDialogBehavior"/>.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ContextDialogClosedTrigger : EventTriggerBehavior
{
    static ContextDialogClosedTrigger()
    {
        EventNameProperty.OverrideMetadata<ContextDialogClosedTrigger>(
            new StyledPropertyMetadata<string?>("Closed"));
    }
}
