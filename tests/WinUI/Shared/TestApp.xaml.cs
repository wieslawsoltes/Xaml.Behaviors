// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Behaviors.WinUI.TestCompat;

/// <summary>
/// The application of the WinUI test session of a test project (see <see cref="WinUITestModule"/>).
/// </summary>
public partial class TestApp : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestApp"/> class.
    /// </summary>
    public TestApp()
    {
        InitializeComponent();
    }
}
