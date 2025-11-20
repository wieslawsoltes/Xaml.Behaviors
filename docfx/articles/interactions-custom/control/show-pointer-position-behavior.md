# ShowPointerPositionBehavior

This behavior tracks the pointer position over the associated control and updates the `Text` property of a target `TextBlock` with the coordinates.

### Properties
*   `TargetTextBlock`: The `TextBlock` where the coordinates will be displayed.

### Example

```xml
<StackPanel>
    <TextBlock Name="PositionText" />
    
    <Border Background="LightBlue" Width="300" Height="300">
        <Interaction.Behaviors>
            <ShowPointerPositionBehavior TargetTextBlock="{Binding #PositionText}" />
        </Interaction.Behaviors>
    </Border>
</StackPanel>
```
