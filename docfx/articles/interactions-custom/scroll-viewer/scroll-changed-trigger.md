# ScrollChangedTrigger

Executes actions when the `ScrollViewer.ScrollChanged` event occurs.

## Usage

```xml
<ScrollViewer>
    <Interaction.Behaviors>
        <ScrollChangedTrigger>
            <InvokeCommandAction Command="{Binding ScrollChangedCommand}" />
        </ScrollChangedTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</ScrollViewer>
```
