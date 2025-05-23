using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class FluidMoveBehaviorViewModel : ViewModelBase
{
    public FluidMoveBehaviorViewModel()
    {
        for (var i = 1; i <= 8; i++)
        {
            Items.Add(i);
        }

        ShuffleCommand = ReactiveCommand.Create(Shuffle);
    }

    public ObservableCollection<int> Items { get; } = new();

    public ICommand ShuffleCommand { get; }

    private void Shuffle()
    {
        var rnd = new Random();
        var array = Items.ToList();
        Items.Clear();
        foreach (var value in array.OrderBy(_ => rnd.Next()))
        {
            Items.Add(value);
        }
    }
}
