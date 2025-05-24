using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class PointerTriggersView : UserControl
{
    public PointerTriggersView()
    {
        InitializeComponent();
        DataContext = new PointerTriggersViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
