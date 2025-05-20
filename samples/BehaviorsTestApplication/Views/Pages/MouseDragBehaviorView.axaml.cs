using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Interactions.Draggable;

namespace BehaviorsTestApplication.Views.Pages;

public partial class MouseDragBehaviorView : UserControl
{
    public MouseDragBehaviorView()
    {
        InitializeComponent();

        var rect1 = this.FindControl<Control>("MultiRect1");
        var rect2 = this.FindControl<Control>("MultiRect2");
        var rect3 = this.FindControl<Control>("MultiRect3");

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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
