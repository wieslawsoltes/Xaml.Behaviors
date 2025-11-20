# NetworkStatusTrigger

The `NetworkStatusTrigger` fires when the network availability changes. It can be configured to trigger only when the network becomes `Online`, `Offline`, or on `Any` change.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Status | NetworkStatus | The network status to listen for (`Online`, `Offline`, `Any`). Default is `Any`. |

## Usage

```xml
<UserControl>
    <StackPanel>
        <Interaction.Behaviors>
            <!-- Trigger when Online -->
            <NetworkStatusTrigger Status="Online">
                <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Online" />
            </NetworkStatusTrigger>

            <!-- Trigger when Offline -->
            <NetworkStatusTrigger Status="Offline">
                <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Offline" />
            </NetworkStatusTrigger>
        </Interaction.Behaviors>
        
        <TextBlock Name="StatusText" Text="Unknown" />
    </StackPanel>
</UserControl>
```

## Remarks

This trigger uses `System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged` to detect changes. It posts the execution of actions to the UI thread. Note that the trigger only fires on *changes*, so it won't execute immediately upon attachment based on the current state.
