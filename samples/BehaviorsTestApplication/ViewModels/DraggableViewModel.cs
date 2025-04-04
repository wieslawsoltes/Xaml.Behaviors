using System.Collections.Generic;
using System.Collections.ObjectModel;
using BehaviorsTestApplication.Models;

namespace BehaviorsTestApplication.ViewModels;

public class DraggableViewModel : ViewModelBase
{
    public DraggableViewModel()
    {
        Items = new ObservableCollection<DragItem>()
        {
            new () { Title = "Item1", X = 30, Y = 30 },
            new () { Title = "Item2", X = 90, Y = 30 },
            new () { Title = "Item3", X = 120, Y = 60 },
            new () { Title = "Item4", X = 45, Y = 90 },
            new () { Title = "Item5", X = 60, Y = 120 },
            new () { Title = "Item6", X = 150, Y = 180 },
            new () { Title = "Item7", X = 250, Y = 120 },
            new () { Title = "Item8", X = 300, Y = 150 }
        };

        Strings = new ObservableCollection<string>();

        for (var i = 0; i < 1_000; i++)
        {
            Strings.Add($"Item {i+1} / {1000}");
        }

        Tiles = new ObservableCollection<Tile>()
        {
            new () { Title = "Tile1", Column = 0, Row = 0, ColumnSpan = 1, RowSpan = 1, Background = "Red" },
            new () { Title = "Tile2", Column = 0, Row = 1, ColumnSpan = 1, RowSpan = 1, Background = "Green" },
            new () { Title = "Tile3", Column = 1, Row = 0, ColumnSpan = 1, RowSpan = 2, Background = "Blue" },
            new () { Title = "Tile4", Column = 2, Row = 0, ColumnSpan = 1, RowSpan = 2, Background = "Cyan" },
        };

    }

    public IList<DragItem> Items { get; set; }

    public IList<string> Strings { get; }

    public IList<Tile> Tiles { get; set; }

}

