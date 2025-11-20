using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class AutoScrollToBottomBehaviorView : UserControl
{
    private ObservableCollection<string> _items;
    private int _count = 0;
    private ListBox? _itemsListBox;

    public AutoScrollToBottomBehaviorView()
    {
        InitializeComponent();
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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _itemsListBox = this.FindControl<ListBox>("ItemsListBox");
    }

    private void AddItemButton_Click(object? sender, RoutedEventArgs e)
    {
        _items.Add($"Item {_count++}");
    }
}
