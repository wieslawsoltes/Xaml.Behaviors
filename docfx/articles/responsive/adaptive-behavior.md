# AdaptiveBehavior

`AdaptiveBehavior` allows you to apply or remove style classes (or pseudo-classes) to a control based on the dimensions (width and height) of a source control. This is essential for creating responsive layouts that adapt to different screen sizes.

## Properties

*   **`SourceControl`**: The control whose bounds are observed. If not set, the `AssociatedObject` is used.
*   **`TargetControl`**: The control to which the classes are applied. If not set, the `AssociatedObject` is used.
*   **`Setters`**: A collection of `AdaptiveClassSetter` objects that define the conditions and classes to apply.

## AdaptiveClassSetter

`AdaptiveClassSetter` defines a rule for applying a class. It has the following properties:

*   **`ClassName`**: The name of the class (or pseudo-class) to apply when the conditions are met.
*   **`IsPseudoClass`**: If `True`, `ClassName` is treated as a pseudo-class (e.g., `:small`). If `False` (default), it is treated as a style class (e.g., `.small`).
*   **`MinWidth`**: The minimum width required to apply the class.
*   **`MaxWidth`**: The maximum width required to apply the class.
*   **`MinHeight`**: The minimum height required to apply the class.
*   **`MaxHeight`**: The maximum height required to apply the class.
*   **`TargetControl`**: An optional specific target control for this setter.

You can also customize the comparison operators using `MinWidthOperator`, `MaxWidthOperator`, etc. (e.g., `GreaterThan`, `LessThanOrEqual`).

## Usage

In this example, the `Border` will have the `small` class when its width is less than 500 pixels, and the `large` class when its width is 500 pixels or more.

```xml
<Border>
    <Border.Styles>
        <Style Selector="Border.small">
            <Setter Property="Background" Value="Red" />
        </Style>
        <Style Selector="Border.large">
            <Setter Property="Background" Value="Green" />
        </Style>
    </Border.Styles>
    
    <Interaction.Behaviors>
        <AdaptiveBehavior>
            <AdaptiveClassSetter MinWidth="0" MaxWidth="500" ClassName="small" />
            <AdaptiveClassSetter MinWidth="500" MinWidthOperator="GreaterThanOrEqual" ClassName="large" />
        </AdaptiveBehavior>
    </Interaction.Behaviors>
    
    <TextBlock Text="Resize me!" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```

### Using Pseudo-Classes

You can also use pseudo-classes for cleaner styling.

```xml
<Border>
    <Border.Styles>
        <Style Selector="Border:compact">
            <Setter Property="Background" Value="Yellow" />
        </Style>
    </Border.Styles>
    
    <Interaction.Behaviors>
        <AdaptiveBehavior>
            <AdaptiveClassSetter MaxWidth="400" ClassName="compact" IsPseudoClass="True" />
        </AdaptiveBehavior>
    </Interaction.Behaviors>
</Border>
```
