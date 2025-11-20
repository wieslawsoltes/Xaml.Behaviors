# SetEnabledAction

Enables or disables a control.

```xml
<CheckBox Content="Enable Submit" Name="EnableCheckBox" />

<Button Content="Submit">
    <Interaction.Behaviors>
        <DataTriggerBehavior Binding="{Binding #EnableCheckBox.IsChecked}" Value="True">
            <SetEnabledAction Value="True" />
        </DataTriggerBehavior>
        <DataTriggerBehavior Binding="{Binding #EnableCheckBox.IsChecked}" Value="False">
            <SetEnabledAction Value="False" />
        </DataTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
