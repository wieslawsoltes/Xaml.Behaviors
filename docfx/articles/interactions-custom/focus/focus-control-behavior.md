# FocusControlBehavior

Sets focus on the associated control when the `FocusFlag` property becomes `true`. This is typically a one-way trigger.

### Properties
- `FocusFlag`: A boolean flag. When it changes to `true`, focus is requested.

### Usage Example

```xml
<Button Content="Submit">
    <Interaction.Behaviors>
        <FocusControlBehavior FocusFlag="{Binding HasErrors}" />
    </Interaction.Behaviors>
</Button>
```
