// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
#if UNO
using Xaml.Interactivity;
#else
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Trigger that listens for the <c>Closed</c> event of <see cref="ContextDialogBehavior"/>.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ContextDialogClosedTrigger : EventTriggerBase
{
    static ContextDialogClosedTrigger()
    {
        EventNameProperty.OverrideMetadata<ContextDialogClosedTrigger>(
            new StyledPropertyMetadata<string?>("Closed"));
    }
}
