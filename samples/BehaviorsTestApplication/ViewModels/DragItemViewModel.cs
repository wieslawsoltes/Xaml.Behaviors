using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class DragItemViewModel : ViewModelBase
{
    private string? _title;

    public string? Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public override string? ToString() => _title;
}
