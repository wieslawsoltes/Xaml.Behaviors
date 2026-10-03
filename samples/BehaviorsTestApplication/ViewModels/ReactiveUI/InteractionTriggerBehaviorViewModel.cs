using System;
using ReactiveUI;
using ReactiveUI.Reactive;
using ReactiveUI.Binding.Reactive;
using System.Reactive;

namespace BehaviorsTestApplication.ViewModels;

public class InteractionTriggerBehaviorViewModel : ViewModelBase
{
    public InteractionTriggerBehaviorViewModel()
    {
        TriggerCommand = ReactiveCommand.Create(Trigger);
    }

    public Interaction<Unit, Unit> TestInteraction { get; } = new();

    public ReactiveCommand<Unit, Unit> TriggerCommand { get; }

    private void Trigger()
    {
        // ReactiveUI 25: Handle returns a task instead of a cold observable.
        _ = TestInteraction.Handle(Unit.Default);
    }
}
