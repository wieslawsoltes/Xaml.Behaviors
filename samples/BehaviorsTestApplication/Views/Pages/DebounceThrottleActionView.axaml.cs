using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class DebounceThrottleActionView : UserControl
{
    private int _throttleCount = 0;

    public DebounceThrottleActionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void IncrementThrottleCount()
    {
        _throttleCount++;
        var textBlock = this.FindControl<TextBlock>("ThrottleCountText");
        if (textBlock != null)
        {
            textBlock.Text = $"Clicks Processed: {_throttleCount}";
        }
    }
}
