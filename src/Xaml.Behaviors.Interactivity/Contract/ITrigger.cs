﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Interface implemented by all custom triggers.
/// </summary>
public interface ITrigger : IBehavior
{
    /// <summary>
    /// Gets the collection of actions associated with the behavior.
    /// </summary>
    ActionCollection? Actions { get; }
}
