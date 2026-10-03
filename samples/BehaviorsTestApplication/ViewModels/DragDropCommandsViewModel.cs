using System.Windows.Input;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
#endif
using ReactiveUI;
using ReactiveUI.Reactive;

namespace BehaviorsTestApplication.ViewModels;

public class DragDropCommandsViewModel : ViewModelBase
{
    private string _status = "Waiting";

    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public ICommand EnterCommand { get; }
    public ICommand OverCommand { get; }
    public ICommand LeaveCommand { get; }
    public ICommand DropCommand { get; }

    public DragDropCommandsViewModel()
    {
        EnterCommand = ReactiveCommand.Create<DragEventArgs>(_ => Status = "DragEnter");
        OverCommand = ReactiveCommand.Create<DragEventArgs>(_ => Status = "DragOver");
        LeaveCommand = ReactiveCommand.Create<DragEventArgs>(_ => Status = "DragLeave");
        DropCommand = ReactiveCommand.Create<DragEventArgs>(_ => Status = "Dropped");
    }
}
