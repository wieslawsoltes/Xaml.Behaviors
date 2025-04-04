using System.Collections.ObjectModel;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class DragAndDropSampleViewModel : ViewModelBase
{
    private ObservableCollection<DragItemViewModel> _items;
    private ObservableCollection<DragNodeViewModel> _nodes;
        
    public ObservableCollection<DragItemViewModel> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public ObservableCollection<DragNodeViewModel> Nodes
    {
        get => _nodes;
        set => this.RaiseAndSetIfChanged(ref _nodes, value);
    }

    public DragAndDropSampleViewModel()
    {
        _items =
        [
            new() { Title = "Item0" },
            new() { Title = "Item1" },
            new() { Title = "Item2" },
            new() { Title = "Item3" },
            new() { Title = "Item4" }
        ];

        var node0 = new DragNodeViewModel()
        {
            Title = "Node0"
        };
        node0.Nodes =
        [
            new() { Title = "Node0-0", Parent = node0 },
            new() { Title = "Node0-1", Parent = node0 },
            new() { Title = "Node0-2", Parent = node0 }
        ]; 

        var node1 = new DragNodeViewModel()
        {
            Title = "Node1"
        };
        node1.Nodes =
        [
            new() { Title = "Node1-0", Parent = node1 },
            new() { Title = "Node1-1", Parent = node1 },
            new() { Title = "Node1-2", Parent = node1 }
        ]; 

        var node2 = new DragNodeViewModel()
        {
            Title = "Node2"
        };
        node2.Nodes =
        [
            new() { Title = "Node2-0", Parent = node2 },
            new() { Title = "Node2-1", Parent = node2 },
            new() { Title = "Node2-2", Parent = node2 }
        ]; 

        _nodes =
        [
            node0,
            node1,
            node2
        ];
    }
}
