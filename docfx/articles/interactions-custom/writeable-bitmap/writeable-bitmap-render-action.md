# WriteableBitmapRenderAction

Invokes an `IWriteableBitmapRenderer` to render into a bitmap.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Renderer | `IWriteableBitmapRenderer` | Gets or sets the renderer used when executing the action. |
| Bitmap | `WriteableBitmap` | Gets or sets the target bitmap. |

## Usage

```xml
<Button Content="Render">
    <Interaction.Actions>
        <EventTriggerBehavior EventName="Click">
            <WriteableBitmapRenderAction Renderer="{Binding MyRenderer}" Bitmap="{Binding MyBitmap}" />
        </EventTriggerBehavior>
    </Interaction.Actions>
</Button>
```
