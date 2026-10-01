using System;
using ReactiveUI;
using ReactiveUI.Binding;
using ReactiveUI.Primitives;

namespace BehaviorsTestApplication.ViewModels;

public class InteractionTriggerBehaviorViewModel : ViewModelBase
{
    public InteractionTriggerBehaviorViewModel()
    {
        TriggerCommand = ReactiveCommand.Create(Trigger);
    }

    public Interaction<RxVoid, RxVoid> TestInteraction { get; } = new();

    public ReactiveCommand<RxVoid, RxVoid> TriggerCommand { get; }

    private void Trigger()
    {
        // ReactiveUI 25: Handle returns a task instead of a cold observable.
        _ = TestInteraction.Handle(RxVoid.Default);
    }
}
