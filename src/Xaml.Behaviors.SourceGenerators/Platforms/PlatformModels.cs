// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// The kinds of source files produced by the generator (selects the using directives of a file).
    /// </summary>
    internal enum GeneratedSourceKind
    {
        Action,
        Trigger,
        ChangePropertyAction,
        DataTrigger,
        MultiDataTrigger,
        InvokeCommandAction,
        PropertyTrigger,
        EventCommand,
        EventArgsAction,
        AsyncTrigger,
        ObservableTrigger,
    }

    /// <summary>
    /// The dispatcher helpers a generated type uses.
    /// </summary>
    [Flags]
    internal enum DispatcherFeatures
    {
        None = 0,

        /// <summary>Posts work to the UI thread.</summary>
        Post = 1,

        /// <summary>Checks UI thread access and synchronously invokes a function on the UI thread.</summary>
        Invoke = 2,
    }

    /// <summary>
    /// How a property trigger observes the source property.
    /// </summary>
    internal enum PropertyObservationKind
    {
        /// <summary>An Avalonia <c>StyledProperty</c> or <c>DirectProperty</c> (observed with <c>GetObservable</c>).</summary>
        AvaloniaProperty,

        /// <summary>A WinUI <c>DependencyProperty</c> (observed with <c>RegisterPropertyChangedCallback</c>).</summary>
        DependencyProperty,

        /// <summary>A CLR property of a type implementing <c>INotifyPropertyChanged</c>.</summary>
        NotifyPropertyChanged,
    }

    /// <summary>
    /// Describes a property registered on a generated type.
    /// </summary>
    /// <param name="Name">The CLR property name (the field is named <c>{Name}Property</c>).</param>
    /// <param name="Type">The property type, including nullable annotations.</param>
    /// <param name="TypeOf">The property type usable in a <c>typeof</c> expression.</param>
    /// <param name="DefaultValue">The default value expression, or <c>null</c> for <c>default</c>.</param>
    /// <param name="PrivateSetter">Whether the setter is private (Avalonia; WinUI dependency properties are always settable).</param>
    internal sealed record PropertySpec(string Name, string Type, string TypeOf, string? DefaultValue = null, bool PrivateSetter = false);

    /// <summary>
    /// Local identifiers used by the Avalonia name scope lookup helper (kept per emitter so the Avalonia output is stable).
    /// </summary>
    internal sealed record NameScopeLookupNames(string Logical, string Ancestor, string Element, string Scope);

    /// <summary>
    /// Describes the source property observed by a property trigger.
    /// </summary>
    /// <param name="Kind">The observation strategy.</param>
    /// <param name="TargetTypeName">The type the resolved source must be an instance of.</param>
    /// <param name="OwnerTypeName">The type declaring the property identifier field.</param>
    /// <param name="FieldName">The property identifier field name.</param>
    /// <param name="ValueTypeName">The property value type.</param>
    /// <param name="ClrPropertyName">The CLR property name (used for <see cref="PropertyObservationKind.NotifyPropertyChanged"/>).</param>
    internal sealed record PropertyObservation(
        PropertyObservationKind Kind,
        string TargetTypeName,
        string OwnerTypeName,
        string FieldName,
        string ValueTypeName,
        string? ClrPropertyName);

    /// <summary>
    /// Diagnostics that only apply to one platform (computed during analysis, reported for the resolved platform).
    /// </summary>
    internal sealed record PlatformDiagnostics(ImmutableArray<Diagnostic> Avalonia, ImmutableArray<Diagnostic> WinUI)
    {
        public static PlatformDiagnostics Empty { get; } = new(ImmutableArray<Diagnostic>.Empty, ImmutableArray<Diagnostic>.Empty);

        public ImmutableArray<Diagnostic> For(TargetPlatform platform)
        {
            var diagnostics = platform == TargetPlatform.WinUI ? WinUI : Avalonia;
            return diagnostics.IsDefault ? ImmutableArray<Diagnostic>.Empty : diagnostics;
        }
    }
}
