// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Base class for behaviors that manage focus on a control.
/// </summary>
public abstract class FocusBehaviorBase : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Gets or sets the navigation method used when focusing.
    /// </summary>
    public static readonly StyledProperty<NavigationMethod> NavigationMethodProperty =
        AvaloniaProperty.Register<FocusBehaviorBase, NavigationMethod>(nameof(NavigationMethod));
    
    /// <summary>
    /// Gets or sets keyboard modifiers used when focusing.
    /// </summary>
    public static readonly StyledProperty<KeyModifiers> KeyModifiersProperty =
        AvaloniaProperty.Register<FocusBehaviorBase, KeyModifiers>(nameof(KeyModifiers));

    /// <summary>
    /// 
    /// </summary>
    public NavigationMethod NavigationMethod
    {
        get => GetValue(NavigationMethodProperty);
        set => SetValue(NavigationMethodProperty, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public KeyModifiers KeyModifiers
    {
        get => GetValue(KeyModifiersProperty);
        set => SetValue(KeyModifiersProperty, value);
    }

    /// <summary>
    /// Sets focus on the associated control.
    /// </summary>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    protected virtual bool Focus()
    {
        if (!IsEnabled)
        {
            return false;
        }

        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus(NavigationMethod, KeyModifiers));

        return true;

    }
}
