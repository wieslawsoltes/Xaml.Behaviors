using System;
using System.Linq;
using Avalonia.Controls;
using BehaviorsTestApplication.ViewModels;
using BehaviorsTestApplication.Views.Pages;

namespace BehaviorsTestApplication.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        AddSamplePages();
    }

    private void AddSamplePages()
    {
        if (PagesTabControl is null)
        {
            return;
        }

        var mainVm = DataContext as MainWindowViewModel;

        var pageTypes = typeof(ISamplePage).Assembly.GetTypes()
            .Where(t => typeof(ISamplePage).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .OrderBy(t => t.Name);

        foreach (var type in pageTypes)
        {
            if (Activator.CreateInstance(type) is not ISamplePage page)
            {
                continue;
            }

            if (page is PointerTriggersView ptr && mainVm != null)
            {
                ptr.DataContext = mainVm.PointerTriggersViewModel;
            }
            else if (page is KeyGestureTriggerView kgt && mainVm != null)
            {
                kgt.DataContext = mainVm.KeyGestureTriggerViewModel;
            }

            var tab = new TabItem
            {
                Header = page.Header,
                Content = page as Control
            };

            PagesTabControl.Items ??= new Avalonia.Collections.AvaloniaList<object>();
            PagesTabControl.Items = PagesTabControl.Items.Cast<object>().Append(tab).ToList();
        }
    }
}
