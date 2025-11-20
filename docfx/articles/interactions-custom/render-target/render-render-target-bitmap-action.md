# RenderRenderTargetBitmapAction

`RenderRenderTargetBitmapAction` is an action that invokes `IRenderTargetBitmapRenderHost.Render` on the specified target.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Target | `IRenderTargetBitmapRenderHost` | Gets or sets the render host. |

## Usage

```xml
<Button Content="Render">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RenderRenderTargetBitmapAction Target="{Binding ElementName=MyBehavior}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Image Width="200" Height="200">
    <Interaction.Behaviors>
        <StaticRenderTargetBitmapBehavior x:Name="MyBehavior" ... />
    </Interaction.Behaviors>
</Image>
```
