using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class InteractionTriggerBehaviorView : UserControl
{
    public InteractionTriggerBehaviorView()
    {
        InitializeComponent();
        DataContext = new InteractionTriggerBehaviorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
