// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;
#endif

#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Defines a <see cref="BehaviorCollection"/> attached property and provides a method for executing an <seealso cref="ActionCollection"/>.
/// </summary>
public class Interaction
{
#if UNO
    /// <summary>
    /// Gets or sets the <see cref="BehaviorCollection"/> associated with a specified object.
    /// </summary>
    public static readonly DependencyProperty BehaviorsProperty =
        DependencyProperty.RegisterAttached(
            "Behaviors",
            typeof(BehaviorCollection),
            typeof(Interaction),
            new PropertyMetadata(null, static (d, e) => BehaviorsChanged(d, e.OldValue as BehaviorCollection, e.NewValue as BehaviorCollection)));
#else
    static Interaction()
    {
        BehaviorsProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<BehaviorCollection?>>(
                static e => BehaviorsChanged(e.Sender, e.OldValue.GetValueOrDefault(), e.NewValue.GetValueOrDefault())));
    }

    /// <summary>
    /// Gets or sets the <see cref="BehaviorCollection"/> associated with a specified object.
    /// </summary>
    public static readonly AttachedProperty<BehaviorCollection?> BehaviorsProperty =
        AvaloniaProperty.RegisterAttached<Interaction, AvaloniaObject, BehaviorCollection?>("Behaviors");
#endif

    /// <summary>
    /// Gets the <see cref="BehaviorCollection"/> associated with a specified object.
    /// </summary>
    /// <param name="obj">The <see cref="AvaloniaObject"/> from which to retrieve the <see cref="BehaviorCollection"/>.</param>
    /// <returns>A <see cref="BehaviorCollection"/> containing the behaviors associated with the specified object.</returns>
    public static BehaviorCollection GetBehaviors(AvaloniaObject obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var behaviorCollection = (BehaviorCollection?)obj.GetValue(BehaviorsProperty);
        if (behaviorCollection is null)
        {
            behaviorCollection = [];
            obj.SetValue(BehaviorsProperty, behaviorCollection);
            SetVisualTreeEventHandlersFromGetter(obj);
        }

        return behaviorCollection;
    }

    private static BehaviorCollection? GetExistingBehaviors(AvaloniaObject obj)
    {
        return (BehaviorCollection?)obj.GetValue(BehaviorsProperty);
    }

    /// <summary>
    /// Sets the <see cref="BehaviorCollection"/> associated with a specified object.
    /// </summary>
    /// <param name="obj">The <see cref="AvaloniaObject"/> on which to set the <see cref="BehaviorCollection"/>.</param>
    /// <param name="value">The <see cref="BehaviorCollection"/> associated with the object.</param>
    public static void SetBehaviors(AvaloniaObject obj, BehaviorCollection? value)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        obj.SetValue(BehaviorsProperty, value);
    }

    /// <summary>
    /// Executes all actions in the <see cref="ActionCollection"/> and returns their results.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> which will be passed on to the action.</param>
    /// <param name="actions">The set of actions to execute.</param>
    /// <param name="parameter">The value of this parameter is determined by the calling behavior.</param>
    /// <returns>Returns the results of the actions.</returns>
    public static IEnumerable<object> ExecuteActions(object? sender, ActionCollection? actions, object? parameter)
    {
        if (actions is null)
        {
            return [];
        }

        var results = new List<object>();

        foreach (var avaloniaObject in actions)
        {
            if (avaloniaObject is not IAction action)
            {
                continue;
            }

            var result = action.Execute(sender, parameter);
            if (result is not null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    private static void BehaviorsChanged(AvaloniaObject sender, BehaviorCollection? oldCollection, BehaviorCollection? newCollection)
    {
        if (oldCollection == newCollection)
        {
            return;
        }

        var isAttachedToVisualTree = IsAttachedToVisualTree(sender);

        if (oldCollection is { AssociatedObject: not null })
        {
            if (isAttachedToVisualTree)
            {
                oldCollection.DetachedFromVisualTree();
            }

            oldCollection.Detach();
        }

        if (newCollection is not null)
        {
            SetVisualTreeEventHandlersFromChangedEvent(sender);
            newCollection.Attach(sender);

            if (isAttachedToVisualTree)
            {
                newCollection.AttachedToVisualTree();
            }
        }
    }

#if UNO
    // WinUI has a single live tree notification pair. Loaded raises the Avalonia initialization, logical tree,
    // visual tree and loaded phases (in that order) and Unloaded raises them in reverse order.

    private static bool IsAttachedToVisualTree(AvaloniaObject obj) => obj is FrameworkElement { IsLoaded: true };

    private static void SetVisualTreeEventHandlersFromGetter(AvaloniaObject obj)
    {
        if (obj is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= Element_Loaded_FromChangedEvent;
        element.Unloaded -= Element_Unloaded_FromChangedEvent;
        element.Loaded -= Element_Loaded_FromGetter;
        element.Loaded += Element_Loaded_FromGetter;
        element.Unloaded -= Element_Unloaded_FromGetter;
        element.Unloaded += Element_Unloaded_FromGetter;
        SetStateEventHandlers(element);
    }

    private static void SetVisualTreeEventHandlersFromChangedEvent(AvaloniaObject obj)
    {
        if (obj is not FrameworkElement element)
        {
            return;
        }

        element.Loaded -= Element_Loaded_FromGetter;
        element.Unloaded -= Element_Unloaded_FromGetter;
        element.Loaded -= Element_Loaded_FromChangedEvent;
        element.Loaded += Element_Loaded_FromChangedEvent;
        element.Unloaded -= Element_Unloaded_FromChangedEvent;
        element.Unloaded += Element_Unloaded_FromChangedEvent;
        SetStateEventHandlers(element);
    }

    private static void SetStateEventHandlers(FrameworkElement element)
    {
        element.DataContextChanged -= Element_DataContextChanged;
        element.DataContextChanged += Element_DataContextChanged;
        element.ActualThemeChanged -= Element_ActualThemeChanged;
        element.ActualThemeChanged += Element_ActualThemeChanged;
    }

    private static void Element_Loaded_FromGetter(object sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d || GetExistingBehaviors(d) is not { } behaviors)
        {
            return;
        }

        behaviors.Attach(d);
        RaiseAttached(behaviors);
    }

    private static void Element_Unloaded_FromGetter(object sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d || GetExistingBehaviors(d) is not { } behaviors)
        {
            return;
        }

        RaiseDetached(behaviors);
        behaviors.Detach();
    }

    private static void Element_Loaded_FromChangedEvent(object sender, RoutedEventArgs e)
    {
        if (sender is AvaloniaObject d && GetExistingBehaviors(d) is { } behaviors)
        {
            RaiseAttached(behaviors);
        }
    }

    private static void Element_Unloaded_FromChangedEvent(object sender, RoutedEventArgs e)
    {
        if (sender is AvaloniaObject d && GetExistingBehaviors(d) is { } behaviors)
        {
            RaiseDetached(behaviors);
        }
    }

    private static void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        => GetExistingBehaviors(sender)?.NotifyDataContextChanged();

    private static void Element_ActualThemeChanged(FrameworkElement sender, object args)
        => GetExistingBehaviors(sender)?.ActualThemeVariantChanged();

    private static void RaiseAttached(BehaviorCollection behaviors)
    {
        behaviors.Initialized();
        behaviors.AttachedToLogicalTree();
        behaviors.AttachedToVisualTree();
        behaviors.Loaded();
    }

    private static void RaiseDetached(BehaviorCollection behaviors)
    {
        behaviors.Unloaded();
        behaviors.DetachedFromVisualTree();
        behaviors.DetachedFromLogicalTree();
    }
#else
    private static bool IsAttachedToVisualTree(AvaloniaObject obj) => obj is Visual visual && visual.IsAttachedToVisualTree();

    private static void SetVisualTreeEventHandlersFromGetter(AvaloniaObject obj)
    {
        if (obj is Visual visual)
        {
            // AttachedToVisualTree / DetachedFromVisualTree

            visual.AttachedToVisualTree -= Visual_AttachedToVisualTree_FromChangedEvent;
            visual.DetachedFromVisualTree -= Visual_DetachedFromVisualTree_FromChangedEvent;
            visual.AttachedToVisualTree -= Visual_AttachedToVisualTree_FromGetter;
            visual.AttachedToVisualTree += Visual_AttachedToVisualTree_FromGetter;
            visual.DetachedFromVisualTree -= Visual_DetachedFromVisualTree_FromGetter;
            visual.DetachedFromVisualTree += Visual_DetachedFromVisualTree_FromGetter;
        }

        if (obj is StyledElement styledElement)
        {
            // AttachedToLogicalTree / DetachedFromLogicalTree

            styledElement.AttachedToLogicalTree -= StyledElement_AttachedToLogicalTree_FromChangedEvent;
            styledElement.DetachedFromLogicalTree -= StyledElement_DetachedFromLogicalTree_FromChangedEvent;
            styledElement.AttachedToLogicalTree -= StyledElement_AttachedToLogicalTree_FromGetter;
            styledElement.AttachedToLogicalTree += StyledElement_AttachedToLogicalTree_FromGetter;
            styledElement.DetachedFromLogicalTree -= StyledElement_DetachedFromLogicalTree_FromGetter;
            styledElement.DetachedFromLogicalTree += StyledElement_DetachedFromLogicalTree_FromGetter;
  
            // Initialized

            styledElement.Initialized -= StyledElement_Initialized_FromChangedEvent;
            styledElement.Initialized -= StyledElement_Initialized_FromGetter;
            styledElement.Initialized += StyledElement_Initialized_FromGetter;
            
            // DataContextChanged

            styledElement.DataContextChanged -= StyledElement_DataContextChanged_FromChangedEvent;
            styledElement.DataContextChanged -= StyledElement_DataContextChanged_FromGetter;
            styledElement.DataContextChanged += StyledElement_DataContextChanged_FromGetter;
  
            // ResourcesChanged

            styledElement.ResourcesChanged -= StyledElement_ResourcesChanged_FromChangedEvent;
            styledElement.ResourcesChanged -= StyledElement_ResourcesChanged_FromGetter;
            styledElement.ResourcesChanged += StyledElement_ResourcesChanged_FromGetter;

            // ActualThemeVariantChanged

            styledElement.ActualThemeVariantChanged -= StyledElement_ActualThemeVariantChanged_FromChangedEvent;
            styledElement.ActualThemeVariantChanged -= StyledElement_ActualThemeVariantChanged_FromGetter;
            styledElement.ActualThemeVariantChanged += StyledElement_ActualThemeVariantChanged_FromGetter;
        }

        if (obj is Control control)
        {
            // Loaded / Unloaded

            control.Loaded -= Control_Loaded_FromChangedEvent;
            control.Unloaded -= Control_Unloaded_FromChangedEvent;
            control.Loaded -= Control_Loaded_FromGetter;
            control.Loaded += Control_Loaded_FromGetter;
            control.Unloaded -= Control_Unloaded_FromGetter;
            control.Unloaded += Control_Unloaded_FromGetter;
        }

        if (obj is TopLevel topLevel)
        {
            topLevel.Opened -= TopLevel_Opened_FromChangedEvent;
            topLevel.Opened -= TopLevel_Opened_FromGetter;
            topLevel.Opened += TopLevel_Opened_FromGetter;
        }
    }

    private static void SetVisualTreeEventHandlersFromChangedEvent(AvaloniaObject obj)
    {
        if (obj is Visual visual)
        {
            // AttachedToVisualTree / DetachedFromVisualTree

            visual.AttachedToVisualTree -= Visual_AttachedToVisualTree_FromGetter;
            visual.DetachedFromVisualTree -= Visual_DetachedFromVisualTree_FromGetter;
            visual.AttachedToVisualTree -= Visual_AttachedToVisualTree_FromChangedEvent;
            visual.AttachedToVisualTree += Visual_AttachedToVisualTree_FromChangedEvent;
            visual.DetachedFromVisualTree -= Visual_DetachedFromVisualTree_FromChangedEvent;
            visual.DetachedFromVisualTree += Visual_DetachedFromVisualTree_FromChangedEvent;
        }

        if (obj is StyledElement styledElement)
        {
            // AttachedToLogicalTree / DetachedFromLogicalTree

            styledElement.AttachedToLogicalTree -= StyledElement_AttachedToLogicalTree_FromGetter;
            styledElement.DetachedFromLogicalTree -= StyledElement_DetachedFromLogicalTree_FromGetter;
            styledElement.AttachedToLogicalTree -= StyledElement_AttachedToLogicalTree_FromChangedEvent;
            styledElement.AttachedToLogicalTree += StyledElement_AttachedToLogicalTree_FromChangedEvent;
            styledElement.DetachedFromLogicalTree -= StyledElement_DetachedFromLogicalTree_FromChangedEvent;
            styledElement.DetachedFromLogicalTree += StyledElement_DetachedFromLogicalTree_FromChangedEvent;

            // Initialized

            styledElement.Initialized -= StyledElement_Initialized_FromGetter;
            styledElement.Initialized -= StyledElement_Initialized_FromChangedEvent;
            styledElement.Initialized += StyledElement_Initialized_FromChangedEvent;
            
            // DataContextChanged

            styledElement.DataContextChanged -= StyledElement_DataContextChanged_FromGetter;
            styledElement.DataContextChanged -= StyledElement_DataContextChanged_FromChangedEvent;
            styledElement.DataContextChanged += StyledElement_DataContextChanged_FromChangedEvent;
  
            // ResourcesChanged

            styledElement.ResourcesChanged -= StyledElement_ResourcesChanged_FromGetter;
            styledElement.ResourcesChanged -= StyledElement_ResourcesChanged_FromChangedEvent;
            styledElement.ResourcesChanged += StyledElement_ResourcesChanged_FromChangedEvent;

            // ActualThemeVariantChanged

            styledElement.ActualThemeVariantChanged -= StyledElement_ActualThemeVariantChanged_FromGetter;
            styledElement.ActualThemeVariantChanged -= StyledElement_ActualThemeVariantChanged_FromChangedEvent;
            styledElement.ActualThemeVariantChanged += StyledElement_ActualThemeVariantChanged_FromChangedEvent;
        }

        if (obj is Control control)
        {
            // Loaded / Unloaded

            control.Loaded -= Control_Loaded_FromGetter;
            control.Unloaded -= Control_Unloaded_FromGetter;
            control.Loaded -= Control_Loaded_FromChangedEvent;
            control.Loaded += Control_Loaded_FromChangedEvent;
            control.Unloaded -= Control_Unloaded_FromChangedEvent;
            control.Unloaded += Control_Unloaded_FromChangedEvent;
        }

        if (obj is TopLevel topLevel)
        {
            topLevel.Opened -= TopLevel_Opened_FromGetter;
            topLevel.Opened -= TopLevel_Opened_FromChangedEvent;
            topLevel.Opened += TopLevel_Opened_FromChangedEvent;
        }
    }

    // AttachedToVisualTree / DetachedFromVisualTree

    private static void Visual_AttachedToVisualTree_FromGetter(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetExistingBehaviors(d);
        behaviors?.Attach(d);
        behaviors?.AttachedToVisualTree();
    }

    private static void Visual_DetachedFromVisualTree_FromGetter(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetExistingBehaviors(d);
        if (behaviors is null)
        {
            return;
        }

        behaviors.DetachedFromVisualTree();

        if (d is TopLevel topLevel)
        {
            ScheduleTopLevelBehaviorDetach(topLevel, behaviors);
        }
        else
        {
            behaviors.Detach();
        }
    }
 
    private static void Visual_AttachedToVisualTree_FromChangedEvent(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.AttachedToVisualTree();
    }

    private static void Visual_DetachedFromVisualTree_FromChangedEvent(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetExistingBehaviors(d);
        if (behaviors is null)
        {
            return;
        }

        behaviors.DetachedFromVisualTree();

        if (d is TopLevel topLevel)
        {
            ScheduleTopLevelBehaviorDetach(topLevel, behaviors);
        }
    }

    private static void ScheduleTopLevelBehaviorDetach(
        TopLevel topLevel,
        BehaviorCollection behaviors)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!topLevel.IsAttachedToVisualTree() &&
                ReferenceEquals(topLevel.GetValue(BehaviorsProperty), behaviors) &&
                behaviors.AssociatedObject is not null)
            {
                behaviors.Detach();
            }
        });
    }

    // AttachedToLogicalTree / DetachedFromLogicalTree

    private static void StyledElement_AttachedToLogicalTree_FromGetter(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.AttachedToLogicalTree();
    }

    private static void StyledElement_DetachedFromLogicalTree_FromGetter(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.DetachedFromLogicalTree();
    }
 
    private static void StyledElement_AttachedToLogicalTree_FromChangedEvent(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.AttachedToLogicalTree();
    }

    private static void StyledElement_DetachedFromLogicalTree_FromChangedEvent(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.DetachedFromLogicalTree();
    }

    // Loaded / Unloaded

    private static void Control_Loaded_FromGetter(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Loaded();
    }

    private static void Control_Unloaded_FromGetter(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Unloaded();
    }
 
    private static void Control_Loaded_FromChangedEvent(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Loaded();
    }

    private static void Control_Unloaded_FromChangedEvent(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Unloaded();
    }

    // Initialized
    
    private static void StyledElement_Initialized_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Initialized();
    }

    private static void StyledElement_Initialized_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Initialized();
    }

    // DataContextChanged
    
    private static void StyledElement_DataContextChanged_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.NotifyDataContextChanged();
    }

    private static void StyledElement_DataContextChanged_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.NotifyDataContextChanged();
    }

    // ResourcesChanged
    
    private static void StyledElement_ResourcesChanged_FromGetter(object? sender, ResourcesChangedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.ResourcesChanged();
    }

    private static void StyledElement_ResourcesChanged_FromChangedEvent(object? sender, ResourcesChangedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.ResourcesChanged();
    }

    // ActualThemeVariantChanged
    
    private static void StyledElement_ActualThemeVariantChanged_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.ActualThemeVariantChanged();
    }

    private static void StyledElement_ActualThemeVariantChanged_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.ActualThemeVariantChanged();
    }

    // TopLevel Opened

    private static void TopLevel_Opened_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetExistingBehaviors(d);
        behaviors?.Attach(d);
        behaviors?.Opened();
    }

    private static void TopLevel_Opened_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        GetExistingBehaviors(d)?.Opened();
    }
#endif
}
