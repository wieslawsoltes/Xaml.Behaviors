using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class TappedTriggerView : UserControl
{
    public TappedTriggerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
