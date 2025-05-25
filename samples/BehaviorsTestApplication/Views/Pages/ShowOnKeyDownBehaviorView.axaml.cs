using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ShowOnKeyDownBehaviorView : UserControl
{
    public ShowOnKeyDownBehaviorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
