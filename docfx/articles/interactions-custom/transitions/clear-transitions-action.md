# ClearTransitionsAction

Clears the `Avalonia.Animation.Transitions` collection.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| StyledElement | `StyledElement` | Gets or sets the target styled element. If not set, the sender is used. |

## Usage

```xml
<Button Content="Clear Transitions">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ClearTransitionsAction StyledElement="{Binding #MyBorder}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Border x:Name="MyBorder" Background="Red" Width="100" Height="100">
    <Border.Transitions>
        <Transitions>
            <DoubleTransition Property="Opacity" Duration="0:0:1" />
        </Transitions>
    </Border.Transitions>
</Border>
```

## Usage

```xml
<Button Content="Clear Transitions">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ClearTransitionsAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

