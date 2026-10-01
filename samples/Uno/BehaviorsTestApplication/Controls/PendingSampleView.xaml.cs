// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BehaviorsTestApplication.Controls;

/// <summary>
/// Placeholder for a sample page that is not ported to Uno Platform yet.
/// </summary>
/// <remarks>
/// <c>build/UnoPort/generate_sample_mainview.py</c> hosts it in <c>MainView</c> for every page of the Avalonia sample
/// without a WinUI twin, so the Uno sample always shows all pages with the Avalonia headers.
/// </remarks>
public sealed partial class PendingSampleView : UserControl
{
    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(PendingSampleView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Initializes a new instance of the <see cref="PendingSampleView"/> class.
    /// </summary>
    public PendingSampleView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the header of the sample (the tab header of the Avalonia sample).
    /// </summary>
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
