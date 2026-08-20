// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Avalonia.Controls;

namespace AnimationsTestApplication.Controls;

public class SingleSelectionTabControl : TabControl
{
    protected override Type StyleKeyOverride => typeof(TabControl);

    static SingleSelectionTabControl()
    {
        SelectionModeProperty.OverrideDefaultValue<SingleSelectionTabControl>(SelectionMode.Single);
    }
}
