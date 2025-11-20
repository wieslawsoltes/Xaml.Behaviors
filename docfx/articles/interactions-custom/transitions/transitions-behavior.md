# TransitionsBehavior

Sets the `Avalonia.Animation.Transitions` collection on the associated control when attached.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TransitionsSource | `Transitions` | Gets or sets the transitions collection to apply. |

## Usage

```xml
<Border Background="Red" Width="100" Height="100">
    <Interaction.Behaviors>
        <TransitionsBehavior>
            <TransitionsBehavior.TransitionsSource>
                <Transitions>
                    <DoubleTransition Property="Opacity" Duration="0:0:1" />
                </Transitions>
            </TransitionsBehavior.TransitionsSource>
        </TransitionsBehavior>
    </Interaction.Behaviors>
</Border>
```

## Usage

```xml
<Border Background="Red" Width="100" Height="100">
    <Interaction.Behaviors>
        <TransitionsBehavior>
            <TransitionsBehavior.TransitionsSource>
                <Transitions>
                    <DoubleTransition Property="Opacity" Duration="0:0:1" />
                </Transitions>
            </TransitionsBehavior.TransitionsSource>
        </TransitionsBehavior>
    </Interaction.Behaviors>
</Border>
```

