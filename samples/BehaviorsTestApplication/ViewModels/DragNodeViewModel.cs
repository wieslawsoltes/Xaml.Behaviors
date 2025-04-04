using System.Collections.ObjectModel;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class DragNodeViewModel : ViewModelBase
{
    private string? _title;
    private DragNodeViewModel? _parent;
    private ObservableCollection<DragNodeViewModel>? _nodes;

    public string? Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public DragNodeViewModel? Parent
    {
        get => _parent;
        set => this.RaiseAndSetIfChanged(ref _parent, value);
    }

    public ObservableCollection<DragNodeViewModel>? Nodes
    {
        get => _nodes;
        set => this.RaiseAndSetIfChanged(ref _nodes, value);
    }

    public override string? ToString() => _title;
}
