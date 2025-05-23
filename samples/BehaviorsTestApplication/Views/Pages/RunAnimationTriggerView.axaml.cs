using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class RunAnimationTriggerView : UserControl, ISamplePage
{
    public RunAnimationTriggerView()
    {
        InitializeComponent();
        DataContext = new CustomAnimatorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
