using System.Collections.ObjectModel;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
#endif

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

#if UNO
    // WinUI raises every pointer event with PointerRoutedEventArgs: the update kind of the current point tells a press
    // or a release from a move.
    private static string DescribePointerEvent(object? args)
    {
        if (args is not PointerRoutedEventArgs pointer)
        {
            return args?.GetType().Name ?? "Unknown";
        }

        var point = pointer.GetCurrentPoint(null);
        var position = $"{point.Position.X:F0}, {point.Position.Y:F0}";
        return point.Properties.PointerUpdateKind switch
        {
            Microsoft.UI.Input.PointerUpdateKind.LeftButtonPressed or
            Microsoft.UI.Input.PointerUpdateKind.MiddleButtonPressed or
            Microsoft.UI.Input.PointerUpdateKind.RightButtonPressed => $"Pressed at {position}",
            Microsoft.UI.Input.PointerUpdateKind.LeftButtonReleased or
            Microsoft.UI.Input.PointerUpdateKind.MiddleButtonReleased or
            Microsoft.UI.Input.PointerUpdateKind.RightButtonReleased => $"Released at {position}",
            _ => $"Moved at {position}"
        };
    }
#else
    private static string DescribePointerEvent(object? args) => args switch
    {
        PointerPressedEventArgs pressed => $"Pressed at {pressed.GetPosition(null):F0}",
        PointerReleasedEventArgs released => $"Released at {released.GetPosition(null):F0}",
        PointerEventArgs moved => $"Moved at {moved.GetPosition(null):F0}",
        _ => args?.GetType().Name ?? "Unknown"
    };
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
