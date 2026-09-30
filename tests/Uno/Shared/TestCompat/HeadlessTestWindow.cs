// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Behaviors.Uno.Headless;

namespace Xaml.Behaviors.Uno.TestCompat;

/// <summary>
/// Uno Platform counterpart of the Avalonia headless test <c>Window</c> used by the shared tests (the <c>Window</c>
/// alias of the Uno test projects): a content control shown as the content of the headless session window.
/// </summary>
public partial class HeadlessTestWindow : ContentControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeadlessTestWindow"/> class.
    /// </summary>
    public HeadlessTestWindow()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
    }

    /// <summary>
    /// Occurs when the window was shown.
    /// </summary>
    public event EventHandler? Opened;

    /// <summary>
    /// Occurs when the window was closed.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Gets or sets the window title (not displayed).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets a value indicating whether the window is shown.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Shows the window: makes it the content of the session window and runs the queued UI work until it is loaded.
    /// </summary>
    public void Show()
    {
        // Like a new Avalonia window, the window starts a separate input sequence: clicks of a previous test at the
        // same position must not continue into a double tap.
        UnoHeadlessSession.Current.Mouse.Wait(TimeSpan.FromSeconds(1));
        UnoHeadlessSession.Current.Show(this);
        IsOpen = true;
        Opened?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Activates (shows) the window.
    /// </summary>
    public void Activate()
    {
        if (!IsOpen)
        {
            Show();
        }
    }

    /// <summary>
    /// Closes the window: removes it from the session window and runs the queued UI work (unloaded events).
    /// </summary>
    public void Close()
    {
        var session = UnoHeadlessSession.Current;
        if (ReferenceEquals(session.Window.Content, this))
        {
            session.Window.Content = new Grid();
        }

        session.RunJobs();
        if (IsOpen)
        {
            IsOpen = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
