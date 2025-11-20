# WriteableBitmapTrigger

A trigger that executes its actions when `Trigger` is called, passing a `WriteableBitmap` as parameter.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Bitmap | `WriteableBitmap` | Gets or sets the bitmap passed to actions. |

## Methods

| Method | Description |
| --- | --- |
| Trigger() | Manually invokes the trigger. |

## Usage

```xml
<UserControl>
    <Interaction.Behaviors>
        <WriteableBitmapTrigger x:Name="MyTrigger" Bitmap="{Binding MyBitmap}">
            <CallMethodAction TargetObject="{Binding}" MethodName="UpdateBitmap" />
        </WriteableBitmapTrigger>
    </Interaction.Behaviors>
</UserControl>
```
