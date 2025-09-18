using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class ExecuteCommandBehaviorsViewModel : ViewModelBase
{
    public ExecuteCommandBehaviorsViewModel()
    {
        Events = new ObservableCollection<string>();

        ActivatedCommand = ReactiveCommand.Create(() => Log("Window activated"));

        GotFocusCommand = ReactiveCommand.Create(() => Log("Got focus"));
        LostFocusCommand = ReactiveCommand.Create(() => Log("Lost focus"));

        PointerEnteredCommand = ReactiveCommand.Create(() => Log("Pointer entered"));
        PointerPressedCommand = ReactiveCommand.Create(() => Log("Pointer pressed"));
        PointerMovedCommand = ReactiveCommand.Create(() => Log("Pointer moved"));
        PointerExitedCommand = ReactiveCommand.Create(() => Log("Pointer exited"));
        PointerReleasedCommand = ReactiveCommand.Create(() => Log("Pointer released"));
        PointerCaptureLostCommand = ReactiveCommand.Create(() => Log("Pointer capture lost"));
        PointerWheelChangedCommand = ReactiveCommand.Create(() => Log("Pointer wheel changed"));

        HoldingCommand = ReactiveCommand.Create(() => Log("Holding gesture"));
        TappedCommand = ReactiveCommand.Create(() => Log("Tapped"));
        DoubleTappedCommand = ReactiveCommand.Create(() => Log("Double tapped"));
        RightTappedCommand = ReactiveCommand.Create(() => Log("Right tapped"));
        PullCommand = ReactiveCommand.Create(() => Log("Pull gesture"));
        PullEndedCommand = ReactiveCommand.Create(() => Log("Pull gesture ended"));
        ScrollGestureCommand = ReactiveCommand.Create(() => Log("Scroll gesture"));
        ScrollGestureEndedCommand = ReactiveCommand.Create(() => Log("Scroll gesture ended"));
        ScrollGestureInertiaCommand = ReactiveCommand.Create(() => Log("Scroll inertia starting"));
        TouchPadMagnifyCommand = ReactiveCommand.Create(() => Log("Touchpad magnify"));
        TouchPadRotateCommand = ReactiveCommand.Create(() => Log("Touchpad rotate"));
        TouchPadSwipeCommand = ReactiveCommand.Create(() => Log("Touchpad swipe"));

        KeyDownCommand = ReactiveCommand.Create(() => Log("Key down"));
        KeyUpCommand = ReactiveCommand.Create(() => Log("Key up"));
        TextInputCommand = ReactiveCommand.Create(() => Log("Text input"));
        TextInputMethodRequestedCommand = ReactiveCommand.Create(() => Log("Text input method client requested"));

        ClearLogCommand = ReactiveCommand.Create(() => Events.Clear());
    }

    public ObservableCollection<string> Events { get; }

    public ICommand ActivatedCommand { get; }

    public ICommand GotFocusCommand { get; }
    public ICommand LostFocusCommand { get; }

    public ICommand PointerEnteredCommand { get; }
    public ICommand PointerPressedCommand { get; }
    public ICommand PointerMovedCommand { get; }
    public ICommand PointerExitedCommand { get; }
    public ICommand PointerReleasedCommand { get; }
    public ICommand PointerCaptureLostCommand { get; }
    public ICommand PointerWheelChangedCommand { get; }

    public ICommand HoldingCommand { get; }
    public ICommand TappedCommand { get; }
    public ICommand DoubleTappedCommand { get; }
    public ICommand RightTappedCommand { get; }
    public ICommand PullCommand { get; }
    public ICommand PullEndedCommand { get; }
    public ICommand ScrollGestureCommand { get; }
    public ICommand ScrollGestureEndedCommand { get; }
    public ICommand ScrollGestureInertiaCommand { get; }
    public ICommand TouchPadMagnifyCommand { get; }
    public ICommand TouchPadRotateCommand { get; }
    public ICommand TouchPadSwipeCommand { get; }

    public ICommand KeyDownCommand { get; }
    public ICommand KeyUpCommand { get; }
    public ICommand TextInputCommand { get; }

    public ICommand TextInputMethodRequestedCommand { get; }

    public ICommand ClearLogCommand { get; }

    private void Log(string message)
    {
        var entry = $"{DateTime.Now:HH:mm:ss} - {message}";
        Events.Insert(0, entry);

        const int maxEntries = 50;
        if (Events.Count > maxEntries)
        {
            Events.RemoveAt(Events.Count - 1);
        }
    }
}
