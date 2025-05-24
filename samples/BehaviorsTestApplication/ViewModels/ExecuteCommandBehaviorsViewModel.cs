using System;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class ExecuteCommandBehaviorsViewModel : ViewModelBase
{
    public ExecuteCommandBehaviorsViewModel()
    {
        PointerEnteredCommand = ReactiveCommand.Create(() => Console.WriteLine("PointerEntered"));
        PointerPressedCommand = ReactiveCommand.Create(() => Console.WriteLine("PointerPressed"));
        DoubleTappedCommand = ReactiveCommand.Create(() => Console.WriteLine("DoubleTapped"));
        KeyDownCommand = ReactiveCommand.Create(() => Console.WriteLine("KeyDown"));
    }

    public ICommand PointerEnteredCommand { get; }
    public ICommand PointerPressedCommand { get; }
    public ICommand DoubleTappedCommand { get; }
    public ICommand KeyDownCommand { get; }
}
