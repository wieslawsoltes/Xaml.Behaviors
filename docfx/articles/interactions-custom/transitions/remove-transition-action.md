# RemoveTransitionAction

Removes a `TransitionBase` from the `Avalonia.Animation.Transitions` collection on the target element.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Transition | `TransitionBase` | Gets or sets the transition to remove. |
| StyledElement | `StyledElement` | Gets or sets the target styled element. If not set, the sender is used. |

## Usage

```xml
<Button Content="Remove Transition">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RemoveTransitionAction StyledElement="{Binding #MyBorder}" Transition="{Binding MyTransition}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Border x:Name="MyBorder" Background="Red" Width="100" Height="100" />
```

