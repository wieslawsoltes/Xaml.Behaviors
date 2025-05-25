using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class PointerCaptureLostTriggerView : UserControl
{
    public PointerCaptureLostTriggerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
