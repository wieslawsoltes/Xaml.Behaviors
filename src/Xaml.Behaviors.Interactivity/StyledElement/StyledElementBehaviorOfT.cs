﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// A base class for behaviors making them code compatible with older frameworks,
/// and allow for typed associated objects.
/// </summary>
/// <typeparam name="T">The object type to attach to</typeparam>
public abstract class StyledElementBehavior<T> : StyledElementBehavior where T : AvaloniaObject
{
    /// <summary>
    /// Gets the object to which this behavior is attached.
    /// </summary>
    public new T? AssociatedObject => base.AssociatedObject as T;

    /// <summary>
    /// Called after the behavior is attached to the <see cref="StyledElementBehavior.AssociatedObject"/>.
    /// </summary>
    /// <remarks>
    /// Override this to hook up functionality to the <see cref="StyledElementBehavior.AssociatedObject"/>
    /// </remarks>
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is null && base.AssociatedObject is not null)
        {
            var actualType = base.AssociatedObject?.GetType().FullName;
            var expectedType = typeof(T).FullName;
            var message = $"AssociatedObject is of type {actualType} but should be of type {expectedType}.";
            throw new InvalidOperationException(message);
        }
    }
}
