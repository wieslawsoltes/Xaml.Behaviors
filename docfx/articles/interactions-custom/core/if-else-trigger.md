# IfElseTrigger

The `IfElseTrigger` executes different sets of actions based on a boolean condition.

### Properties
- `Condition`: The boolean condition to evaluate.
- `IfActions`: Actions to execute when `Condition` is true.
- `ElseActions`: Actions to execute when `Condition` is false.

### Usage Example

```xml
<CheckBox IsChecked="{Binding IsEnabled}">
    <Interaction.Behaviors>
        <IfElseTrigger Condition="{Binding IsEnabled}">
            <IfElseTrigger.IfActions>
                <ChangePropertyAction TargetObject="{Binding ElementName=StatusText}" 
                                      PropertyName="Text" Value="Enabled" />
            </IfElseTrigger.IfActions>
            <IfElseTrigger.ElseActions>
                <ChangePropertyAction TargetObject="{Binding ElementName=StatusText}" 
                                      PropertyName="Text" Value="Disabled" />
            </IfElseTrigger.ElseActions>
        </IfElseTrigger>
    </Interaction.Behaviors>
</CheckBox>
```
