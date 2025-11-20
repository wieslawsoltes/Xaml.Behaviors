# PopupAction

The `PopupAction` is a self-contained action that creates and manages its own `Popup`. You define the content of the popup directly inside the action.

```xml
<Button Content="Show Popup">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <PopupAction>
                <Border Background="White" BorderBrush="Black" BorderThickness="1" Padding="10">
                    <TextBlock Text="Hello from Popup!" />
                </Border>
            </PopupAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
