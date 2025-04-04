using System;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class PointerTriggersViewModel : ViewModelBase
{
    public PointerTriggersViewModel()
    {
        PointerPressedCommand = ReactiveCommand.Create<(double X, double Y)>(PointerPressed);
        PointerReleasedCommand = ReactiveCommand.Create<(double X, double Y)>(PointerReleased);
        PointerMovedCommand = ReactiveCommand.Create<(double X, double Y)>(PointerMoved);
    }
    
    public ICommand PointerPressedCommand { get; set; }

    public ICommand PointerReleasedCommand { get; set; }

    public ICommand PointerMovedCommand { get; set; }

    private void PointerPressed((double X, double Y) point)
    {
        Console.WriteLine($"Pressed: {point}");
    }

    private void PointerReleased((double X, double Y) point)
    {
        Console.WriteLine($"Released: {point}");
    }
    
    private void PointerMoved((double X, double Y) point)
    {
        Console.WriteLine($"Moved: {point}");
    }
}
