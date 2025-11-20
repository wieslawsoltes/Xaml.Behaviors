# AddTransitionAction

Adds a `TransitionBase` to the `Avalonia.Animation.Transitions` collection on the target element.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Transition | `TransitionBase` | Gets or sets the transition to add. |
| StyledElement | `StyledElement` | Gets or sets the target styled element. If not set, the sender is used. |

## Usage

```xml
<Button Content="Add Transition">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <AddTransitionAction StyledElement="{Binding #MyBorder}">
                <AddTransitionAction.Transition>
                    <DoubleTransition Property="Opacity" Duration="0:0:1" />
                </AddTransitionAction.Transition>
            </AddTransitionAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Border x:Name="MyBorder" Background="Red" Width="100" Height="100" />
```

## Usage

```xml
<Button Content="Add Transition">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <AddTransitionAction>
                <AddTransitionAction.Transition>
                    <DoubleTransition Property="Opacity" Duration="0:0:0.5" />
                </AddTransitionAction.Transition>
            </AddTransitionAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

