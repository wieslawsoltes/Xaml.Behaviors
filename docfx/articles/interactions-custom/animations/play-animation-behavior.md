# PlayAnimationBehavior

Runs a standard Avalonia `Animation` defined in XAML.

### Example

```xml
<Border Background="Blue" Width="100" Height="100">
    <Border.Resources>
        <Animation x:Key="SlideIn" Duration="0:0:1">
            <KeyFrame Cue="0%">
                <Setter Property="TranslateTransform.X" Value="-100"/>
            </KeyFrame>
            <KeyFrame Cue="100%">
                <Setter Property="TranslateTransform.X" Value="0"/>
            </KeyFrame>
        </Animation>
    </Border.Resources>
    
    <Interaction.Behaviors>
        <PlayAnimationBehavior Animation="{StaticResource SlideIn}" />
    </Interaction.Behaviors>
</Border>
```
