using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

namespace BehaviorsTestApplication.Controls;

#if UNO
// WinUI: a TabView (always single selection) that uses the TabView styles.
public partial class SingleSelectionTabControl : TabView
{
    public SingleSelectionTabControl()
    {
        DefaultStyleKey = typeof(TabView);
    }
}
#else
public class SingleSelectionTabControl : TabControl
{
    protected override Type StyleKeyOverride => typeof(TabControl);
    
    static SingleSelectionTabControl()
    {
        SelectionModeProperty.OverrideDefaultValue<SingleSelectionTabControl>(SelectionMode.Single);
    }
}
#endif
