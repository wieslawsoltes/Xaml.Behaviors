#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class DebounceThrottleActionView : UserControl
{
    private int _throttleCount = 0;

    public DebounceThrottleActionView()
    {
        InitializeComponent();
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif

    public void IncrementThrottleCount()
    {
        _throttleCount++;
#if UNO
        // WinUI has no FindControl: the Uno XAML generator provides the named element as a field.
        var textBlock = ThrottleCountText;
#else
        var textBlock = this.FindControl<TextBlock>("ThrottleCountText");
#endif
        if (textBlock != null)
        {
            textBlock.Text = $"Clicks Processed: {_throttleCount}";
        }
    }
}
