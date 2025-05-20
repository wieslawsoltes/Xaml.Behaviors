using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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

        foreach (var item in PagesTabControl.Items.OfType<TabItem>())
        {
            var header = item.Header?.ToString()?.ToLowerInvariant() ?? string.Empty;
            var visible = header.Contains(query);
            item.IsVisible = visible;

            if (visible)
            {
                visibleCount++;
            }
        }

        NoMatchesText.IsVisible = visibleCount == 0;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
