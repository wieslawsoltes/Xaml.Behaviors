using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class KeyGestureTriggerView : UserControl
{
    public KeyGestureTriggerView()
    {
        InitializeComponent();
        DataContext = new KeyGestureTriggerViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
