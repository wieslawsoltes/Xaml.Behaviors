# ViewportBehavior

Listens for the associated element entering or exiting the parent `ScrollViewer` viewport.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| IsAlwaysOn | `bool` | Gets or sets a value indicating whether this behavior will remain attached after the associated element enters the viewport. |
| IsFullyInViewport | `bool` | Gets a value indicating whether the associated element is fully in the `ScrollViewer` viewport. (Read-only) |
| IsInViewport | `bool` | Gets a value indicating whether the associated element is in the `ScrollViewer` viewport. (Read-only) |

## Events

- `EnteredViewport`: Occurs when the associated element has fully entered the viewport.
- `ExitedViewport`: Occurs when the associated element has fully exited the viewport.
- `EnteringViewport`: Occurs when the associated element starts entering the viewport.
- `ExitingViewport`: Occurs when the associated element starts exiting the viewport.

## Usage

```xml
<ScrollViewer>
    <StackPanel>
        <Border Height="100" Background="Red">
            <Interaction.Behaviors>
                <ViewportBehavior IsInViewport="{Binding IsVisibleInViewport, Mode=OneWayToSource}" />
            </Interaction.Behaviors>
        </Border>
        <!-- More items -->
    </StackPanel>
</ScrollViewer>
```
