# Third-Party Integration

You can generate typed actions and triggers for classes in third-party libraries (where you cannot add attributes directly) by using assembly-level attributes.

## Usage

Add the attributes to your assembly (e.g., in `Program.cs` or `App.axaml.cs`).

### Actions

```csharp
[assembly: GenerateTypedAction(typeof(ThirdPartyViewModel), "ExternalMethod")]
```

This generates `ExternalMethodAction` in the namespace of `ThirdPartyViewModel`.

### Triggers

```csharp
[assembly: GenerateTypedTrigger(typeof(ThirdPartyViewModel), "ExternalEvent")]
```

This generates `ExternalEventTrigger` in the namespace of `ThirdPartyViewModel`.

### ChangePropertyAction

```csharp
[assembly: GenerateTypedChangePropertyAction(typeof(ThirdPartyViewModel), "ExternalProperty")]
```

This generates `SetExternalPropertyAction` in the namespace of `ThirdPartyViewModel`.

## XAML Usage

Since the generated classes are placed in the same namespace as the target type (e.g., `ThirdParty.Library.ViewModels`), you need to declare that namespace in your XAML.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:external="using:ThirdParty.Library.ViewModels">
    <Button Content="Click Me">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <external:ExternalMethodAction TargetObject="{Binding ExternalViewModel}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```
