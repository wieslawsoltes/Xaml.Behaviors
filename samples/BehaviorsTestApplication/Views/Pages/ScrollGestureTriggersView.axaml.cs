using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
