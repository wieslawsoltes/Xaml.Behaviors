# SetPathIconDataAction

Sets the `Data` property of a target `PathIcon` to the specified `Geometry`.

### Properties
- `Data`: The `Geometry` to apply.
- `TargetObject`: The target `PathIcon`. If not set, uses the sender.

### Usage Example

```xml
<Button Content="Change Icon">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetPathIconDataAction TargetObject="{Binding ElementName=MyPathIcon}" 
                                   Data="{StaticResource NewIconGeometry}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
