# WriteableBitmapTimerTrigger

A trigger that fires its actions on a timer and passes a `WriteableBitmap` as parameter.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Bitmap | `WriteableBitmap` | Gets or sets the bitmap passed to actions. |
| MillisecondsPerTick | `int` | Gets or sets the timer interval in milliseconds. Default is 16. |

## Usage

```xml
<UserControl>
    <Interaction.Behaviors>
        <WriteableBitmapTimerTrigger Bitmap="{Binding MyBitmap}" MillisecondsPerTick="33">
            <CallMethodAction TargetObject="{Binding}" MethodName="UpdateBitmap" />
        </WriteableBitmapTimerTrigger>
    </Interaction.Behaviors>
</UserControl>
```
