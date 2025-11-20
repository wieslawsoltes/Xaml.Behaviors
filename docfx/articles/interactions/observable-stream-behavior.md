# ObservableStreamBehavior

The `ObservableStreamBehavior` allows you to subscribe to an `IObservable<T>` directly in XAML and execute actions when the stream emits a value, an error, or completes. This is useful for reacting to raw streams (e.g., from a service or a reactive property) without needing to manually subscribe in the ViewModel or code-behind.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Source` | `object` | The `IObservable<T>` to subscribe to. |
| `Actions` | `ActionCollection` | Actions to execute when `OnNext` is received. The value emitted is passed as the parameter to the actions. |
| `ErrorActions` | `ActionCollection` | Actions to execute when `OnError` is received. The exception is passed as the parameter. |
| `CompletedActions` | `ActionCollection` | Actions to execute when `OnCompleted` is received. |

## Example

```xml
<TextBlock Name="StatusText" Text="Waiting...">
    <Interaction.Behaviors>
        <ObservableStreamBehavior Source="{Binding MyDataStream}">
            <ObservableStreamBehavior.Actions>
                <ChangePropertyAction TargetObject="{Binding #StatusText}" 
                                      PropertyName="Text" 
                                      Value="{Binding StringFormat='Received: {0}'}" />
            </ObservableStreamBehavior.Actions>
            
            <ObservableStreamBehavior.ErrorActions>
                <ChangePropertyAction TargetObject="{Binding #StatusText}" 
                                      PropertyName="Text" 
                                      Value="An error occurred!" />
                <ChangePropertyAction TargetObject="{Binding #StatusText}" 
                                      PropertyName="Foreground" 
                                      Value="Red" />
            </ObservableStreamBehavior.ErrorActions>
            
            <ObservableStreamBehavior.CompletedActions>
                <ChangePropertyAction TargetObject="{Binding #StatusText}" 
                                      PropertyName="Text" 
                                      Value="Stream Completed." />
            </ObservableStreamBehavior.CompletedActions>
        </ObservableStreamBehavior>
    </Interaction.Behaviors>
</TextBlock>
```

## Notes

- The behavior automatically disposes of the subscription when it is detached from the logical tree or when the `Source` property changes.
- Actions are executed on the UI thread.
