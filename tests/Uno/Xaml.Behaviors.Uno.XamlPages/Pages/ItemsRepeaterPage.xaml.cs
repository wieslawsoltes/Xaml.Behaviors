// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace Xaml.Behaviors.Uno.XamlPages.Pages;

public sealed partial class ItemsRepeaterPage : UserControl
{
    public ItemsRepeaterPage()
    {
        InitializeComponent();
    }

    public List<string> Items { get; } = ["a", "b"];
}
