// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Behaviors.Uno.Headless.Internal;

/// <summary>
/// The minimal application used when <see cref="UnoHeadlessSessionOptions.ApplicationFactory"/> is not set.
/// It creates no window of its own: the session owns the test window.
/// </summary>
internal sealed class UnoHeadlessApplication : Application
{
}
