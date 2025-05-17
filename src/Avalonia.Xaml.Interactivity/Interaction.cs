using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Defines a <see cref="BehaviorCollection"/> attached property and provides a method for executing an <seealso cref="ActionCollection"/>.
/// </summary>
public class Interaction
{
    static Interaction()
    {
        BehaviorsProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<BehaviorCollection?>>(BehaviorsChanged));
    }

    /// <summary>
    /// Gets or sets the <see cref="BehaviorCollection"/> associated with a specified object.
    /// </summary>
    public static readonly AttachedProperty<BehaviorCollection?> BehaviorsProperty =
        AvaloniaProperty.RegisterAttached<Interaction, AvaloniaObject, BehaviorCollection?>("Behaviors");

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

        var behaviorCollection = obj.GetValue(BehaviorsProperty);
        if (behaviorCollection is null)
        {
            behaviorCollection = [];
            obj.SetValue(BehaviorsProperty, behaviorCollection);
            SetVisualTreeEventHandlersFromGetter(obj);
        }

        return behaviorCollection;
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

        var results = new List<object>(actions.Count);

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

    private static void BehaviorsChanged(AvaloniaPropertyChangedEventArgs<BehaviorCollection?> e)
    {
        var oldCollection = e.OldValue.GetValueOrDefault();
        var newCollection = e.NewValue.GetValueOrDefault();

        if (oldCollection == newCollection)
        {
            return;
        }

        if (oldCollection is { AssociatedObject: not null })
        {
            oldCollection.Detach();
        }

        if (newCollection is not null)
        {
            newCollection.Attach(e.Sender);
            SetVisualTreeEventHandlersFromChangedEvent(e.Sender);
        }
    }

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

        var behaviors = GetBehaviors(d);
        behaviors.Attach(d);
        behaviors.AttachedToVisualTree();
    }

    private static void Visual_DetachedFromVisualTree_FromGetter(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DetachedFromVisualTree();
        behaviors.Detach();
    }
 
    private static void Visual_AttachedToVisualTree_FromChangedEvent(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.AttachedToVisualTree();
    }

    private static void Visual_DetachedFromVisualTree_FromChangedEvent(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DetachedFromVisualTree();
    }

    // AttachedToLogicalTree / DetachedFromLogicalTree

    private static void StyledElement_AttachedToLogicalTree_FromGetter(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.AttachedToLogicalTree();
    }

    private static void StyledElement_DetachedFromLogicalTree_FromGetter(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DetachedFromLogicalTree();
    }
 
    private static void StyledElement_AttachedToLogicalTree_FromChangedEvent(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.AttachedToLogicalTree();
    }

    private static void StyledElement_DetachedFromLogicalTree_FromChangedEvent(object? sender, LogicalTreeAttachmentEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DetachedFromLogicalTree();
    }

    // Loaded / Unloaded

    private static void Control_Loaded_FromGetter(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Loaded();
    }

    private static void Control_Unloaded_FromGetter(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Unloaded();
    }
 
    private static void Control_Loaded_FromChangedEvent(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Loaded();
    }

    private static void Control_Unloaded_FromChangedEvent(object? sender, RoutedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Unloaded();
    }

    // Initialized
    
    private static void StyledElement_Initialized_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Initialized();
    }

    private static void StyledElement_Initialized_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Initialized();
    }

    // DataContextChanged
    
    private static void StyledElement_DataContextChanged_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DataContextChanged();
    }

    private static void StyledElement_DataContextChanged_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.DataContextChanged();
    }

    // ResourcesChanged
    
    private static void StyledElement_ResourcesChanged_FromGetter(object? sender, ResourcesChangedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.ResourcesChanged();
    }

    private static void StyledElement_ResourcesChanged_FromChangedEvent(object? sender, ResourcesChangedEventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.ResourcesChanged();
    }

    // ActualThemeVariantChanged
    
    private static void StyledElement_ActualThemeVariantChanged_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.ActualThemeVariantChanged();
    }

    private static void StyledElement_ActualThemeVariantChanged_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.ActualThemeVariantChanged();
    }

    // TopLevel Opened

    private static void TopLevel_Opened_FromGetter(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.Attach(d);
        behaviors.AttachedToVisualTree();
        behaviors.AttachedToLogicalTree();
    }

    private static void TopLevel_Opened_FromChangedEvent(object? sender, EventArgs e)
    {
        if (sender is not AvaloniaObject d)
        {
            return;
        }

        var behaviors = GetBehaviors(d);
        behaviors.AttachedToVisualTree();
        behaviors.AttachedToLogicalTree();
    }
}
