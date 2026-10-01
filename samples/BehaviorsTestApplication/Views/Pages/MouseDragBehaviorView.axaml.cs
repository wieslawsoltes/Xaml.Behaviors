using System.Linq;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Xaml.Interactivity;
using Xaml.Interactions.Draggable;
#else
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Draggable;
#endif

namespace BehaviorsTestApplication.Views.Pages;

public partial class MouseDragBehaviorView : UserControl
{
    public MouseDragBehaviorView()
    {
        InitializeComponent();

#if UNO
        var rect1 = FindName("MultiRect1") as Control;
        var rect2 = FindName("MultiRect2") as Control;
        var rect3 = FindName("MultiRect3") as Control;
#else
        var rect1 = this.FindControl<Control>("MultiRect1");
        var rect2 = this.FindControl<Control>("MultiRect2");
        var rect3 = this.FindControl<Control>("MultiRect3");
#endif

        if (rect1 is not null && rect2 is not null && rect3 is not null)
        {
            var behavior = Interaction.GetBehaviors(rect1)
                .OfType<MultiMouseDragElementBehavior>()
                .FirstOrDefault();
            if (behavior is not null)
            {
                behavior.TargetControls.Add(rect2);
                behavior.TargetControls.Add(rect3);
            }
        }
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
