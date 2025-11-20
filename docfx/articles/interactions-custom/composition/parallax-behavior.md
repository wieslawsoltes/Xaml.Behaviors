# ParallaxBehavior

A behavior that moves the associated element at a different speed than the scrolling container, creating a parallax depth effect.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| SourceScrollViewer | ScrollViewer | The source ScrollViewer to track. If not set, the behavior attempts to find a parent ScrollViewer. |
| ParallaxRatio | double | The ratio of movement relative to the scroll offset. 0.0 is static, 1.0 moves with scroll. Default is 0.2. |

## Usage

```xml
<ScrollViewer x:Name="MyScroller">
    <Grid>
        <!-- Background element moving slower -->
        <Image Source="background.png">
            <Interaction.Behaviors>
                <ParallaxBehavior SourceScrollViewer="{Binding #MyScroller}" ParallaxRatio="0.5" />
            </Interaction.Behaviors>
        </Image>
        
        <!-- Foreground content -->
        <StackPanel>
            <!-- ... -->
        </StackPanel>
    </Grid>
</ScrollViewer>
```
