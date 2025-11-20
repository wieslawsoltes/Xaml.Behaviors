# XML Namespace Definitions

The `Xaml.Behaviors.Interactivity` package (and related packages) uses the `[XmlnsDefinition]` attribute to map its CLR namespaces to the standard Avalonia XML namespace (`https://github.com/avaloniaui`).

## What does this mean?

This means that you do not need to declare separate XML namespaces (like `xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Xaml.Behaviors.Interactivity"`) to use behaviors, triggers, and actions in your XAML.

Instead, you can use them directly under the default Avalonia namespace, or any prefix mapped to it.

## Example

**Without XmlnsDefinition (Old Way):**

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Xaml.Behaviors.Interactivity"
             xmlns:ia="clr-namespace:Avalonia.Xaml.Interactions.Core;assembly=Xaml.Behaviors.Interactions">
    <Button>
        <i:Interaction.Behaviors>
            <ia:EventTriggerBehavior EventName="Click">
                <!-- ... -->
            </ia:EventTriggerBehavior>
        </i:Interaction.Behaviors>
    </Button>
</UserControl>
```

**With XmlnsDefinition (Current Way):**

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button>
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <!-- ... -->
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

## Benefits

*   **Cleaner XAML**: Reduces the number of namespace declarations at the top of your files.
*   **Simplicity**: Makes the code easier to read and write.
*   **Discoverability**: IntelliSense can find behaviors and actions more easily when they are part of the default namespace.

## How it works

The assembly contains an attribute like this:

```csharp
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "Avalonia.Xaml.Interactivity")]
```

When the XAML compiler or loader encounters the `https://github.com/avaloniaui` namespace, it looks into all referenced assemblies that have this attribute and includes the specified CLR namespaces in the lookup scope.
