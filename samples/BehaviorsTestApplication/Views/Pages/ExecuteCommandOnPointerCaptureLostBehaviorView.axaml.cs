using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class ExecuteCommandOnPointerCaptureLostBehaviorView : UserControl
{
    public ExecuteCommandOnPointerCaptureLostBehaviorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
