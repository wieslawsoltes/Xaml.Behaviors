using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class AnimationBuilderView : UserControl
{
    public AnimationBuilderView()
    {
        InitializeComponent();
        DataContext = new CustomAnimatorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
