#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Xaml.Interactions.Custom;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class HideAttachedFlyoutBehaviorTests
{
    [AvaloniaFact]
    public void IsFlyoutOpenProperty_Is_Owned_By_HideAttachedFlyoutBehavior()
    {
#if !UNO
        // WinUI dependency properties expose neither their name nor their owner type.
        Assert.Equal(nameof(HideAttachedFlyoutBehavior.IsFlyoutOpen), HideAttachedFlyoutBehavior.IsFlyoutOpenProperty.Name);
        Assert.Equal(typeof(HideAttachedFlyoutBehavior), HideAttachedFlyoutBehavior.IsFlyoutOpenProperty.OwnerType);
        Assert.True(AvaloniaPropertyRegistry.Instance.IsRegistered(typeof(HideAttachedFlyoutBehavior), HideAttachedFlyoutBehavior.IsFlyoutOpenProperty));
        Assert.False(AvaloniaPropertyRegistry.Instance.IsRegistered(typeof(ButtonHideFlyoutBehavior), HideAttachedFlyoutBehavior.IsFlyoutOpenProperty));
        Assert.Same(
            ButtonHideFlyoutBehavior.IsFlyoutOpenProperty,
            AvaloniaPropertyRegistry.Instance.FindRegistered(typeof(ButtonHideFlyoutBehavior), nameof(ButtonHideFlyoutBehavior.IsFlyoutOpen)));
#endif
        Assert.NotSame(ButtonHideFlyoutBehavior.IsFlyoutOpenProperty, HideAttachedFlyoutBehavior.IsFlyoutOpenProperty);

        var behavior = new HideAttachedFlyoutBehavior();

        Assert.False(behavior.IsFlyoutOpen);

        behavior.IsFlyoutOpen = true;

        Assert.True(behavior.IsFlyoutOpen);
        Assert.Equal(true, behavior.GetValue(HideAttachedFlyoutBehavior.IsFlyoutOpenProperty));
    }

    [AvaloniaFact]
    public void Toggling_IsFlyoutOpen_Hides_The_Attached_Flyout_Only_When_False()
    {
        var target = new Border { Width = 20, Height = 20 };
        var flyout = new Flyout { Content = new TextBlock { Text = "flyout" } };
        FlyoutBase.SetAttachedFlyout(target, flyout);
        var behavior = new HideAttachedFlyoutBehavior { IsFlyoutOpen = true };
        Interaction.GetBehaviors(target).Add(behavior);
        var window = new Window { Width = 200, Height = 200, Content = target };

        window.Show();

        FlyoutBase.ShowAttachedFlyout(target);
        Dispatcher.UIThread.RunJobs();
        Assert.True(flyout.IsOpen);

        behavior.IsFlyoutOpen = false;
        Dispatcher.UIThread.RunJobs();
        Assert.False(flyout.IsOpen);

        FlyoutBase.ShowAttachedFlyout(target);
        behavior.IsFlyoutOpen = true;
        Dispatcher.UIThread.RunJobs();
        Assert.True(flyout.IsOpen);

        behavior.IsFlyoutOpen = false;
        Dispatcher.UIThread.RunJobs();
        Assert.False(flyout.IsOpen);

        window.Close();
    }
}
