# SetAutomationIdAction

This action sets the `AutomationProperties.AutomationId` on a target control. This can be useful for dynamically assigning IDs during runtime interactions, which can then be used by UI test automation scripts to locate elements.

### Properties
*   `TargetControl`: The control to modify. If not set, it targets the associated object.
*   `AutomationId`: The ID string to assign.

### Example

```xml
<Button Content="Generate ID">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetAutomationIdAction TargetControl="{Binding #MyDynamicPanel}" 
                                   AutomationId="DynamicPanel_123" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<StackPanel Name="MyDynamicPanel" />
```
