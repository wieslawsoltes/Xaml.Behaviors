// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.Interactivity;

/// <summary>
/// A base class for triggers, implementing the basic plumbing of <see cref="ITrigger"/>.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia <c>StyledElementTrigger</c>. The actions are stored in a dependency
/// property so they inherit the data context of the associated object.
/// </remarks>
public abstract partial class StyledElementTrigger : StyledElementBehavior, ITrigger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StyledElementTrigger"/> class.
    /// </summary>
    protected StyledElementTrigger()
    {
        Actions = new ActionCollection();
    }

    /// <summary>
    /// Gets or sets the collection of actions associated with the behavior.
    /// </summary>
    [StyledProperty(Content = true)]
    public partial ActionCollection? Actions { get; set; }

    partial void OnActionsChanged(ActionCollection? oldValue, ActionCollection? newValue)
    {
        oldValue?.SetHost(null);
        newValue?.SetHost(AssociatedObject);
    }
}
