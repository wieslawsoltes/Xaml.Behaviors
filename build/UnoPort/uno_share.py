#!/usr/bin/env python3
# Copyright (c) Wiesław Šoltés. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for details.
"""
Mechanical helper used to prepare Avalonia-first sources for sharing with the Uno Platform port.

The transformations are idempotent and keep the Avalonia build byte-for-byte equivalent
(everything added for Uno lives inside `#if UNO` blocks):

  * namespace    `namespace Avalonia.Xaml.X;`  ->  `#if UNO namespace Xaml.X; #else ... #endif`
  * usings       `using Avalonia.*;` lines are wrapped in `#if UNO <winui usings> #else ... #endif`
  * properties   `static readonly StyledProperty<T>/DirectProperty<,>/AttachedProperty<T> XProperty =`
                 get an Uno twin declared as `DependencyProperty` (the registration expression is shared,
                 see src/Uno/Xaml.Behaviors.Interactivity/Compat/AvaloniaProperty.cs)
  * getters      `get => GetValue(XProperty);` -> `get => (T)GetValue(XProperty);`
  * partial      classes deriving directly from AvaloniaObject become `partial` (required by Uno's
                 DependencyObject source generator)

Usage: uno_share.py <file-or-directory> [...]
"""
import os
import re
import sys

USING_MAP = {
    "Avalonia": ["Microsoft.UI.Xaml"],
    "Avalonia.Controls": ["Microsoft.UI.Xaml", "Microsoft.UI.Xaml.Controls"],
    "Avalonia.Controls.Primitives": ["Microsoft.UI.Xaml.Controls.Primitives"],
    "Avalonia.Input": ["Microsoft.UI.Xaml.Input"],
    "Avalonia.Interactivity": ["Microsoft.UI.Xaml"],
    "Avalonia.Media": ["Microsoft.UI.Xaml.Media"],
    "Avalonia.Media.Imaging": ["Microsoft.UI.Xaml.Media.Imaging"],
    "Avalonia.Data": ["Microsoft.UI.Xaml.Data"],
    "Avalonia.Data.Converters": ["Microsoft.UI.Xaml.Data"],
    "Avalonia.Threading": ["Microsoft.UI.Dispatching"],
    "Avalonia.Animation": ["Microsoft.UI.Xaml.Media.Animation"],
    "Avalonia.Layout": ["Microsoft.UI.Xaml"],
    "Avalonia.Markup.Xaml": ["Microsoft.UI.Xaml.Markup"],
}

PROPERTY_DECL = re.compile(
    r"^(?P<indent>[ \t]*)(?P<mods>(?:public|internal|private|protected)(?:\s+(?:static|readonly|new))+\s+)"
    r"(?P<type>(?:StyledProperty|AttachedProperty)<.+?>|DirectProperty<.+?>)"
    r"(?P<rest>\s+\w+\s*=.*)$",
    re.M,
)


def map_namespace(ns):
    return ns[len("Avalonia."):] if ns.startswith("Avalonia.Xaml.") else ns


def transform_namespace(text):
    m = re.search(r"^namespace\s+(Avalonia\.Xaml\.[\w.]+)\s*;[ \t]*$", text, re.M)
    if not m:
        return text
    before = text[: m.start()].rstrip()
    if before.endswith("#else") or before.endswith("#if UNO"):
        return text
    ns = m.group(1)
    block = f"#if UNO\nnamespace {map_namespace(ns)};\n#else\nnamespace {ns};\n#endif"
    return text[: m.start()] + block + text[m.end():]


def transform_usings(text):
    lines = text.split("\n")
    idx = [i for i, l in enumerate(lines) if re.match(r"^using\s+(static\s+)?Avalonia[\w.]*\s*;\s*$", l)]
    if not idx:
        return text
    # Already processed?
    first = idx[0]
    if first > 0 and lines[first - 1].strip() in ("#else", "#if !UNO"):
        return text
    ava = [lines[i].strip() for i in idx]
    uno = []
    for u in ava:
        name = re.match(r"^using\s+(?:static\s+)?([\w.]+)", u).group(1)
        if name.startswith("Avalonia.Xaml."):
            targets = [map_namespace(name)]
        else:
            targets = USING_MAP.get(name, [])
        for t in targets:
            if t not in uno:
                uno.append(t)
    for i in reversed(idx):
        del lines[i]
    block = ["#if UNO"] + [f"using {u};" for u in uno] + ["#else"] + ava + ["#endif"]
    lines[first:first] = block
    return "\n".join(lines)


