# ShowOnDoubleTappedBehavior

A behavior that allows to show a control on double tapped event.

## Properties

Inherits properties from [ShowBehaviorBase](show-behavior-base.md).

## Usage

```xml
<Button Content="Double Tap Me">
    <Interaction.Behaviors>
        <ShowOnDoubleTappedBehavior TargetControl="{Binding #MyPopup}" />
    </Interaction.Behaviors>
</Button>

<Border x:Name="MyPopup" IsVisible="False" Background="Red" Width="100" Height="100">
    <TextBlock Text="Hello!" />
</Border>
```
