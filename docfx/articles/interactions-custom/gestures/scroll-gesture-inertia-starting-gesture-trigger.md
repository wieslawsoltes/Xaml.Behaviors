# ScrollGestureInertiaStartingGestureTrigger

A trigger that executes when scroll gesture inertia starts on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <ScrollGestureInertiaStartingGestureTrigger>
            <InvokeCommandAction Command="{Binding ScrollInertiaStartingCommand}" />
        </ScrollGestureInertiaStartingGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Scroll Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
