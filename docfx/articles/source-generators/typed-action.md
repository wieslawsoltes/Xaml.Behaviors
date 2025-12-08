# Typed Actions

The `[GenerateTypedAction]` attribute generates a strongly-typed `Action` class that invokes a specific method on a target object. This is the AOT-safe alternative to `CallMethodAction`.

## Usage

Annotate a method in your ViewModel (or any class) with `[GenerateTypedAction]`.

```csharp
public partial class MyViewModel
{
    [GenerateTypedAction]
    public void Submit()
    {
        // ...
    }
}
```

The generator will create a class named `SubmitAction` in the same namespace.

You can also add attributes at the assembly level to generate actions for one or more methods on a type. The `methodName` can be a literal name, a wildcard (`*`) pattern, or a regular expression. Assembly-level actions are prefixed with the target type name to avoid collisions (e.g. `ButtonSubmitAction`).

```csharp
[assembly: GenerateTypedAction(typeof(Avalonia.Controls.Button), "Click")]
[assembly: GenerateTypedAction(typeof(MyApp.ViewModels.ShellViewModel), "Save*")]
[assembly: GenerateTypedAction(typeof(MyApp.ViewModels.ShellViewModel), "^(Load|Unload)$")]
```

> Actions require the target method to be public or internal in the same/friend assembly. Wildcard/regex assembly attributes skip inaccessible matches and will emit a diagnostic if nothing accessible matches.
> Method parameter and return types must also be accessible. Public types always work; internal types are fine when you generate inside the same assembly (the normal case). If you point at internal members in a different assembly, that assembly must grant `InternalsVisibleTo("<your assembly name>")` to the consuming assembly. Wildcard/regex matches that rely on inaccessible types are ignored.
> Generated action classes are `public` unless any target/parameter/return type requires `internal`, in which case the class is emitted as `internal`.

### Matching rules at a glance

| Aspect | Behavior |
| --- | --- |
| Pattern kinds | Literal name, `*` wildcard, or regular expression |
| Name collisions | Assembly attributes prefix the target type name (e.g. `ButtonClickAction`) |
| Accessibility | Methods must be public/internal; parameter and return types must be accessible (public or internal with `InternalsVisibleTo`) |
| Ambiguity | Literal names with multiple overloads produce XBG007; wildcard/regex still emit XBG007 if multiple overloads remain after filtering |
| Inaccessible matches | Wildcard/regex patterns drop matches that use non-public types and emit XBG014 if nothing accessible remains |
| Static/generic | Static methods and generic type/parameter usage are rejected (diagnostic) |

### XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
    <Button Content="Submit">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <local:SubmitAction TargetObject="{Binding}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

## UI Thread Dispatching

Set `UseDispatcher = true` on the attribute to have the generated action marshal the method invocation to `Dispatcher.UIThread.Post`, mirroring the behavior of `FocusControlAction`. The default is `false`, which executes on the calling thread.

```csharp
[GenerateTypedAction(UseDispatcher = true)]
public void Submit()
{
    // ...
}
```

## Parameter Binding

The generator supports methods with parameters. For each parameter in the method, a corresponding `StyledProperty` is generated on the Action class, allowing you to bind values from XAML.

### Example

```csharp
[GenerateTypedAction]
public void UpdateMessage(string message, int count)
{
    // ...
}
```

### XAML Usage

```xml
<Button Content="Update">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <local:UpdateMessageAction TargetObject="{Binding}" 
                                       Message="Hello World" 
                                       Count="{Binding Count}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

> Ensure `xmlns:local` is defined in your XAML root pointing to the namespace of `UpdateMessageAction`.

### Rich parameter example

```csharp
[GenerateTypedAction]
public void Configure(string title, bool isEnabled, double threshold, TimeSpan timeout, Uri endpoint)
{
    // ...
}
```

```xml
<Button Content="Configure">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <local:ConfigureAction TargetObject="{Binding}"
                                   Title="{Binding PageTitle}"
                                   IsEnabled="True"
                                   Threshold="0.85"
                                   Timeout="00:00:30"
                                   Endpoint="https://api.example.com/" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Async Support

If the target method returns `Task` or `ValueTask`, the generated action handles the async execution. It also exposes an `IsExecuting` property that you can bind to (e.g., to show a loading spinner).

When the async method faults or is canceled, the generated action now captures the exception in `LastError` instead of rethrowing on the UI thread. You can bind to `LastError` to surface errors while keeping the app running.

### Example

```csharp
[GenerateTypedAction]
public async Task SubmitAsync()
{
    await Task.Delay(1000);
}
```

### XAML Usage

```xml
<Button Content="Submit" IsEnabled="{Binding !IsBusy}">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <local:SubmitAsyncAction TargetObject="{Binding}" 
                                     IsExecuting="{Binding IsBusy, Mode=OneWayToSource}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

> Ensure `xmlns:local` is defined in your XAML root pointing to the namespace of `SubmitAsyncAction`.

## Event Handler Signature

If the method matches the standard event handler signature `(object sender, object parameter)` (or `EventArgs`), the generated `Execute` method will pass the sender and parameter directly to the method, similar to how `CallMethodAction` works for event handlers.

```csharp
[GenerateTypedAction]
public void OnClick(object sender, object parameter)
{
    // ...
}
```
