// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Xaml.Interactivity;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>IPseudoClasses</c> view of <see cref="StyleClasses"/>.
/// </summary>
internal interface IPseudoClasses
{
    /// <summary>Adds a pseudo class.</summary>
    /// <param name="name">The pseudo class name (Avalonia convention: <c>:name</c>).</param>
    void Add(string name);

    /// <summary>Removes a pseudo class.</summary>
    /// <param name="name">The pseudo class name.</param>
    /// <returns><c>true</c> when the pseudo class was removed.</returns>
    bool Remove(string name);

    /// <summary>Checks whether a pseudo class is set.</summary>
    /// <param name="name">The pseudo class name.</param>
    /// <returns><c>true</c> when the pseudo class is set.</returns>
    bool Contains(string name);
}

/// <summary>
/// Uno Platform counterpart of the Avalonia style <c>Classes</c> of an element.
/// </summary>
/// <remarks>
/// WinUI has no style classes. The set is kept per element and every change is mapped to the
/// <see cref="VisualStateManager"/>: adding a class (or a pseudo class, without its leading <c>:</c>) goes to the
/// visual state of that name (or its PascalCase variant, e.g. <c>:dragging</c> → <c>dragging</c> or
/// <c>Dragging</c>); removing a class that is the current state of its group goes to the <c>Not{Name}</c>,
/// <c>Normal</c> or <c>Default</c> state of that group. The states are looked up on the element when it is a
/// <see cref="Microsoft.UI.Xaml.Controls.Control"/> (template root, or content of a <c>UserControl</c>/<c>Page</c>),
/// or on the element itself when it is the template/content root of its parent control.
/// </remarks>
internal sealed class StyleClasses : IPseudoClasses, IReadOnlyCollection<string>
{
    private static readonly ConditionalWeakTable<FrameworkElement, StyleClasses> s_classes = new();
    private static readonly string[] s_fallbackStates = ["Normal", "Default"];

    private readonly WeakReference<FrameworkElement> _owner;
    private readonly List<string> _items = [];

    private StyleClasses(FrameworkElement owner)
    {
        _owner = new WeakReference<FrameworkElement>(owner);
    }

    /// <summary>Occurs when the set changes.</summary>
    public event EventHandler? Changed;

    /// <inheritdoc />
    public int Count => _items.Count;

    /// <summary>
    /// Gets the classes of an element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The classes.</returns>
    public static StyleClasses Get(FrameworkElement element) => s_classes.GetValue(element, static e => new StyleClasses(e));

    /// <summary>
    /// Checks whether a class is set on an element without creating the set.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The class name.</param>
    /// <returns><c>true</c> when the class is set.</returns>
    public static bool Contains(FrameworkElement element, string name)
        => s_classes.TryGetValue(element, out var classes) && classes.Contains(name);

    /// <inheritdoc />
    public bool Contains(string name) => _items.Contains(name);

    /// <inheritdoc />
    public void Add(string name)
    {
        if (string.IsNullOrEmpty(name) || _items.Contains(name))
        {
            return;
        }

        _items.Add(name);
        ApplyState(name, isSet: true);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public bool Remove(string name)
    {
        if (!_items.Remove(name))
        {
            return false;
        }

        ApplyState(name, isSet: false);
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <summary>
    /// Adds or removes a class.
    /// </summary>
    /// <param name="name">The class name.</param>
    /// <param name="value">Whether the class is set.</param>
    public void Set(string name, bool value)
    {
        if (value)
        {
            Add(name);
        }
        else
        {
            Remove(name);
        }
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void ApplyState(string name, bool isSet)
    {
        if (!_owner.TryGetTarget(out var element) || !TryGetStateOwner(element, out var control, out var root))
        {
            return;
        }

        var stateName = name.TrimStart(':');
        if (stateName.Length == 0)
        {
            return;
        }

        if (isSet)
        {
            if (!VisualStateManager.GoToState(control, stateName, true))
            {
                var pascal = ToPascalCase(stateName);
                if (!string.Equals(pascal, stateName, StringComparison.Ordinal))
                {
                    VisualStateManager.GoToState(control, pascal, true);
                }
            }

            return;
        }

        if (root is null)
        {
            return;
        }

        foreach (var group in VisualStateManager.GetVisualStateGroups(root))
        {
            var current = group.CurrentState?.Name;
            if (current is null
                || !(string.Equals(current, stateName, StringComparison.Ordinal)
                     || string.Equals(current, ToPascalCase(stateName), StringComparison.Ordinal)))
            {
                continue;
            }

            if (FindFallbackState(group, current) is { } fallback)
            {
                VisualStateManager.GoToState(control, fallback, true);
            }
        }
    }

    private static string? FindFallbackState(VisualStateGroup group, string current)
    {
        var negated = "Not" + ToPascalCase(current);
        foreach (var state in group.States)
        {
            if (string.Equals(state.Name, negated, StringComparison.OrdinalIgnoreCase))
            {
                return state.Name;
            }
        }

        foreach (var candidate in s_fallbackStates)
        {
            foreach (var state in group.States)
            {
                if (string.Equals(state.Name, candidate, StringComparison.Ordinal))
                {
                    return state.Name;
                }
            }
        }

        return null;
    }

    private static bool TryGetStateOwner(FrameworkElement element, out Microsoft.UI.Xaml.Controls.Control control, out FrameworkElement? root)
    {
        if (element is Microsoft.UI.Xaml.Controls.Control self)
        {
            control = self;
            root = VisualTreeHelper.GetChildrenCount(self) > 0 ? VisualTreeHelper.GetChild(self, 0) as FrameworkElement : null;
            return true;
        }

        if (VisualTreeHelper.GetParent(element) is Microsoft.UI.Xaml.Controls.Control parent
            && VisualTreeHelper.GetChildrenCount(parent) > 0
            && ReferenceEquals(VisualTreeHelper.GetChild(parent, 0), element))
        {
            control = parent;
            root = element;
            return true;
        }

        control = null!;
        root = null;
        return false;
    }

    private static string ToPascalCase(string name)
    {
        if (name.Length == 0 || char.IsUpper(name[0]))
        {
            return name;
        }

        var chars = new char[name.Length];
        var upper = true;
        var length = 0;
        foreach (var c in name)
        {
            if (c is '-' or '_')
            {
                upper = true;
                continue;
            }

            chars[length++] = upper ? char.ToUpperInvariant(c) : c;
            upper = false;
        }

        return new string(chars, 0, length);
    }
}

/// <summary>
/// Exposes <see cref="StyleClasses"/> as the Avalonia <c>Classes</c> member of an element.
/// </summary>
internal static class StyleClassesExtensions
{
    extension(FrameworkElement element)
    {
        /// <summary>Gets the style classes of the element.</summary>
        public StyleClasses Classes => StyleClasses.Get(element);
    }
}
