// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BehaviorsTestApplication.Controls;

/// <summary>
/// Shows that a sample is not available on Uno Platform and why.
/// </summary>
/// <remarks>
/// The WinUI twin of a sample page whose behavior has no WinUI counterpart (see the status table of
/// <c>src/Uno/PORTING.md</c>) uses it as its content:
/// <code>
/// &lt;controls:NotAvailableOnUnoView Title="WindowDragMoveBehavior"
///                                 Reason="Window.BeginMoveDrag has no WinUI counterpart." /&gt;
/// </code>
/// </remarks>
public sealed partial class NotAvailableOnUnoView : UserControl
{
    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(NotAvailableOnUnoView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="Reason"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ReasonProperty =
        DependencyProperty.Register(nameof(Reason), typeof(string), typeof(NotAvailableOnUnoView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Initializes a new instance of the <see cref="NotAvailableOnUnoView"/> class.
    /// </summary>
    public NotAvailableOnUnoView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the title of the sample (the tab header of the Avalonia sample).
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets why the sample is not available on Uno Platform (the missing WinUI counterpart).
    /// </summary>
    public string Reason
    {
        get => (string)GetValue(ReasonProperty);
        set => SetValue(ReasonProperty, value);
    }
}
