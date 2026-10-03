// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace Xaml.PortAnalyzers.Json
{
    /// <summary>
    /// The kind of a <see cref="JsonValue"/>.
    /// </summary>
    internal enum JsonValueKind
    {
        /// <summary>The <c>null</c> literal.</summary>
        Null,

        /// <summary>A <c>true</c> or <c>false</c> literal.</summary>
        Boolean,

        /// <summary>A number.</summary>
        Number,

        /// <summary>A string.</summary>
        String,

        /// <summary>An array.</summary>
        Array,

        /// <summary>An object.</summary>
        Object,
    }

    /// <summary>
    /// An immutable JSON value produced by <see cref="JsonParser"/>.
    /// </summary>
    internal sealed class JsonValue
    {
        private JsonValue(JsonValueKind kind, int position, string? text, bool boolean, List<JsonValue>? items, List<KeyValuePair<string, JsonValue>>? properties)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Boolean = boolean;
            Items = items ?? [];
            Properties = properties ?? [];
        }

        /// <summary>Gets the kind of the value.</summary>
        public JsonValueKind Kind { get; }

        /// <summary>Gets the offset of the value in the source text.</summary>
        public int Position { get; }

        /// <summary>Gets the string value, or the raw text of a number.</summary>
        public string? Text { get; }

        /// <summary>Gets the value of a boolean literal.</summary>
        public bool Boolean { get; }

        /// <summary>Gets the items of an array.</summary>
        public IReadOnlyList<JsonValue> Items { get; }

        /// <summary>Gets the properties of an object in declaration order.</summary>
        public IReadOnlyList<KeyValuePair<string, JsonValue>> Properties { get; }

        /// <summary>Creates a <c>null</c> value.</summary>
        public static JsonValue CreateNull(int position) => new(JsonValueKind.Null, position, null, false, null, null);

        /// <summary>Creates a boolean value.</summary>
        public static JsonValue CreateBoolean(bool value, int position) => new(JsonValueKind.Boolean, position, null, value, null, null);

        /// <summary>Creates a number value from its raw text.</summary>
        public static JsonValue CreateNumber(string text, int position) => new(JsonValueKind.Number, position, text, false, null, null);

        /// <summary>Creates a string value.</summary>
        public static JsonValue CreateString(string text, int position) => new(JsonValueKind.String, position, text, false, null, null);

        /// <summary>Creates an array value.</summary>
        public static JsonValue CreateArray(List<JsonValue> items, int position) => new(JsonValueKind.Array, position, null, false, items, null);

        /// <summary>Creates an object value.</summary>
        public static JsonValue CreateObject(List<KeyValuePair<string, JsonValue>> properties, int position) => new(JsonValueKind.Object, position, null, false, null, properties);

        /// <summary>
        /// Gets the last property with the given name of an object, or <see langword="null"/>.
        /// </summary>
        public JsonValue? GetProperty(string name)
        {
            JsonValue? result = null;
            for (var i = 0; i < Properties.Count; i++)
            {
                if (Properties[i].Key == name)
                {
                    result = Properties[i].Value;
                }
            }

            return result;
        }
    }
}
