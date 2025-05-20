using Avalonia.Controls;
using System.Linq;

namespace BehaviorsTestApplication.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnSearchTextChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var query = SearchBox.Text?.ToLowerInvariant() ?? string.Empty;
        var visibleCount = 0;

        var tabItems = PagesTabControl.Items.OfType<TabItem>().ToList();

        foreach (var item in tabItems)
        {
            var header = item.Header?.ToString()?.ToLowerInvariant() ?? string.Empty;
            var visible = header.Contains(query);
            item.IsVisible = visible;

            if (visible)
            {
                visibleCount++;
            }
        }

        PagesTabControl.SelectedItem = tabItems.FirstOrDefault(x => x.IsVisible);

        NoMatchesText.IsVisible = visibleCount == 0;
    }
}
