# TransitionsChangedTrigger

Executes actions whenever the `Avalonia.Animation.Transitions` collection changes.

## Usage

```xml
<Border Background="Red" Width="100" Height="100">
    <Interaction.Behaviors>
        <TransitionsChangedTrigger>
            <InvokeCommandAction Command="{Binding TransitionsChangedCommand}" />
        </TransitionsChangedTrigger>
    </Interaction.Behaviors>
</Border>
```

