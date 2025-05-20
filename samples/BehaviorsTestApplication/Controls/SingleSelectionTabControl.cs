using System;
using Avalonia.Controls;

namespace BehaviorsTestApplication.Controls;

public class SingleSelectionTabControl : TabControl
{
    protected override Type StyleKeyOverride => typeof(TabControl);
    
    static SingleSelectionTabControl()
    {
        SelectionModeProperty.OverrideDefaultValue<SingleSelectionTabControl>(SelectionMode.Single);
    }
}
