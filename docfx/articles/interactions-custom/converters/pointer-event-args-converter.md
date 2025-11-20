# PointerEventArgsConverter

The `PointerEventArgsConverter` is an `IValueConverter` designed to extract position or delta information from pointer event arguments. This is particularly useful when binding event arguments to a command in a ViewModel.

### Supported Event Arguments

| Event Argument Type | Output | Description |
| :--- | :--- | :--- |
| `PointerPressedEventArgs` | `(double x, double y)` | Returns the pointer position relative to the source visual. |
| `PointerReleasedEventArgs` | `(double x, double y)` | Returns the pointer position relative to the source visual. |
| `PointerDeltaEventArgs` | `(double x, double y)` | Returns the pointer delta (change in position). |

### Usage Example

You can use this converter with an `EventTriggerBehavior` to pass the pointer position to a command.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <UserControl.Resources>
        <PointerEventArgsConverter x:Key="PointerEventArgsConverter" />
    </UserControl.Resources>

    <Border Background="Transparent">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="PointerPressed">
                <InvokeCommandAction Command="{Binding PointerPressedCommand}"
                                     InputConverter="{StaticResource PointerEventArgsConverter}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Border>
</UserControl>
```

In your ViewModel:

```csharp
public void PointerPressedCommand((double x, double y) position)
{
    Console.WriteLine($"Pointer pressed at: {position.x}, {position.y}");
}
```
