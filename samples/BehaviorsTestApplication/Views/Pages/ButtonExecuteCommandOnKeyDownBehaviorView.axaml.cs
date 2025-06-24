using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ButtonExecuteCommandOnKeyDownBehaviorView : UserControl
{
    public ButtonExecuteCommandOnKeyDownBehaviorView()
    {
        InitializeComponent();
        DataContext = new ExecuteCommandBehaviorsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
