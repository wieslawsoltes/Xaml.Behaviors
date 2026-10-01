using System.Collections.ObjectModel;
using System.IO;
#if UNO
// ReactiveUI 25 (ReactiveUI.Uno) commands use ReactiveUI.Primitives.RxVoid instead of System.Reactive.Unit.
using Unit = ReactiveUI.Primitives.RxVoid;
#else
using System.Reactive;
#endif
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class FileSystemViewModel : ViewModelBase
{
    private string? _targetPath;
    private string? _fileContent;
    private ObservableCollection<string> _logs = new();

    public string? TargetPath
    {
        get => _targetPath;
        set => this.RaiseAndSetIfChanged(ref _targetPath, value);
    }

    public string? FileContent
    {
        get => _fileContent;
        set => this.RaiseAndSetIfChanged(ref _fileContent, value);
    }

    public ObservableCollection<string> Logs
    {
        get => _logs;
        set => this.RaiseAndSetIfChanged(ref _logs, value);
    }

    public ReactiveCommand<object?, Unit> LogEventCommand { get; }

    public FileSystemViewModel()
    {
        TargetPath = Path.Combine(Path.GetTempPath(), "AvaloniaBehaviorsTest");
        FileContent = "Hello Avalonia Behaviors!";
        LogEventCommand = ReactiveCommand.Create<object?>(LogEvent);
    }

    private void LogEvent(object? args)
    {
        if (args is FileSystemEventArgs e)
        {
            Logs.Insert(0, $"{e.ChangeType}: {e.FullPath}");
        }
        else if (args is RenamedEventArgs r)
        {
            Logs.Insert(0, $"{r.ChangeType}: {r.OldFullPath} -> {r.FullPath}");
        }
        else
        {
            Logs.Insert(0, $"Event: {args}");
        }
    }
}
