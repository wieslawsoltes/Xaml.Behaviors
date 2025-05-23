using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class FluidMoveBehaviorView : UserControl, ISamplePage
{
    public FluidMoveBehaviorView()
    {
        InitializeComponent();
        DataContext = new FluidMoveBehaviorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
