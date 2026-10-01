using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
// Uno Platform: the Avalonia ListBox is a WinUI ListView in the view.
using ListBox = Microsoft.UI.Xaml.Controls.ListView;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class AutoScrollToBottomBehaviorView : UserControl
{
    private ObservableCollection<string> _items;
    private int _count = 0;
    private ListBox? _itemsListBox;

    public AutoScrollToBottomBehaviorView()
    {
        InitializeComponent();
#if UNO
        // The Uno XAML generator provides the named element as a field.
        _itemsListBox = ItemsListBox;
#endif
        _items = new ObservableCollection<string>();
        for (int i = 0; i < 20; i++)
        {
            _items.Add($"Item {_count++}");
        }
        
        if (_itemsListBox is not null)
        {
            _itemsListBox.ItemsSource = _items;
        }
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _itemsListBox = this.FindControl<ListBox>("ItemsListBox");
    }
#endif

    private void AddItemButton_Click(object? sender, RoutedEventArgs e)
    {
        _items.Add($"Item {_count++}");
    }
}
