using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.UnitTests.Core;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Core;
#endif

public partial class ItemDragBehaviorVertical : Window
{
    public ObservableCollection<string> Items { get; } = new(["Item1", "Item2", "Item3"]);

    public ItemDragBehaviorVertical()
    {
        InitializeComponent();
        DataContext = this;
    }
}
