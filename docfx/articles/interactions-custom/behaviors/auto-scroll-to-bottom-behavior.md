# AutoScrollToBottomBehavior

`AutoScrollToBottomBehavior` automatically scrolls a `ScrollViewer` to the bottom when new items are added to its content.

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ScrollViewer>
        <Interaction.Behaviors>
            <AutoScrollToBottomBehavior />
        </Interaction.Behaviors>
        <ItemsControl ItemsSource="{Binding Logs}" />
    </ScrollViewer>
</UserControl>
```
