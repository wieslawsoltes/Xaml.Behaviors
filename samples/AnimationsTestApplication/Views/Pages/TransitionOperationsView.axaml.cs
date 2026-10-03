// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

namespace AnimationsTestApplication.Views.Pages;

public partial class TransitionOperationsView : UserControl
{
    public TransitionOperationsView()
    {
        InitializeComponent();
    }
}
