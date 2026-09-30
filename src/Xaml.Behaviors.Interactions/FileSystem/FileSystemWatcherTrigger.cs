using System;
using System.IO;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.FileSystem;
#else
namespace Avalonia.Xaml.Interactions.FileSystem;
#endif

/// <summary>
/// A trigger that listens to file system events.
/// </summary>
public partial class FileSystemWatcherTrigger : Trigger
{
    private FileSystemWatcher? _watcher;

    /// <summary>
    /// Gets or sets the path to watch.
    /// </summary>
    [StyledProperty]
    public partial string? Path { get; set; }

    /// <summary>
    /// Gets or sets the filter string used to determine what files are monitored in a directory.
    /// </summary>
    [StyledProperty(DefaultValue = "*.*")]
    public partial string? Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
    /// </summary>
    [StyledProperty]
    public partial bool IncludeSubdirectories { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        StartWatcher();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        StopWatcher();
        base.OnDetaching();
    }

    private void StartWatcher()
    {
        if (_watcher != null)
        {
            return;
        }

        var path = Path;
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            return;
        }

        _watcher = new FileSystemWatcher(path);
        var filter = Filter;
        if (!string.IsNullOrEmpty(filter))
        {
            _watcher.Filter = filter;
        }
        _watcher.IncludeSubdirectories = IncludeSubdirectories;
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnRenamed;

        _watcher.EnableRaisingEvents = true;
    }

    private void StopWatcher()
    {
        if (_watcher == null)
        {
            return;
        }

        _watcher.Changed -= OnChanged;
        _watcher.Created -= OnChanged;
        _watcher.Deleted -= OnChanged;
        _watcher.Renamed -= OnRenamed;
        _watcher.Dispose();
        _watcher = null;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, e);
        });
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, e);
        });
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PathProperty || 
            change.Property == FilterProperty || 
            change.Property == IncludeSubdirectoriesProperty)
        {
            StopWatcher();
            StartWatcher();
        }
    }
}
