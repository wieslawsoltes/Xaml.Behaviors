# Templates

The Interactivity SDK provides several template classes that allow you to define collections of objects or single objects in XAML resources or styles, and then instantiate them when needed. This is particularly useful for defining behaviors in styles, where a new instance of the behavior collection must be created for each control the style is applied to.

## Available Templates

The following template classes are available in the `Xaml.Behaviors.Interactivity` namespace:

*   **`BehaviorCollectionTemplate`**: Used to create a `BehaviorCollection`.
*   **`ActionCollectionTemplate`**: Used to create an `ActionCollection`.
*   **`NotificationTemplate`**: Used to create an `INotification` object (from `Avalonia.Controls.Notifications`).
*   **`ObjectTemplate`**: A generic template for creating any object.

## Using BehaviorCollectionTemplate

The most common use case for templates is `BehaviorCollectionTemplate`. It allows you to define a set of behaviors within a `Style` and apply them to multiple controls.

### Defining Behaviors in Styles

Since behaviors are stateful objects attached to a specific control, you cannot simply share a single `BehaviorCollection` instance across multiple controls. Instead, you use a `BehaviorCollectionTemplate` in the setter of the `Interaction.Behaviors` property. When the style is applied, the template creates a new `BehaviorCollection` for that specific control.

```xml
<Style Selector="Button.warning">
    <Setter Property="(Interaction.Behaviors)">
        <BehaviorCollectionTemplate>
            <BehaviorCollection>
                <EventTriggerBehavior EventName="PointerEnter">
                    <ChangePropertyAction PropertyName="Background" Value="Orange" />
                </EventTriggerBehavior>
                <EventTriggerBehavior EventName="PointerExited">
                    <ChangePropertyAction PropertyName="Background" Value="Yellow" />
                </EventTriggerBehavior>
            </BehaviorCollection>
        </BehaviorCollectionTemplate>
    </Setter>
</Style>
```

In this example, any `Button` with the `warning` class will automatically get the defined behaviors.

## Other Templates

### ActionCollectionTemplate

`ActionCollectionTemplate` can be used to define a reusable set of actions.

```xml
<UserControl.Resources>
    <ActionCollectionTemplate x:Key="CommonActions">
        <ActionCollection>
            <ChangePropertyAction PropertyName="Opacity" Value="0.5" />
            <!-- Other actions -->
        </ActionCollection>
    </ActionCollectionTemplate>
</UserControl.Resources>
```

### NotificationTemplate

`NotificationTemplate` is useful for defining notifications that can be shown using the `ShowNotificationAction` (if available in your interactions library) or custom actions.

```xml
<NotificationTemplate x:Key="SuccessNotification">
    <Notification Title="Success" Message="Operation completed successfully." Type="Success" />
</NotificationTemplate>
```

### ObjectTemplate

`ObjectTemplate` is a general-purpose template that can be used to instantiate any object graph.

```xml
<ObjectTemplate x:Key="MyObjectFactory">
    <MyCustomObject Property1="Value1" />
</ObjectTemplate>
```
