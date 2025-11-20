# CallMethodAsyncAction

This action allows you to invoke a method on a ViewModel (or any object) that returns a `Task`. It handles the execution of the asynchronous method.

### Properties

*   **TargetObject**: The object instance containing the method.
*   **MethodName**: The name of the method to call.

### Example

Assuming a ViewModel:

```csharp
public class MyViewModel
{
    public async Task SaveDataAsync()
    {
        await Task.Delay(1000); // Simulate work
        Console.WriteLine("Data Saved!");
    }
}
```

XAML Usage:

```xml
<Button Content="Save">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CallMethodAsyncAction TargetObject="{Binding}" 
                                       MethodName="SaveDataAsync" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

> **Note**: Unlike `CallMethodAction`, this action is designed to handle `Task`-returning methods gracefully, though it does not currently expose a way to await the completion within the XAML behavior chain itself.
