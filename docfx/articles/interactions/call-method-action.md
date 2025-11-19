# CallMethodAction

`CallMethodAction` is an action that calls a method on a specified object when invoked.

## Properties

*   **`TargetObject`**: The object on which to call the method. If not set, it defaults to the `AssociatedObject` of the behavior.
*   **`MethodName`**: The name of the method to call.

## Method Signature

The method to be called must meet one of the following criteria:

1.  It takes no parameters.
2.  It takes two parameters: `(object? sender, object? parameter)`. This matches the signature of the `Execute` method of the action.

## Usage

```xml
<Button Content="Save">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CallMethodAction TargetObject="{Binding}" MethodName="SaveData" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

```csharp
// ViewModel
public void SaveData()
{
    // ...
}
```
