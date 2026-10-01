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
        // Move the items into a shuffled order instead of clearing and re-adding them; FluidMoveBehavior animates
        // every item from its previous position.
        var order = Items.ToArray();
        Random.Shared.Shuffle(order);
        for (var i = 0; i < order.Length; i++)
        {
            var index = Items.IndexOf(order[i]);
            if (index != i)
            {
                Items.Move(index, i);
            }
        }
    }
}
