using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class ScrollGestureTriggersView : UserControl
{
    public ScrollGestureTriggersView()
    {
        InitializeComponent();
        DataContext = this;
        Events = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Events { get; }

    public void OnScroll(object? sender, object? parameter) => AddEntry("Scroll gesture");

    public void OnScrollEnded(object? sender, object? parameter) => AddEntry("Scroll gesture ended");

    public void OnScrollInertia(object? sender, object? parameter) => AddEntry("Scroll inertia starting");

    public void Clear() => Events.Clear();

    private void AddEntry(string message)
    {
        Events.Insert(0, $"{System.DateTime.Now:T} - {message}");
        if (Events.Count > 50)
        {
            Events.RemoveAt(Events.Count - 1);
        }
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
