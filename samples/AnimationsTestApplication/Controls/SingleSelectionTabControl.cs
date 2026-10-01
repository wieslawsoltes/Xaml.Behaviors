// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

namespace AnimationsTestApplication.Controls;

#if UNO
/// <summary>
/// The WinUI tab control (<see cref="TabView"/>) with the sample defaults. TabView only supports single selection;
/// the vertical sidebar look comes from the <c>SideBarTabControlStyle</c> template (SideBar.xaml).
/// </summary>
public class SingleSelectionTabControl : TabView
{
    public SingleSelectionTabControl()
    {
        // Counterpart of StyleKeyOverride: use the TabView default style.
        DefaultStyleKey = typeof(TabView);
        IsAddTabButtonVisible = false;
        CanDragTabs = false;
        CanReorderTabs = false;
    }
}
#else
public class SingleSelectionTabControl : TabControl
{
    protected override Type StyleKeyOverride => typeof(TabControl);

    static SingleSelectionTabControl()
    {
        SelectionModeProperty.OverrideDefaultValue<SingleSelectionTabControl>(SelectionMode.Single);
    }
}
#endif
