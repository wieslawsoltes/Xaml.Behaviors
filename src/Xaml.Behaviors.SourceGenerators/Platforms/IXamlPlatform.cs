// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// The platform strategy used by the emitters: a composition of small, platform specific snippet providers.
    /// </summary>
    internal interface IXamlPlatform
    {
        TargetPlatform Kind { get; }

        IUsingDirectivesEmitter Usings { get; }

        IInteractivityTypeNames Types { get; }

        IPropertySystemEmitter Properties { get; }

        IDispatcherEmitter Dispatcher { get; }

        INameScopeEmitter NameScope { get; }

        IPropertyObservationEmitter PropertyObservation { get; }
    }

    /// <summary>
    /// Emits the using directives of a generated file.
    /// </summary>
    internal interface IUsingDirectivesEmitter
    {
        void Append(StringBuilder sb, GeneratedSourceKind kind);
    }

    /// <summary>
    /// Names of the interactivity runtime types generated code derives from.
    /// </summary>
    internal interface IInteractivityTypeNames
    {
        /// <summary>The base class of generated actions.</summary>
        string StyledElementAction { get; }

        /// <summary>The base class of generated triggers.</summary>
        string StyledElementTrigger { get; }

        /// <summary>The reversible action contract.</summary>
        string ReversibleAction { get; }
    }

    /// <summary>
    /// Emits property registrations, accessors and change notification plumbing.
    /// </summary>
    internal interface IPropertySystemEmitter
    {
        /// <summary>The parameter type of the <c>OnPropertyChanged</c> override.</summary>
        string ChangedEventArgsType { get; }

        /// <summary>Appends the property identifier field (two lines, no trailing blank line).</summary>
        void AppendField(StringBuilder sb, string ownerClassName, PropertySpec property);

        /// <summary>Appends the CLR accessor property (no trailing blank line).</summary>
        void AppendAccessor(StringBuilder sb, PropertySpec property);

        /// <summary>Gets an expression reading the typed new value from the change arguments.</summary>
        string NewValue(string changeVariable, string typeName);
    }

    /// <summary>
    /// Emits UI thread dispatching.
    /// </summary>
    internal interface IDispatcherEmitter
    {
        /// <summary>The method posting an <c>Action</c> to the UI thread.</summary>
        string PostMethod { get; }

        /// <summary>The method synchronously invoking a <c>Func&lt;bool&gt;</c> on the UI thread.</summary>
        string InvokeMethod { get; }

        /// <summary>An expression that is <c>true</c> on the UI thread.</summary>
        string CheckAccessExpression { get; }

        /// <summary>Appends the helper members the snippets above rely on (if any).</summary>
        void AppendHelpers(StringBuilder sb, DispatcherFeatures features);
    }

    /// <summary>
    /// Emits the <c>FindInNameScope(source, sourceName)</c> helper used to resolve <c>SourceName</c>.
    /// </summary>
    internal interface INameScopeEmitter
    {
        void AppendFindInNameScope(StringBuilder sb, NameScopeLookupNames names);
    }

    /// <summary>
    /// Emits the source property observation of property triggers.
    /// </summary>
    internal interface IPropertyObservationEmitter
    {
        /// <summary>Appends the subscription state fields.</summary>
        void AppendFields(StringBuilder sb, PropertyObservation observation);

        /// <summary>Appends the body of <c>Subscribe()</c> (observes <c>_resolvedSource</c> and calls <c>OnObserved(value)</c>).</summary>
        void AppendSubscribe(StringBuilder sb, PropertyObservation observation);

        /// <summary>Appends the body of <c>Unsubscribe()</c>.</summary>
        void AppendUnsubscribe(StringBuilder sb, PropertyObservation observation);

        /// <summary>Gets an expression reading the current value from a typed source variable.</summary>
        string CurrentValue(PropertyObservation observation, string sourceVariable);

        /// <summary>Appends helper members (if any).</summary>
        void AppendHelpers(StringBuilder sb, PropertyObservation observation);
    }
}
