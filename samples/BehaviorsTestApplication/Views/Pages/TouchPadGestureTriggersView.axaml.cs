using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class TouchPadGestureTriggersView : UserControl
{
    public TouchPadGestureTriggersView()
    {
        InitializeComponent();
        DataContext = this;
        Events = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Events { get; }

    public void OnMagnify(object? sender, object? parameter) => AddEntry("Touchpad magnify gesture");

    public void OnRotate(object? sender, object? parameter) => AddEntry("Touchpad rotate gesture");

    public void OnSwipe(object? sender, object? parameter) => AddEntry("Touchpad swipe gesture");

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
