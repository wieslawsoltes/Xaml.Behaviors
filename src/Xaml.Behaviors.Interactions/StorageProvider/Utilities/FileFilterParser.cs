// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.Core;

internal static class FileFilterParser
{
    public static List<FilePickerFileType>? ConvertToFilePickerFileType(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return null;
        }

        var parts = filter.Split('|');
        if (parts.Length % 2 != 0)
        {
            return null;
        }

        var fileTypes = new List<FilePickerFileType>();

        for (var i = 0; i < parts.Length; i += 2)
        {
            var description = parts[i];
            var patternPart = parts[i + 1];
            var index = description.IndexOf(" (", StringComparison.Ordinal);
            if (index > 0)
            {
                description = description.Substring(0, index);
            }

            description = description.Trim();

            var patterns = patternPart
                .Split([';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            fileTypes.Add(new FilePickerFileType(description) { Patterns = patterns });
        }

        return fileTypes;
    }
}
