using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ToggleIsExpandedOnDoubleTappedBehaviorView : UserControl
{
    public ToggleIsExpandedOnDoubleTappedBehaviorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
