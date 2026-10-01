using System.Collections.ObjectModel;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class DragLeaveEventTriggerView : UserControl
{
    public DragLeaveEventTriggerView()
    {
        InitializeComponent();
        DataContext = this;
        Events = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Events { get; }

    public void OnDragLeave(object? sender, object? parameter)
    {
        Events.Insert(0, $"{System.DateTime.Now:T} - Drag left drop zone");
        if (Events.Count > 20)
        {
            Events.RemoveAt(Events.Count - 1);
        }
    }

#if !UNO
    // Uno Platform: the Start drag button of the WinUI view uses ContextDragBehavior instead of this handler.
    private async void OnStartDrag(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var data = new DataTransfer();
        data.Add(DataTransferItem.Create(DataFormat.Text, "DragLeave sample"));
        await DragDrop.DoDragDropAsync(e, data, DragDropEffects.Copy);
    }
#endif

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
