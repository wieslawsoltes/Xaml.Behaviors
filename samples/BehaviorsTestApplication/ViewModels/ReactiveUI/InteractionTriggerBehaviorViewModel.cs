using System;
#if UNO
// ReactiveUI 25 (ReactiveUI.Uno): Interaction<TInput, TOutput> lives in ReactiveUI.Binding and commands use
// ReactiveUI.Primitives.RxVoid instead of System.Reactive.Unit.
using ReactiveUI.Binding;
using Unit = ReactiveUI.Primitives.RxVoid;
#else
using System.Reactive;
#endif
using ReactiveUI;

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
#if UNO
        // ReactiveUI 25: Handle returns a task instead of a cold observable.
        _ = TestInteraction.Handle(Unit.Default);
#else
        TestInteraction.Handle(Unit.Default).Subscribe();
#endif
    }
}
