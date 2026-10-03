// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xaml.PortAnalyzers.Json
{
    /// <summary>
    /// A small, dependency-free JSON parser (RFC 8259) that additionally accepts <c>//</c> and <c>/* */</c> comments.
    /// </summary>
    /// <remarks>
    /// Analyzers cannot rely on System.Text.Json being available in every compiler host, so the port map is read
    /// with this parser instead.
    /// </remarks>
    internal sealed class JsonParser
    {
        private const int MaxDepth = 64;

        private readonly string _text;
        private int _position;
        private int _depth;

        private JsonParser(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Parses a complete JSON document.
        /// </summary>
        /// <exception cref="JsonParseException">The text is not valid JSON.</exception>
        public static JsonValue Parse(string text)
        {
            var parser = new JsonParser(text);
            parser.SkipByteOrderMark();
            parser.SkipTrivia();
            var value = parser.ParseValue();
            parser.SkipTrivia();
            if (parser._position < text.Length)
            {
                throw new JsonParseException("Unexpected character '" + text[parser._position] + "' after the end of the document", parser._position);
            }

            return value;
        }

        private void SkipByteOrderMark()
        {
            if (_text.Length > 0 && _text[0] == '﻿')
            {
                _position = 1;
            }
        }

        private JsonValue ParseValue()
        {
            if (_position >= _text.Length)
            {
                throw new JsonParseException("Unexpected end of the document", _position);
            }

            var c = _text[_position];
            switch (c)
            {
                case '{':
                    return ParseObject();
                case '[':
                    return ParseArray();
                case '"':
                    var start = _position;
                    return JsonValue.CreateString(ParseString(), start);
                case 't':
                    return ParseLiteral("true", JsonValue.CreateBoolean(true, _position));
                case 'f':
                    return ParseLiteral("false", JsonValue.CreateBoolean(false, _position));
                case 'n':
                    return ParseLiteral("null", JsonValue.CreateNull(_position));
                default:
                    if (c == '-' || (c >= '0' && c <= '9'))
                    {
                        return ParseNumber();
                    }

                    throw new JsonParseException("Unexpected character '" + c + "'", _position);
            }
        }

        private JsonValue ParseObject()
        {
            var start = _position;
            EnterNested();
            _position++;
            var properties = new List<KeyValuePair<string, JsonValue>>();
            SkipTrivia();
            if (TryConsume('}'))
            {
                _depth--;
                return JsonValue.CreateObject(properties, start);
            }

            while (true)
            {
                SkipTrivia();
                if (_position >= _text.Length || _text[_position] != '"')
                {
                    throw new JsonParseException("Expected a property name", _position);
                }

                var name = ParseString();
                SkipTrivia();
                Expect(':');
                SkipTrivia();
                var value = ParseValue();
                properties.Add(new KeyValuePair<string, JsonValue>(name, value));
                SkipTrivia();
                if (TryConsume(','))
                {
                    continue;
                }

                Expect('}');
                _depth--;
                return JsonValue.CreateObject(properties, start);
            }
        }

        private JsonValue ParseArray()
        {
            var start = _position;
            EnterNested();
            _position++;
            var items = new List<JsonValue>();
            SkipTrivia();
            if (TryConsume(']'))
            {
                _depth--;
                return JsonValue.CreateArray(items, start);
            }

            while (true)
            {
                SkipTrivia();
                items.Add(ParseValue());
                SkipTrivia();
                if (TryConsume(','))
                {
                    continue;
                }

                Expect(']');
                _depth--;
                return JsonValue.CreateArray(items, start);
            }
        }

        private string ParseString()
        {
            var start = _position;
            _position++;
            StringBuilder? builder = null;
            var runStart = _position;
            while (true)
            {
                if (_position >= _text.Length)
                {
                    throw new JsonParseException("Unterminated string", start);
                }

                var c = _text[_position];
                if (c == '"')
                {
                    var result = builder is null
                        ? _text.Substring(runStart, _position - runStart)
                        : builder.Append(_text, runStart, _position - runStart).ToString();
                    _position++;
                    return result;
                }

                if (c < ' ')
                {
                    throw new JsonParseException("Control characters must be escaped in strings", _position);
                }

                if (c != '\\')
                {
                    _position++;
                    continue;
                }

                builder ??= new StringBuilder();
                builder.Append(_text, runStart, _position - runStart);
                _position++;
                if (_position >= _text.Length)
                {
                    throw new JsonParseException("Unterminated string", start);
                }

                var escape = _text[_position];
                switch (escape)
                {
                    case '"': builder.Append('"'); break;
                    case '\\': builder.Append('\\'); break;
                    case '/': builder.Append('/'); break;
                    case 'b': builder.Append('\b'); break;
                    case 'f': builder.Append('\f'); break;
                    case 'n': builder.Append('\n'); break;
                    case 'r': builder.Append('\r'); break;
                    case 't': builder.Append('\t'); break;
                    case 'u':
                        if (_position + 4 >= _text.Length
                            || !int.TryParse(_text.Substring(_position + 1, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var code))
                        {
                            throw new JsonParseException("Invalid unicode escape sequence", _position - 1);
                        }

                        builder.Append((char)code);
                        _position += 4;
                        break;
                    default:
                        throw new JsonParseException("Invalid escape sequence '\\" + escape + "'", _position - 1);
                }

                _position++;
                runStart = _position;
            }
        }

        private JsonValue ParseNumber()
        {
            var start = _position;
            TryConsume('-');
            if (TryConsume('0'))
            {
                // A leading zero cannot be followed by more digits.
            }
            else if (!ConsumeDigits())
            {
                throw new JsonParseException("Invalid number", start);
            }

            if (TryConsume('.') && !ConsumeDigits())
            {
                throw new JsonParseException("Invalid number", start);
            }

            if (_position < _text.Length && (_text[_position] == 'e' || _text[_position] == 'E'))
            {
                _position++;
                if (!TryConsume('+'))
                {
                    TryConsume('-');
                }

                if (!ConsumeDigits())
                {
                    throw new JsonParseException("Invalid number", start);
                }
            }

            if (_position < _text.Length && IsIdentifierChar(_text[_position]))
            {
                throw new JsonParseException("Invalid number", start);
            }

            return JsonValue.CreateNumber(_text.Substring(start, _position - start), start);
        }

        private JsonValue ParseLiteral(string literal, JsonValue value)
        {
            if (string.CompareOrdinal(_text, _position, literal, 0, literal.Length) != 0
                || (_position + literal.Length < _text.Length && IsIdentifierChar(_text[_position + literal.Length])))
            {
                throw new JsonParseException("Unexpected token", _position);
            }

            _position += literal.Length;
            return value;
        }

        private bool ConsumeDigits()
        {
            var start = _position;
            while (_position < _text.Length && _text[_position] >= '0' && _text[_position] <= '9')
            {
                _position++;
            }

            return _position > start;
        }

        private void SkipTrivia()
        {
            while (_position < _text.Length)
            {
                var c = _text[_position];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    _position++;
                }
                else if (c == '/' && _position + 1 < _text.Length && _text[_position + 1] == '/')
                {
                    _position += 2;
                    while (_position < _text.Length && _text[_position] != '\n')
                    {
                        _position++;
                    }
                }
                else if (c == '/' && _position + 1 < _text.Length && _text[_position + 1] == '*')
                {
                    var start = _position;
                    var end = _text.IndexOf("*/", _position + 2, System.StringComparison.Ordinal);
                    if (end < 0)
                    {
                        throw new JsonParseException("Unterminated comment", start);
                    }

                    _position = end + 2;
                }
                else
                {
                    return;
                }
            }
        }

        private bool TryConsume(char c)
        {
            if (_position < _text.Length && _text[_position] == c)
            {
                _position++;
                return true;
            }

            return false;
        }

        private void Expect(char c)
        {
            if (!TryConsume(c))
            {
                throw new JsonParseException(
                    _position < _text.Length ? "Expected '" + c + "' but found '" + _text[_position] + "'" : "Expected '" + c + "' but reached the end of the document",
                    _position);
            }
        }

        private void EnterNested()
        {
            if (++_depth > MaxDepth)
            {
                throw new JsonParseException("The document is nested too deeply", _position);
            }
        }

        private static bool IsIdentifierChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '.';
    }
}
