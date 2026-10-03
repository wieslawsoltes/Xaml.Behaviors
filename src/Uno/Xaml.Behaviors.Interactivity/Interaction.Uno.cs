// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <content>
/// Uno Platform counterpart of the Avalonia <c>BehaviorCollectionTemplate</c>: behaviors created per element from a
/// template, typically assigned from a style setter.
/// </content>
public partial class Interaction
{
    /// <summary>
    /// Identifies the <c>BehaviorsTemplate</c> attached property.
    /// </summary>
    /// <remarks>
    /// The template root must be a <see cref="BehaviorCollectionHost"/> whose content are the behaviors. Each element gets
    /// its own collection created from the template, so the template can be shared through a style:
    /// <code language="xml">
    /// &lt;Setter Property="i:Interaction.BehaviorsTemplate"&gt;
    ///   &lt;Setter.Value&gt;
    ///     &lt;DataTemplate&gt;
    ///       &lt;i:BehaviorCollectionHost&gt;
    ///         &lt;ic:EventTriggerBehavior EventName="Click"&gt;...&lt;/ic:EventTriggerBehavior&gt;
    ///       &lt;/i:BehaviorCollectionHost&gt;
    ///     &lt;/DataTemplate&gt;
    ///   &lt;/Setter.Value&gt;
    /// &lt;/Setter&gt;
    /// </code>
    /// </remarks>
    public static readonly DependencyProperty BehaviorsTemplateProperty =
        DependencyProperty.RegisterAttached(
            "BehaviorsTemplate",
            typeof(DataTemplate),
            typeof(Interaction),
            new PropertyMetadata(null, static (d, e) => BehaviorsTemplateChanged(d, e.NewValue as DataTemplate)));

    /// <summary>
    /// Gets the template creating the behaviors of an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The template.</returns>
    public static DataTemplate? GetBehaviorsTemplate(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (DataTemplate?)obj.GetValue(BehaviorsTemplateProperty);
    }

    /// <summary>
    /// Sets the template creating the behaviors of an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The template.</param>
    public static void SetBehaviorsTemplate(DependencyObject obj, DataTemplate? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(BehaviorsTemplateProperty, value);
    }

    private static void BehaviorsTemplateChanged(DependencyObject obj, DataTemplate? template)
    {
        if (template is null)
        {
            SetBehaviors(obj, null);
            return;
        }

        if (template.LoadContent() is not BehaviorCollectionHost host)
        {
            throw new InvalidOperationException($"The root of the behaviors template must be a {nameof(BehaviorCollectionHost)}.");
        }

        var behaviors = host.Behaviors;
        host.ClearValue(BehaviorCollectionHost.BehaviorsProperty);
        SetBehaviors(obj, behaviors);
    }
}
