using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class WindowInteractionsView : UserControl
{
    public WindowInteractionsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
