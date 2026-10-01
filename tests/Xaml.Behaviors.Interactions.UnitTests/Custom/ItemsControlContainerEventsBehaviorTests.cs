using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Xaml.Interactions.Custom;
using ItemsControl = Microsoft.UI.Xaml.Controls.ItemsRepeater;
using ContainerPreparedEventArgs = Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs;
using ContainerClearingEventArgs = Microsoft.UI.Xaml.Controls.ItemsRepeaterElementClearingEventArgs;
#else
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
#endif
using Xunit;

#if UNO
namespace Xaml.Interactions.UnitTests.Custom;
#else
namespace Avalonia.Xaml.Interactions.UnitTests.Custom;
#endif

public class ItemsControlContainerEventsBehaviorTests
{
    [AvaloniaFact]
    public void Detached_Behavior_No_Longer_Receives_Container_Events()
    {
        var items = new ObservableCollection<string> { "A" };
        var itemsControl = new ItemsControl { ItemsSource = items };
        var window = new Window { Width = 200, Height = 200, Content = itemsControl };
        window.Show();

        var behavior = new CountingBehavior();
        behavior.Attach(itemsControl);

        items.Add("B");
        Realize(window);

        var preparing = behavior.PreparingCount;
        var prepared = behavior.PreparedCount;
#if !UNO
        // ItemsRepeater (Uno) has no event before an element is prepared.
        Assert.Equal(1, preparing);
#endif
        Assert.Equal(1, prepared);

        behavior.Detach();

        items.Add("C");
        items.RemoveAt(0);
        Realize(window);

        Assert.Equal(preparing, behavior.PreparingCount);
        Assert.Equal(prepared, behavior.PreparedCount);
        Assert.Equal(0, behavior.ClearingCount);

        window.Close();
    }

    private static void Realize(Window window)
    {
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
    }

    private sealed class CountingBehavior : ItemsControlContainerEventsBehavior
    {
        public int PreparingCount { get; private set; }

        public int PreparedCount { get; private set; }

        public int ClearingCount { get; private set; }

        protected override void OnPreparingContainer(object? sender, ContainerPreparedEventArgs e)
        {
            PreparingCount++;
        }

        protected override void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
        {
            PreparedCount++;
        }

        protected override void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
        {
            ClearingCount++;
        }
    }
}
