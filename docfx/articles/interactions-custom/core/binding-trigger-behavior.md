# BindingTriggerBehavior

A behavior that listens to a binding and executes actions when the binding updates.

## Uno Platform
WinUI XAML applies a binding to `Binding` instead of assigning it, so on Uno Platform `Binding` is an `object` property
whose value is compared: `Binding="{x:Bind Slider.Value, Mode=OneWay}"`. A `BindingBase` assigned in code is evaluated
like on Avalonia.
