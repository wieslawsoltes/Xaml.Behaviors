using System.Reactive;
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
        TestInteraction.Handle(Unit.Default).Subscribe();
    }
}
