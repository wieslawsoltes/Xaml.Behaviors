# LogAction

The `LogAction` logs a message to the debug output (using `System.Diagnostics.Trace`) and the console. It is useful for debugging XAML triggers and behaviors without needing to set breakpoints in code-behind.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Message | string | The message format string. |
| Argument | object | An optional argument to format the message with. |
| Level | LogActionLevel | The severity level of the log (Info, Warning, Error, Debug). Default is Info. |

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <LogAction Message="Button clicked at {0}" Argument="{Binding $parent[Window].Clock.Now}" Level="Info" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Remarks

The action uses `Trace.TraceInformation`, `Trace.TraceWarning`, `Trace.TraceError`, or `Debug.WriteLine` depending on the `Level`. It also writes to `Console.WriteLine` for visibility in console applications.
