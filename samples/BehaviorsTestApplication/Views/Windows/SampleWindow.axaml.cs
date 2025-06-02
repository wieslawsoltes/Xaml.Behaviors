using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Windows;

public partial class SampleWindow : Window
{
    public SampleWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
