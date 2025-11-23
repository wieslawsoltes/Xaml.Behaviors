# NetworkInformationTrigger

`NetworkInformationTrigger` listens for network availability changes and executes its actions when connectivity is gained or lost.

## Behavior

- Subscribes to `NetworkChange.NetworkAvailabilityChanged` and runs on the UI thread.
- Passes a `bool` parameter (`true` when network is available, `false` otherwise) to attached actions.
- Works well with `InvokeCommandAction` when `PassEventArgsToCommand="True"` is set to forward the availability value to a command.

## Usage

```xml
<Border>
    <Interaction.Triggers>
        <NetworkInformationTrigger>
            <core:InvokeCommandAction Command="{Binding UpdateNetworkStatusCommand}"
                                      PassEventArgsToCommand="True" />
        </NetworkInformationTrigger>
    </Interaction.Triggers>
</Border>
```

In this example, `UpdateNetworkStatusCommand` receives a `bool` indicating whether the network is currently available.
