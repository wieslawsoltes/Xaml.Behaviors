# RenderTargetBitmapTrigger

`RenderTargetBitmapTrigger` is a trigger that calls `IRenderTargetBitmapRenderHost.Render` periodically.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Target | `IRenderTargetBitmapRenderHost` | Gets or sets the render host that should be rendered. |
| MillisecondsPerTick | `int` | Gets or sets the interval, in milliseconds, between render ticks. Default is 16. |

## Usage

```xml
<Image>
    <Interaction.Behaviors>
        <StaticRenderTargetBitmapBehavior x:Name="MyBehavior" ... />
        <RenderTargetBitmapTrigger Target="{Binding ElementName=MyBehavior}" MillisecondsPerTick="33" />
    </Interaction.Behaviors>
</Image>
```
