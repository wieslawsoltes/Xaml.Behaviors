// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Metadata;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// A base class for behaviors, implementing the basic plumbing of <seealso cref="ITrigger"/>.
/// </summary>
#if UNO
[ContentProperty(Name = nameof(Actions))]
#endif
public abstract partial class Trigger : Behavior, ITrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
#if UNO
    public static readonly DependencyProperty ActionsProperty =
#else
    public static readonly DirectProperty<Trigger, ActionCollection> ActionsProperty =
#endif
        AvaloniaProperty.RegisterDirect<Trigger, ActionCollection>(nameof(Actions), t => t.Actions);

#if UNO
    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is a dependency property.
    /// </summary>
    /// <remarks>
    /// The collection is stored in <see cref="ActionsProperty"/> so the actions inherit the data context.
    /// </remarks>
    public ActionCollection Actions
    {
        get
        {
            if (GetValue(ActionsProperty) is not ActionCollection actions)
            {
                actions = [];
                SetValue(ActionsProperty, actions);
            }

            return actions;
        }
    }
#else
    private ActionCollection? _actions;

    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection Actions => _actions ??= [];
#endif
}
