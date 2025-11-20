using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.Views.Windows;

namespace BehaviorsTestApplication.Views.Pages;

public partial class WindowDragMoveBehaviorView : UserControl
{
    public WindowDragMoveBehaviorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenWindowDemo_Click(object? sender, RoutedEventArgs e)
    {
        var window = new WindowDemo();
        if (VisualRoot is Window owner)
        {
            window.ShowDialog(owner);
        }
        else
        {
            window.Show();
        }
    }
}
