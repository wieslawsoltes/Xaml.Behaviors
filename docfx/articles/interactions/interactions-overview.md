# Interactions Overview

The `Xaml.Behaviors.Interactions` package contains a collection of common behaviors, triggers, and actions that cover many standard use cases in Avalonia applications.

## Namespaces

The package is organized into several namespaces:

*   **`Avalonia.Xaml.Interactions.Core`**: Contains fundamental triggers and actions like `EventTriggerBehavior`, `DataTriggerBehavior`, `ChangePropertyAction`, and `InvokeCommandAction`.
*   **`Avalonia.Xaml.Interactions.Clipboard`**: Contains actions for interacting with the system clipboard.
*   **`Avalonia.Xaml.Interactions.StorageProvider`**: Contains actions and behaviors for opening and saving files using the `IStorageProvider` API.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions
```

## Usage

The components in this package are designed to be used with the `Interaction` class from the `Xaml.Behaviors.Interactivity` package.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Save">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <CallMethodAction TargetObject="{Binding}" MethodName="Save" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```
