// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Behaviors.Storyboards;

/// <summary>
/// Indicates how a storyboard should interact with any existing animations that target the same properties.
/// </summary>
public enum StoryboardHandoffBehavior
{
    /// <summary>
    /// Replaces any existing animations on the targeted properties.
    /// </summary>
    SnapshotAndReplace = 0,

    /// <summary>
    /// Composes with existing animations so that multiple storyboards may run simultaneously.
    /// </summary>
    Compose = 1
}
