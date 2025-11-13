// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Provides C# 14 extension members that surface <see cref="Interaction.BehaviorsProperty"/> directly on <see cref="AvaloniaObject"/>.
/// </summary>
/// <remarks>
/// XAML compilers that support C# 14 extension metadata can reference the attached property through inherited syntax
/// such as <c>&lt;Border.Behaviors&gt;</c>. This helper keeps the runtime registration in place while exposing a
/// property-like API for code-behind and markup.
/// </remarks>
public static class AvaloniaObjectBehaviorsExtensions
{
    static AvaloniaObjectBehaviorsExtensions()
    {
        Interaction.BehaviorsProperty.AddOwner<AvaloniaObject>();
    }

    /// <summary>
    /// Publishes static helpers so XAML can bind to <see cref="Interaction.BehaviorsProperty"/> using inherited property syntax.
    /// </summary>
    extension(AvaloniaObject)
    {
        /// <summary>
        /// Exposes the behaviors attached property for XAML consumers targeting <see cref="AvaloniaObject"/>.
        /// </summary>
        /// <remarks>
        /// This accessor can be used from XAML with the syntax
        /// <code language="xml">
        /// &lt;Border.Behaviors&gt;
        ///   &lt;i:EventTriggerBehavior /&gt;
        /// &lt;/Border.Behaviors&gt;
        /// </code>
        /// where <c>i</c> is the behaviors namespace prefix.
        /// </remarks>
        public static AttachedProperty<BehaviorCollection?> BehaviorsProperty => Interaction.BehaviorsProperty;

        /// <summary>
        /// Delegates to <see cref="Interaction.GetBehaviors(AvaloniaObject)"/>.
        /// </summary>
        public static BehaviorCollection GetBehaviors(AvaloniaObject obj) => Interaction.GetBehaviors(obj);

        /// <summary>
        /// Delegates to <see cref="Interaction.SetBehaviors(AvaloniaObject, BehaviorCollection?)"/>.
        /// </summary>
        public static void SetBehaviors(AvaloniaObject obj, BehaviorCollection? value) => Interaction.SetBehaviors(obj, value);
    }
}
