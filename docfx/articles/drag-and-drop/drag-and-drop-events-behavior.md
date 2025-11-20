# DragAndDropEventsBehavior

`DragAndDropEventsBehavior` allows you to subscribe to standard Drag and Drop events (`Enter`, `Leave`, `Over`, `Drop`) and execute `IAction`s in response.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetControl | `Control` | Gets or sets the control that is used as source for drag and drop events. |

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <DragAndDropEventsBehavior>
            <EventTriggerBehavior EventName="Drop">
                <InvokeCommandAction Command="{Binding DropCommand}" />
            </EventTriggerBehavior>
        </DragAndDropEventsBehavior>
    </Interaction.Behaviors>
</Border>
```

```xml
<Border Background="LightBlue">
    <Interaction.Behaviors>
        <DragAndDropEventsBehavior>
            <DragAndDropEventsBehavior.Enter>
                <ChangePropertyAction PropertyName="Background" Value="Yellow" />
            </DragAndDropEventsBehavior.Enter>
            <DragAndDropEventsBehavior.Leave>
                <ChangePropertyAction PropertyName="Background" Value="LightBlue" />
            </DragAndDropEventsBehavior.Leave>
            <DragAndDropEventsBehavior.Drop>
                <InvokeCommandAction Command="{Binding DropCommand}" />
            </DragAndDropEventsBehavior.Drop>
        </DragAndDropEventsBehavior>
    </Interaction.Behaviors>
    <TextBlock Text="Drag over me" />
</Border>
```
