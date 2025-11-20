# Introduction

Introduction to XAML Behaviors for Avalonia.

## Getting Started

### Adding Behaviors in XAML

The steps below show how a new user can attach their first behavior. These examples assume you have created an Avalonia application and want to enhance it with interactions.

1. **Install the package** – open your NuGet package manager and add `Xaml.Behaviors` or any of the interaction packages such as `Xaml.Behaviors.Interactions`.
2. **Declare the namespace** – reference the default Avalonia namespace and the XAML Behaviors namespace at the top of your XAML file.
3. **Attach a behavior** – use the `Interaction.Behaviors` attached property to add behaviors to your controls.

Here is a minimal example:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Click">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <InvokeCommandAction Command="{Binding ClickCommand}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

### Basic Examples

The examples below walk through typical scenarios where behaviors remove the need for boilerplate code.

1. **React to data changes** – monitor a bound property and update another control automatically.
2. **Handle lifecycle events** – execute actions when a control appears or is loaded.

In XAML this looks like:

```xml
<!-- Change text based on slider value -->
<TextBlock x:Name="Output">
    <Interaction.Behaviors>
        <DataTriggerBehavior Binding="{Binding #Slider.Value}" ComparisonCondition="GreaterThan" Value="50">
            <ChangePropertyAction PropertyName="Text" Value="High" />
        </DataTriggerBehavior>
        <DataTriggerBehavior Binding="{Binding #Slider.Value}" ComparisonCondition="LessThanOrEqual" Value="50">
            <ChangePropertyAction PropertyName="Text" Value="Low" />
        </DataTriggerBehavior>
    </Interaction.Behaviors>
</TextBlock>

<!-- Focus a control when it appears -->
<TextBox>
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Loaded">
            <FocusControlAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</TextBox>
```

### How It Works

Behind the scenes behaviors rely on Avalonia's attached property and reactive object model. When you set `Interaction.Behaviors` on any `AvaloniaObject`, Avalonia's property system fires a property changed notification. The `Interaction` helper class listens for these changes and attaches each entry of the `BehaviorCollection` to the target control. Each behavior implements `IAttachedObject` and inherits from `Behavior` or `Behavior<T>` which provide `OnAttached` and `OnDetaching` methods. These methods are invoked by the framework so the behavior can subscribe to routed events, bind to properties or manipulate the visual tree. Because behaviors live in a collection you can add or remove them dynamically at runtime just like any other Avalonia property.

Triggers and actions follow the same pattern. A `Trigger` holds a collection of `IAction` objects. When the trigger condition is met—for example an event fires or a bound property changes—the trigger iterates its actions and calls their `Execute` method. Everything relies on base types such as `AvaloniaObject`, the binding engine and the routing/event system, so no reflection or markup extension magic is needed. Understanding these building blocks helps when creating custom behaviors or when troubleshooting how they interact with your controls.

### Advanced Usage

Behaviors can also be defined in styles or extended by creating your own implementations. Advanced scenarios typically involve keeping XAML clean or encapsulating reusable interaction logic.

1. **Attach behaviors from styles** – ideal when the same behavior should apply to multiple controls.
2. **Create custom behaviors** – extend `Behavior<T>` to implement unique interactions.

Below is a walkthrough of both approaches.

**Defining behaviors in a style**

```xml
<Style Selector="Button.alert">
    <Setter Property="(Interaction.Behaviors)">
        <BehaviorCollectionTemplate>
            <BehaviorCollection>
                <EventTriggerBehavior EventName="PointerEnter">
                    <ChangePropertyAction PropertyName="Background" Value="Red" />
                </EventTriggerBehavior>
            </BehaviorCollection>
        </BehaviorCollectionTemplate>
    </Setter>
</Style>
```

To use the style, assign the `alert` class to any `Button` and it will change its background when the pointer enters the control.

**Creating a custom behavior**

```csharp
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

public class RotateOnClickBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        AssociatedObject.PointerPressed += OnPointerPressed;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PointerPressed -= OnPointerPressed;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        AssociatedObject.RenderTransform = new RotateTransform(45);
    }
}
```

1. **Derive from `Behavior<T>`** – specify the control type the behavior will attach to.
2. **Override `OnAttached` and `OnDetaching`** – wire up and remove event handlers or other logic.
3. **Consume the behavior** – add it in XAML like any built‑in behavior or through a style.

Custom triggers and actions follow the same pattern—derive from `Trigger` or `Action` and override the relevant methods. Use these techniques to build a library of reusable interactions tailored to your application.
