using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class PullGestureTriggersView : UserControl
{
    public PullGestureTriggersView()
    {
        InitializeComponent();
        DataContext = this;
        Events = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Events { get; }

    public void OnPull(object? sender, object? parameter)
    {
        AddEntry("Pull gesture started");
    }

    public void OnPullEnded(object? sender, object? parameter)
    {
        AddEntry("Pull gesture ended");
    }

    public void Clear()
    {
        Events.Clear();
    }

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
