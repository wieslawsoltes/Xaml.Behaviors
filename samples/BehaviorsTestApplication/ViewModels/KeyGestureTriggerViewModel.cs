using System;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public partial class KeyGestureTriggerViewModel : ViewModelBase
{
    public KeyGestureTriggerViewModel()
    {
        TriggerCommand = ReactiveCommand.Create(OnTriggered);
    }

    public ICommand TriggerCommand { get; }

    private void OnTriggered()
    {
        Console.WriteLine("KeyGestureTrigger fired");
    }
}
