using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class EventTriggerBehaviorView : UserControl, ISamplePage
{
    public EventTriggerBehaviorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (this.FindControl<ContentControl>("ContentControl") is { } contentControl)
        {
            contentControl.Content = null;
        }
    }
}
