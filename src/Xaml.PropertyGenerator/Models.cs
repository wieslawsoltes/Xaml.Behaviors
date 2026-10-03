// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.PropertyGenerator
{
    /// <summary>The XAML framework the generated code targets.</summary>
    internal enum TargetPlatform
    {
        None = 0,
        Avalonia = 1,
        WinUI = 2,
    }

    /// <summary>The kind of generated property.</summary>
    internal enum PropertyKind
    {
        Styled = 0,
        Direct = 1,
        Attached = 2,
    }

    /// <summary>An immutable array with value equality, required for incremental generator caching.</summary>
    internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
        where T : IEquatable<T>
    {
        private readonly T[]? _items;

        public EquatableArray(T[] items) => _items = items;

        public static EquatableArray<T> Empty => new(Array.Empty<T>());

        public int Count => _items?.Length ?? 0;

        public T this[int index] => _items![index];

        public bool Equals(EquatableArray<T> other)
        {
            var a = _items ?? Array.Empty<T>();
            var b = other._items ?? Array.Empty<T>();
            if (a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

        public override int GetHashCode()
        {
            var hash = 17;
            if (_items is not null)
            {
                foreach (var item in _items)
                {
                    hash = unchecked((hash * 31) + item.GetHashCode());
                }
            }

            return hash;
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)(_items ?? Array.Empty<T>())).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>A containing type declaration (outermost first) used to re-open partial types.</summary>
    internal sealed record TypeDeclarationModel(string Keyword, string Name, string TypeParameters);

    /// <summary>A diagnostic with an equatable location.</summary>
    internal sealed record DiagnosticModel(string Id, string FilePath, TextSpan Span, LinePositionSpan LineSpan, string Argument)
    {
        public Diagnostic ToDiagnostic()
        {
            var descriptor = Diagnostics.Get(Id);
            var location = FilePath.Length == 0 ? Location.None : Location.Create(FilePath, Span, LineSpan);
            return Diagnostic.Create(descriptor, location, Argument);
        }

        public static DiagnosticModel Create(string id, SyntaxNode node, string argument)
        {
            var location = node.GetLocation();
            var lineSpan = location.GetLineSpan();
            return new DiagnosticModel(id, lineSpan.Path ?? string.Empty, location.SourceSpan, lineSpan.Span, argument);
        }
    }

    /// <summary>
    /// Trimming information of a property type, used by the WinUI emitter: WinUI/Uno Platform annotates the property
    /// type parameter of <c>DependencyProperty.Register</c> with <c>[DynamicallyAccessedMembers]</c>.
    /// </summary>
    /// <param name="TypeParameterLevel">
    /// The index (outermost first) of the containing type declaring the type parameter used as property type that must
    /// be annotated, or -1.
    /// </param>
    /// <param name="TypeParameterName">The name of that type parameter.</param>
    /// <param name="HasAnnotatedMembers">
    /// The property type has members with <c>[DynamicallyAccessedMembers]</c> annotations (for example
    /// <see cref="System.Type"/>), which the registration exposes to reflection (IL2111).
    /// </param>
    internal sealed record TrimmingModel(int TypeParameterLevel, string TypeParameterName, bool HasAnnotatedMembers)
    {
        public static TrimmingModel None { get; } = new(-1, string.Empty, false);
    }

    /// <summary>A generated property.</summary>
    internal sealed record PropertyModel(
        PropertyKind Kind,
        string Name,
        string Type,
        string TypeOfType,
        bool IsValueType,
        string Modifiers,
        string FieldAccessibility,
        string GetterModifiers,
        string? SetterModifiers,
        bool SetterIsInit,
        string? DefaultValue,
        int DefaultBindingMode,
        bool Inherits,
        bool Content,
        bool ResolveByName,
        bool AssignBinding,
        bool Lazy,
        bool HasChangedHook,
        string HostType,
        TrimmingModel Trimming,
        bool IsEnum = false);

    /// <summary>All generated properties of one type.</summary>
    internal sealed record TypeModel(
        TargetPlatform Platform,
        string HintName,
        string Namespace,
        EquatableArray<TypeDeclarationModel> ContainingTypes,
        string OwnerType,
        string Usings,
        string? ChangedMethod,
        bool HasTrimmingAttributes,
        EquatableArray<PropertyModel> Properties,
        bool IsNativeWinUI = false);

    /// <summary>The result of analyzing one annotated declaration.</summary>
    internal sealed record CandidateModel(TypeModel? Type, EquatableArray<DiagnosticModel> Diagnostics);

    internal static class EquatableArrayExtensions
    {
        public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> items)
            where T : IEquatable<T>
            => new(new List<T>(items).ToArray());
    }
}
