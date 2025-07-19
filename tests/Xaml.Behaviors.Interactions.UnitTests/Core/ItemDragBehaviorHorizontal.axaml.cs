using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public partial class ItemDragBehaviorHorizontal : Window
{
    public ObservableCollection<string> Items { get; } = new(["Item1", "Item2", "Item3"]);

    public ItemDragBehaviorHorizontal()
    {
        InitializeComponent();
        DataContext = this;
    }
}
