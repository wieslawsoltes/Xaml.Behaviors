// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace Xaml.Interactivity;

/// <summary>
/// A base class for triggers, implementing the basic plumbing of <see cref="ITrigger"/>.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia <c>StyledElementTrigger</c>. The actions are stored in a dependency
/// property so they inherit the data context of the associated object.
/// </remarks>
[ContentProperty(Name = nameof(Actions))]
public abstract partial class StyledElementTrigger : StyledElementBehavior, ITrigger
{
    /// <summary>
    /// Identifies the <seealso cref="Actions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ActionsProperty =
        AvaloniaProperty.Register<StyledElementTrigger, ActionCollection?>(nameof(Actions));

    /// <summary>
    /// Initializes a new instance of the <see cref="StyledElementTrigger"/> class.
    /// </summary>
    protected StyledElementTrigger()
    {
        SetValue(ActionsProperty, new ActionCollection());
    }

    /// <summary>
    /// Gets or sets the collection of actions associated with the behavior.
    /// </summary>
    public ActionCollection? Actions
    {
        get => (ActionCollection?)GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }
}
