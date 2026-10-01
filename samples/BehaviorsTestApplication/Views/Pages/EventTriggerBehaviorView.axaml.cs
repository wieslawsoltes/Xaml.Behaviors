#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class EventTriggerBehaviorView : UserControl
{
    public EventTriggerBehaviorView()
    {
        InitializeComponent();
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

#if UNO
        // WinUI looks named elements up with FindName (Avalonia FindControl).
        if (FindName("ContentControl") is ContentControl contentControl)
#else
        if (this.FindControl<ContentControl>("ContentControl") is { } contentControl)
#endif
        {
            contentControl.Content = null;
        }
    }
}
