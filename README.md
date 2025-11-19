# XAML Behaviors


[![CI](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml)
[![Release](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/release.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/release.yml)

[![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Avalonia.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia)
[![NuGet](https://img.shields.io/nuget/dt/Xaml.Behaviors.Interactivity.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity)
[![MyGet](https://img.shields.io/myget/xamlbehaviors-nightly/vpre/Xaml.Behaviors.Avalonia.svg?label=myget)](https://www.myget.org/gallery/xamlbehaviors-nightly) 

**Xaml Behaviors** is a port of [Windows UWP](https://github.com/Microsoft/XamlBehaviors) version of XAML Behaviors for [Avalonia](https://github.com/AvaloniaUI/Avalonia) XAML.

Avalonia XAML Behaviors is an easy-to-use means of adding common and reusable interactivity to your [Avalonia](https://github.com/AvaloniaUI/Avalonia) applications with minimal code. Avalonia port is available only for managed applications. Use of XAML Behaviors is governed by the MIT License. 

<a href='https://www.youtube.com/watch?v=pffBS-yQ_uM' target='_blank'>![](https://i.ytimg.com/vi/pffBS-yQ_uM/hqdefault.jpg)<a/>

## Building Avalonia XAML Behaviors

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/Xaml.Behaviors.git
```

### Build on Windows using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=windows).

Open up a command-prompt and execute the commands:
```
.\build.ps1
```

### Build on Linux using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=linux).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

### Build on OSX using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=macos).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

## NuGet

Avalonia XamlBehaviors is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Xaml.Behaviors/) and install the package like this:

`Install-Package Xaml.Behaviors`

or by using nightly build feed:
* Add `https://www.myget.org/F/xamlbehaviors-nightly/api/v2` to your package sources
* Alternative nightly build feed `https://pkgs.dev.azure.com/wieslawsoltes/GitHub/_packaging/Nightly/nuget/v3/index.json`
* Update your package using `XamlBehaviors` feed

and install the package like this:

`Install-Package Xaml.Behaviors -Pre`

### Package Sources

* https://api.nuget.org/v3/index.json

### Available Packages

| Package | NuGet | Description |
|---------|-------|-------------|
| [Xaml.Behaviors](https://www.nuget.org/packages/Xaml.Behaviors) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.svg)](https://www.nuget.org/packages/Xaml.Behaviors) | Complete library of behaviors, actions and triggers for Avalonia applications. |
| [Xaml.Behaviors.Avalonia](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Avalonia.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia) | Meta package that bundles all Avalonia XAML Behaviors for easy installation. |
| [Xaml.Behaviors.Interactivity](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactivity.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity) | Foundation library providing base classes for actions, triggers and behaviors. |
| [Xaml.Behaviors.Interactions](https://www.nuget.org/packages/Xaml.Behaviors.Interactions) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions) | Core collection of common triggers and actions for Avalonia. |
| [Xaml.Behaviors.Interactions.Custom](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Custom.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom) | Custom behaviors and actions for common Avalonia controls. |
| [Xaml.Behaviors.Interactions.DragAndDrop](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.DragAndDrop.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop) | Behaviors that enable drag-and-drop support in Avalonia. |
| [Xaml.Behaviors.Interactions.DragAndDrop.DataGrid](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid) | DataGrid-specific drag-and-drop helpers built on top of the drag-and-drop framework. |
| [Xaml.Behaviors.Interactions.Draggable](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Draggable.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable) | Draggable element behaviors for moving controls around. |
| [Xaml.Behaviors.Interactions.Events](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Events.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events) | Behaviors responding to Avalonia input and focus events. |
| [Xaml.Behaviors.Interactions.ReactiveUI](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.ReactiveUI.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI) | Behaviors integrating ReactiveUI navigation patterns. |
| [Xaml.Behaviors.Interactions.Responsive](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Responsive.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive) | Behaviors to assist with responsive and adaptive UI layouts. |
| [Xaml.Behaviors.Interactions.Scripting](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Scripting.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting) | Execute C# scripts at runtime to add dynamic behavior. |

## Getting Started

### Adding Behaviors in XAML

The steps below show how a new user can attach their first behavior. These examples assume you have created an Avalonia application and want to enhance it with interactions.

1. **Install the package** – open your NuGet package manager and add `Xaml.Behaviors` or any of the interaction packages such as `Xaml.Behaviors.Interactions`.
2. **Declare the namespace** – reference the default Avalonia namespace and the XAML Behaviors namespace at the top of your XAML file.
3. **Attach a behavior** – use the `Interaction.Behaviors` attached property to add behaviors to your controls.

Here is a minimal example:

```xaml
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

```xaml
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

```xaml
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

## Docs

[XAML Behaviors for Avalonia documentation.](https://wieslawsoltes.github.io/Xaml.Behaviors/)

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Xaml.Behaviors)

## License

XAML Behaviors is licensed under the [MIT license](LICENSE.TXT).
