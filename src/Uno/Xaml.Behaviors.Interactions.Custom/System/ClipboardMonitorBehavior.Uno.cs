// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace Xaml.Interactions.Custom;

/// <content>
/// WinUI clipboard access of <see cref="ClipboardMonitorBehavior"/>.
/// </content>
public partial class ClipboardMonitorBehavior
{
    /// <summary>
    /// Gets the formats available on the application clipboard (WinUI identifiers, see <see cref="StandardDataFormats"/>).
    /// </summary>
    private static Task<IReadOnlyList<string>?> GetDataFormatsAsync()
        => Task.FromResult<IReadOnlyList<string>?>(Clipboard.GetContent()?.AvailableFormats);
}
