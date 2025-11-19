# TaskCompletedTrigger

`TaskCompletedTrigger` is a trigger that invokes its actions when a specified `Task` completes. This is useful for triggering UI updates or animations after an asynchronous operation finishes.

## Properties

*   **`Task`**: The `Task` to monitor. When this task completes, the actions associated with the trigger are executed.

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="Sample.Views.MainView">
    <StackPanel>
        <TextBlock Text="Loading...">
            <Interaction.Behaviors>
                <TaskCompletedTrigger Task="{Binding MyLoadingTask}">
                    <ChangePropertyAction PropertyName="Text" Value="Loaded!" />
                </TaskCompletedTrigger>
            </Interaction.Behaviors>
        </TextBlock>
    </StackPanel>
</UserControl>
```
