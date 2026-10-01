#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.Views.Windows;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class WindowDragMoveBehaviorView : UserControl
{
    public WindowDragMoveBehaviorView()
    {
        InitializeComponent();
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif

#if !UNO
    // Opens the decoration-less WindowDemo (WindowDragMoveBehavior); the Uno Platform view shows that the sample is not
    // available (Window.BeginMoveDrag has no WinUI counterpart).
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
#endif
}
