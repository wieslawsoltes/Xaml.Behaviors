# RightTappedGestureTrigger

A trigger that executes when a right tap gesture is recognized on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <RightTappedGestureTrigger>
            <InvokeCommandAction Command="{Binding RightTapCommand}" />
        </RightTappedGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Right Tap Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
