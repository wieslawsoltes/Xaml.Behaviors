# DragAndDropEventsBehavior

`DragAndDropEventsBehavior` allows you to subscribe to standard Drag and Drop events (`Enter`, `Leave`, `Over`, `Drop`) and execute `IAction`s in response.

## Usage

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
