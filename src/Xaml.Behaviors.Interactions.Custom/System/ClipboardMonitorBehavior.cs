using System;
using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
using Windows.ApplicationModel.DataTransfer;
using VisualTreeAttachmentEventArgs = Microsoft.UI.Xaml.RoutedEventArgs;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that monitors the system clipboard for specific data formats.
/// </summary>
public partial class ClipboardMonitorBehavior : StyledElementBehavior<Control>
{
    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the comma-separated list of clipboard formats to listen for (e.g., "Text,FileNames").
    /// </summary>
    [StyledProperty(DefaultValue = "Text")]
    public partial string Formats { get; set; }

    /// <summary>
    /// Gets a value indicating whether the clipboard contains data in any of the specified <see cref="Formats"/>.
    /// </summary>
    [DirectProperty]
    public partial bool HasData { get; private set; }

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
#if UNO
            // Read before the Loaded handler is added (see LoadedState).
            var isLoaded = LoadedState.IsLoaded(AssociatedObject);
            AssociatedObject.Loaded += AssociatedObject_AttachedToVisualTree;
            AssociatedObject.Unloaded += AssociatedObject_DetachedFromVisualTree;
#else
            AssociatedObject.AttachedToVisualTree += AssociatedObject_AttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree += AssociatedObject_DetachedFromVisualTree;
#endif
            
            // If already attached to visual tree, start monitoring
#if UNO
            if (isLoaded)
#else
            if (AssociatedObject.IsLoaded)
#endif
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
#if UNO
            AssociatedObject.Loaded -= AssociatedObject_AttachedToVisualTree;
            AssociatedObject.Unloaded -= AssociatedObject_DetachedFromVisualTree;
#else
            AssociatedObject.AttachedToVisualTree -= AssociatedObject_AttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree -= AssociatedObject_DetachedFromVisualTree;
#endif
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

#if UNO
    private void Timer_Tick(object? sender, object e)
#else
    private void Timer_Tick(object? sender, EventArgs e)
#endif
    {
        CheckClipboard();
    }

    private async void CheckClipboard()
    {
        if (AssociatedObject == null) return;

#if UNO
        // WinUI has a single application clipboard (Windows.ApplicationModel.DataTransfer.Clipboard).
        if (AssociatedObject.XamlRoot is not null)
        {
            try
            {
                var formats = await GetDataFormatsAsync();
#else
        var topLevel = TopLevel.GetTopLevel(AssociatedObject);
        if (topLevel?.Clipboard is { } clipboard)
        {
            try
            {
                var formats = await ClipboardExtensions.GetDataFormatsAsync(clipboard);
#endif
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

                if (HasData != hasData)
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

#if UNO
    private static string ConvertDataFormatIdentifier(string format)
    {
        if (StandardDataFormats.Text.Equals(format, StringComparison.Ordinal))
        {
            return TextFormat;
        }

        if (StandardDataFormats.StorageItems.Equals(format, StringComparison.Ordinal))
        {
            return FilesFormat;
        }

        return format;
    }
#else
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
#endif

    private static string NormalizeRequestedFormat(string format)
    {
        return format.Equals("FileNames", StringComparison.OrdinalIgnoreCase)
            ? FilesFormat
            : format;
    }

    private const string TextFormat = "Text";
    private const string FilesFormat = "Files";
}
