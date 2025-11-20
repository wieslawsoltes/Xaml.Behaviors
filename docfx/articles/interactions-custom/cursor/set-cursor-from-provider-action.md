# SetCursorFromProviderAction

Sets the `Cursor` property on a target `InputElement` using a value from an `ICursorProvider`.

### Properties
- `CursorProvider`: The provider to get the cursor from.
- `TargetObject`: The target `InputElement`.

### Usage Example

```xml
<Button Content="Apply Provider Cursor">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetCursorFromProviderAction TargetObject="{Binding ElementName=MyBorder}" 
                                         CursorProvider="{Binding MyCursorProvider}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