def transform_properties(text):
    def repl(m):
        line = m.group(0)
        prev = text[: m.start()].rstrip("\n").split("\n")[-1].strip()
        if prev == "#else":
            return line
        uno_line = f"{m.group('indent')}{m.group('mods')}DependencyProperty{m.group('rest')}"
        return f"#if UNO\n{uno_line}\n#else\n{line}\n#endif"

    return PROPERTY_DECL.sub(repl, text)


GETTER = re.compile(
    r"(?P<head>(?:public|protected|internal|private)\s+(?:(?:new|override|virtual|static|sealed)\s+)*"
    r"(?P<type>[\w<>?,.\[\]]+(?:<[^>{}]+>)?\??)\s+(?P<name>\w+)\s*\{\s*get\s*=>\s*)GetValue\((?P<prop>\w+)\)",
)

ATTACHED_GETTER = re.compile(
    r"(?P<head>static\s+(?P<type>[\w<>?,.\[\]]+\??)\s+Get\w+\([^)]*\)\s*(?:=>|\{\s*return)\s*)(?P<obj>\w+)\.GetValue\("
)


def transform_getters(text):
    def repl(m):
        t = m.group("type")
        return f"{m.group('head')}({t})GetValue({m.group('prop')})"

    text = GETTER.sub(repl, text)

    def repl2(m):
        t = m.group("type")
        if t in ("void", "object", "object?"):
            return m.group(0)
        return f"{m.group('head')}({t}){m.group('obj')}.GetValue("

    return ATTACHED_GETTER.sub(repl2, text)


def transform_partial(text):
    return re.sub(
        r"^(?P<pre>[ \t]*(?:public|internal)\s+(?:(?:abstract|sealed|static)\s+)*)class(?P<post>\s+\w+(?:<[^>]+>)?\s*:\s*AvaloniaObject\b)",
        lambda m: m.group(0) if "partial" in m.group("pre") else f"{m.group('pre')}partial class{m.group('post')}",
        text,
        flags=re.M,
    )


def ensure_uno_xaml_using(text):
    """Files declaring Uno dependency properties need Microsoft.UI.Xaml even without Avalonia usings."""
    if "DependencyProperty" not in text or "using Microsoft.UI.Xaml;" in text:
        return text
    m = re.search(r"^#if UNO\n((?:using [\w.]+;\n)*)#else\nusing Avalonia", text, re.M)
    if m:
        pos = m.start() + len("#if UNO\n")
        return text[:pos] + "using Microsoft.UI.Xaml;\n" + text[pos:]
    m = re.search(r"^#if UNO\nnamespace ", text, re.M)
    if not m:
        return text
    block = "#if UNO\nusing Microsoft.UI.Xaml;\n#endif\n\n"
    return text[: m.start()] + block + text[m.start():]


def cleanup(text):
    return text.replace("#if UNO\n#else\n", "#if !UNO\n")


def process(path):
    raw = open(path, "rb").read()
    bom = raw.startswith(b"\xef\xbb\xbf")
    text = raw.decode("utf-8-sig")
    crlf = "\r\n" in text
    if crlf:
        text = text.replace("\r\n", "\n")
    new = cleanup(ensure_uno_xaml_using(transform_partial(transform_getters(transform_properties(transform_usings(transform_namespace(text)))))))
    if new == text:
        return False
    if crlf:
        new = new.replace("\n", "\r\n")
    with open(path, "wb") as f:
        f.write((b"\xef\xbb\xbf" if bom else b"") + new.encode("utf-8"))
    return True


def main(args):
    changed = 0
    for a in args:
        if os.path.isdir(a):
            for root, dirs, files in os.walk(a):
                dirs[:] = [d for d in dirs if d not in ("bin", "obj")]
                for f in files:
                    if f.endswith(".cs") and process(os.path.join(root, f)):
                        changed += 1
        elif process(a):
            changed += 1
    print(f"changed {changed} file(s)")


if __name__ == "__main__":
    main(sys.argv[1:])
