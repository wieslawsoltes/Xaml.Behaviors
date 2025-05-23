using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Views.Pages;

public partial class TypedDragBehaviorView : UserControl, ISamplePage
{
    public TypedDragBehaviorView()
    {
        InitializeComponent();
        DataContext = new DragAndDropSampleViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
