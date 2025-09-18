using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace BehaviorsTestApplication.Views.Pages;

public partial class PointerEventsTriggerView : UserControl
{
    public PointerEventsTriggerView()
    {
        InitializeComponent();
        DataContext = this;
        Events = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Events { get; }

    public void OnPointerEvent(object? sender, object? args)
    {
        var description = DescribePointerEvent(args);
        Events.Insert(0, description);
        if (Events.Count > 50)
        {
            Events.RemoveAt(Events.Count - 1);
        }
    }

    private static string DescribePointerEvent(object? args) => args switch
    {
        PointerPressedEventArgs pressed => $"Pressed at {pressed.GetPosition(null):F0}",
        PointerReleasedEventArgs released => $"Released at {released.GetPosition(null):F0}",
        PointerEventArgs moved => $"Moved at {moved.GetPosition(null):F0}",
        _ => args?.GetType().Name ?? "Unknown"
    };

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
