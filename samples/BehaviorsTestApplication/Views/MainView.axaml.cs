using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views;

public partial class MainView : UserControl
{
    private TextBox? _searchBox;
    private TabControl? _tabControl;

    public MainView()
    {
        InitializeComponent();
        _tabControl = this.FindControl<TabControl>("SamplesTabControl");
        _searchBox = this.FindControl<TextBox>("SearchBox");
        if (_searchBox != null)
        {
            _searchBox.GetObservable(TextBox.TextProperty)
                .Subscribe(text => Search(text));
        }
    }

    private void Search(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || _tabControl == null)
            return;

        var item = _tabControl.Items
            .OfType<TabItem>()
            .FirstOrDefault(i => i.Header is string header &&
                header.Contains(text, StringComparison.OrdinalIgnoreCase));
        if (item != null)
        {
            _tabControl.SelectedItem = item;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
