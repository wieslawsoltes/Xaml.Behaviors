# BindingBehavior

Applies a binding to a target property on a target object when the associated control is attached to the visual tree.

#### Properties
- `TargetObject`: The object that owns the target property.
- `TargetProperty`: The property to bind.
- `Binding`: The binding definition to apply.

#### Uno Platform
WinUI XAML applies a binding to `Binding` instead of assigning it, so on Uno Platform `Binding` is an `object` property
whose value is pushed to the target property (and updated when it changes):

```xml
<icustom:BindingBehavior TargetObject="{x:Bind TargetText}"
                         TargetProperty="{x:Bind mux:TextBlock.TextProperty}"
                         Binding="{x:Bind SourceBox.Text, Mode=OneWay}" />
```

A `BindingBase` assigned in code is applied to the target property like on Avalonia.
