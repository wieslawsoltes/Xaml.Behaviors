using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that monitors the system clipboard for specific data formats.
/// </summary>
public class ClipboardMonitorBehavior : StyledElementBehavior<Control>
{
    private DispatcherTimer? _timer;
    private bool _hasData;

    /// <summary>
    /// Identifies the <see cref="Formats"/> property.
    /// </summary>
    public static readonly StyledProperty<string> FormatsProperty =
        AvaloniaProperty.Register<ClipboardMonitorBehavior, string>(nameof(Formats), "Text");

    /// <summary>
    /// Identifies the <see cref="HasData"/> property.
    /// </summary>
    public static readonly DirectProperty<ClipboardMonitorBehavior, bool> HasDataProperty =
        AvaloniaProperty.RegisterDirect<ClipboardMonitorBehavior, bool>(
            nameof(HasData),
            o => o.HasData);

    /// <summary>
    /// Gets or sets the comma-separated list of clipboard formats to listen for (e.g., "Text,FileNames").
    /// </summary>
    public string Formats
    {
        get => GetValue(FormatsProperty);
        set => SetValue(FormatsProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the clipboard contains data in any of the specified <see cref="Formats"/>.
    /// </summary>
    public bool HasData
    {
        get => _hasData;
        private set => SetAndRaise(HasDataProperty, ref _hasData, value);
    }

    /// <summary>
    /// Occurs when the clipboard content availability changes.
    /// </summary>
    public event EventHandler? ContentChanged;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.AttachedToVisualTree += AssociatedObject_AttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree += AssociatedObject_DetachedFromVisualTree;
            
            // If already attached to visual tree, start monitoring
            if (AssociatedObject.IsLoaded)
            {
                StartMonitoring();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.AttachedToVisualTree -= AssociatedObject_AttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree -= AssociatedObject_DetachedFromVisualTree;
        }
        StopMonitoring();
        base.OnDetaching();
    }

    private void AssociatedObject_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        StartMonitoring();
    }

    private void AssociatedObject_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        StopMonitoring();
    }

    private void StartMonitoring()
    {
        if (_timer == null)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            CheckClipboard();
        }
    }

    private void StopMonitoring()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _timer = null;
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        CheckClipboard();
    }

    private async void CheckClipboard()
    {
        if (AssociatedObject == null) return;

        var topLevel = TopLevel.GetTopLevel(AssociatedObject);
        if (topLevel?.Clipboard is { } clipboard)
        {
            try
            {
                var formats = await ClipboardExtensions.GetDataFormatsAsync(clipboard);
                var requestedFormats = (Formats ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(f => f.Trim())
                                                      .Where(f => !string.IsNullOrEmpty(f))
                                                      .ToArray();
                var normalizedFormats = formats?.Select(ConvertDataFormatIdentifier).ToArray();

                bool hasData = false;
                if (requestedFormats.Length == 0)
                {
                    // If no formats specified, check if any format exists
                    hasData = normalizedFormats != null && normalizedFormats.Length > 0;
                }
                else
                {
                    hasData = requestedFormats.Any(requestedFormat =>
                        normalizedFormats != null &&
                        normalizedFormats.Any(format => string.Equals(format, NormalizeRequestedFormat(requestedFormat), StringComparison.OrdinalIgnoreCase)));
                }

                if (_hasData != hasData)
                {
                    HasData = hasData;
                    ContentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch
            {
                // Ignore clipboard access errors
            }
        }
    }

    private static string ConvertDataFormatIdentifier(DataFormat format)
    {
        if (DataFormat.Text.Equals(format))
        {
            return TextFormat;
        }

        if (DataFormat.File.Equals(format))
        {
            return FilesFormat;
        }

        return format.Identifier;
    }

    private static string NormalizeRequestedFormat(string format)
    {
        return format.Equals("FileNames", StringComparison.OrdinalIgnoreCase)
            ? FilesFormat
            : format;
    }

    private const string TextFormat = "Text";
    private const string FilesFormat = "Files";
}
