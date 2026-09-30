// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Transitions = Microsoft.UI.Xaml.Media.Animation.TransitionCollection;
#else
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Sets the <see cref="Avalonia.Animation.Transitions"/> collection on the associated control when attached.
/// </summary>
public partial class TransitionsBehavior : AttachedToVisualTreeBehavior<Control>
{

    private Transitions? _oldTransitions;

    /// <summary>
    /// Gets or sets the transitions collection to apply. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Transitions? TransitionsSource { get; set; }

    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        _oldTransitions = TransitionOperations.Replace(AssociatedObject, TransitionsSource);

        return DisposableAction.Create(() =>
        {
            if (AssociatedObject is not null)
            {
                TransitionOperations.Replace(AssociatedObject, _oldTransitions);
            }
        });
    }
}
