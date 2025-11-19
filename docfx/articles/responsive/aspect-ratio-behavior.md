# AspectRatioBehavior

`AspectRatioBehavior` allows you to apply or remove style classes (or pseudo-classes) to a control based on the aspect ratio (width divided by height) of a source control.

## Properties

*   **`SourceControl`**: The control whose bounds are observed. If not set, the `AssociatedObject` is used.
*   **`TargetControl`**: The control to which the classes are applied. If not set, the `AssociatedObject` is used.
*   **`Setters`**: A collection of `AspectRatioClassSetter` objects that define the conditions and classes to apply.

## AspectRatioClassSetter

`AspectRatioClassSetter` defines a rule based on the aspect ratio. It has the following properties:

*   **`ClassName`**: The name of the class (or pseudo-class) to apply.
*   **`IsPseudoClass`**: If `True`, `ClassName` is treated as a pseudo-class.
*   **`MinRatio`**: The minimum aspect ratio required.
*   **`MaxRatio`**: The maximum aspect ratio required.
*   **`TargetControl`**: An optional specific target control for this setter.

You can also customize the comparison operators using `MinRatioOperator` and `MaxRatioOperator`.

## Usage

This example changes the background color based on whether the control is in "landscape" (ratio > 1) or "portrait" (ratio <= 1) mode.

```xml
<Border>
    <Border.Styles>
        <Style Selector="Border.landscape">
            <Setter Property="Background" Value="LightBlue" />
        </Style>
        <Style Selector="Border.portrait">
            <Setter Property="Background" Value="LightPink" />
        </Style>
    </Border.Styles>
    
    <Interaction.Behaviors>
        <AspectRatioBehavior>
            <AspectRatioClassSetter MinRatio="1.0" MinRatioOperator="GreaterThan" ClassName="landscape" />
            <AspectRatioClassSetter MaxRatio="1.0" MaxRatioOperator="LessThanOrEqual" ClassName="portrait" />
        </AspectRatioBehavior>
    </Interaction.Behaviors>
    
    <TextBlock Text="Resize me!" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
