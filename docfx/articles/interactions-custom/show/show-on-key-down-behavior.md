# ShowOnKeyDownBehavior

A behavior that allows to show a control on key down event.

## Properties

Inherits properties from [ShowBehaviorBase](show-behavior-base.md).

| Property | Type | Description |
| --- | --- | --- |
| Key | `Key?` | Gets or sets the key used to trigger the behavior. |
| Gesture | `KeyGesture?` | Gets or sets the key gesture used to trigger the behavior. |

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <ShowOnKeyDownBehavior Key="Enter" TargetControl="{Binding #MyPopup}" />
    </Interaction.Behaviors>
</TextBox>

<Border x:Name="MyPopup" IsVisible="False" Background="Red" Width="100" Height="100">
    <TextBlock Text="Hello!" />
</Border>
```
