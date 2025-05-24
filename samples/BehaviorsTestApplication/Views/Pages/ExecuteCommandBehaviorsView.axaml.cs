using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ExecuteCommandBehaviorsView : UserControl
{
    public ExecuteCommandBehaviorsView()
    {
        InitializeComponent();
        DataContext = new ExecuteCommandBehaviorsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
