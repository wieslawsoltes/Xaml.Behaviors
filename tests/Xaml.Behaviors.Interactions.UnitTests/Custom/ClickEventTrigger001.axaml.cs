using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public partial class ClickEventTrigger001 : Window
{
    public int ReleaseClicks { get; private set; }
    public int PressClicks { get; private set; }
    public int ModifierClicks { get; private set; }
    public int SpaceReleaseClicks { get; private set; }
    public int SpacePressClicks { get; private set; }
    public int DefaultClicks { get; private set; }
    public int CancelClicks { get; private set; }
    public int FlyoutClicks { get; private set; }

    public ClickEventTrigger001()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void OnReleaseClicked() => ReleaseClicks++;

    public void OnPressClicked() => PressClicks++;

    public void OnModifierClicked() => ModifierClicks++;

    public void OnSpaceReleaseClicked() => SpaceReleaseClicks++;

    public void OnSpacePressClicked() => SpacePressClicks++;

    public void OnDefaultClicked() => DefaultClicks++;

    public void OnCancelClicked() => CancelClicks++;

    public void OnFlyoutClicked() => FlyoutClicks++;
}
