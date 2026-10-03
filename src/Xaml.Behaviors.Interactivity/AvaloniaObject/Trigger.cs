// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// A base class for behaviors, implementing the basic plumbing of <seealso cref="ITrigger"/>.
/// </summary>
public abstract partial class Trigger : Behavior, ITrigger
{
    /// <summary>
    /// Gets the collection of actions associated with the behavior. This is an avalonia property.
    /// </summary>
    [DirectProperty(Lazy = true, Content = true)]
    public partial ActionCollection Actions { get; }
}
