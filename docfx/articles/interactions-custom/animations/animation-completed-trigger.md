# AnimationCompletedTrigger

Runs a specified `Animation` and executes actions upon completion.

### Example

```xml
<Border Background="Purple" Width="100" Height="100">
    <Border.Resources>
        <Animation x:Key="FadeOut" Duration="0:0:1">
            <KeyFrame Cue="0%">
                <Setter Property="Opacity" Value="1"/>
            </KeyFrame>
            <KeyFrame Cue="100%">
                <Setter Property="Opacity" Value="0"/>
            </KeyFrame>
        </Animation>
    </Border.Resources>

    <Interaction.Behaviors>
        <AnimationCompletedTrigger Animation="{StaticResource FadeOut}">
            <!-- Actions to run after animation completes -->
            <HideControlAction TargetObject="{Binding $parent[Border]}" />
        </AnimationCompletedTrigger>
    </Interaction.Behaviors>
</Border>
```
