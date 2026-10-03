// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Implemented by non action containers of actions (for example switch cases) that join the action tree through
/// their owner; nested actions are hosted by <see cref="ActionHost"/>.
/// </summary>
/// <remarks>
/// Avalonia containers are logical tree elements; on Uno Platform the action tree resolves the host of a nested
/// action from its parent action (<see cref="Action.Host"/>), behavior (associated object) or this provider.
/// </remarks>
internal interface IActionHostProvider
{
    /// <summary>
    /// Gets the object hosting the actions of the container, or <c>null</c> when it is detached.
    /// </summary>
    DependencyObject? ActionHost { get; }
}
