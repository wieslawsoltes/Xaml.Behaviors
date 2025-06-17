# XAML Behaviors


[![Build Status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/wieslawsoltes.AvaloniaBehaviors?repoName=wieslawsoltes%2FAvaloniaBehaviors&branchName=master)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=90&repoName=wieslawsoltes%2FAvaloniaBehaviors&branchName=master)
[![CI](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Avalonia.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia)
[![NuGet](https://img.shields.io/nuget/dt/Xaml.Behaviors.Interactivity.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity)
[![MyGet](https://img.shields.io/myget/xamlbehaviors-nightly/vpre/Xaml.Behaviors.Avalonia.svg?label=myget)](https://www.myget.org/gallery/xamlbehaviors-nightly) 

**Xaml Behaviors** is a port of [Windows UWP](https://github.com/Microsoft/XamlBehaviors) version of XAML Behaviors for [Avalonia](https://github.com/AvaloniaUI/Avalonia) XAML.

Avalonia XAML Behaviors is an easy-to-use means of adding common and reusable interactivity to your [Avalonia](https://github.com/AvaloniaUI/Avalonia) applications with minimal code. Avalonia port is available only for managed applications. Use of XAML Behaviors is governed by the MIT License. 

<a href='https://www.youtube.com/watch?v=pffBS-yQ_uM' target='_blank'>![](https://i.ytimg.com/vi/pffBS-yQ_uM/hqdefault.jpg)<a/>

## Building Avalonia XAML Behaviors

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/Xaml.Behaviors.git
```

### Build on Windows using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=windows).

Open up a command-prompt and execute the commands:
```
.\build.ps1
```

### Build on Linux using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=linux).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

### Build on OSX using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=macos).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

## NuGet

Avalonia XamlBehaviors is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Xaml.Behaviors/) and install the package like this:

`Install-Package Xaml.Behaviors`

or by using nightly build feed:
* Add `https://www.myget.org/F/xamlbehaviors-nightly/api/v2` to your package sources
* Alternative nightly build feed `https://pkgs.dev.azure.com/wieslawsoltes/GitHub/_packaging/Nightly/nuget/v3/index.json`
* Update your package using `XamlBehaviors` feed

and install the package like this:

`Install-Package Xaml.Behaviors -Pre`

### Package Sources

* https://api.nuget.org/v3/index.json

### Available Packages

| Package | Description |
|---------|-------------|
| [Xaml.Behaviors](https://www.nuget.org/packages/Xaml.Behaviors) | Complete library of behaviors, actions and triggers for Avalonia applications. |
| [Xaml.Behaviors.Avalonia](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia) | Meta package that bundles all Avalonia XAML Behaviors for easy installation. |
| [Xaml.Behaviors.Interactivity](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity) | Foundation library providing base classes for actions, triggers and behaviors. |
| [Xaml.Behaviors.Interactions](https://www.nuget.org/packages/Xaml.Behaviors.Interactions) | Core collection of common triggers and actions for Avalonia. |
| [Xaml.Behaviors.Interactions.Custom](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom) | Custom behaviors and actions for common Avalonia controls. |
| [Xaml.Behaviors.Interactions.DragAndDrop](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop) | Behaviors that enable drag-and-drop support in Avalonia. |
| [Xaml.Behaviors.Interactions.Draggable](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable) | Draggable element behaviors for moving controls around. |
| [Xaml.Behaviors.Interactions.Events](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events) | Behaviors responding to Avalonia input and focus events. |
| [Xaml.Behaviors.Interactions.ReactiveUI](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI) | Behaviors integrating ReactiveUI navigation patterns. |
| [Xaml.Behaviors.Interactions.Responsive](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive) | Behaviors to assist with responsive and adaptive UI layouts. |
| [Xaml.Behaviors.Interactions.Scripting](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting) | Execute C# scripts at runtime to add dynamic behavior. |

## Getting Started

### Adding Behaviors in XAML

The steps below show how a new user can attach their first behavior. These examples assume you have created an Avalonia application and want to enhance it with interactions.

1. **Install the package** – open your NuGet package manager and add `Xaml.Behaviors` or any of the interaction packages such as `Xaml.Behaviors.Interactions`.
2. **Declare the namespace** – reference the default Avalonia namespace and the XAML Behaviors namespace at the top of your XAML file.
3. **Attach a behavior** – use the `Interaction.Behaviors` attached property to add behaviors to your controls.

Here is a minimal example:

```xaml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Click">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <InvokeCommandAction Command="{Binding ClickCommand}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

### Basic Examples

The examples below walk through typical scenarios where behaviors remove the need for boilerplate code.

1. **React to data changes** – monitor a bound property and update another control automatically.
2. **Handle lifecycle events** – execute actions when a control appears or is loaded.

In XAML this looks like:

```xaml
<!-- Change text based on slider value -->
<TextBlock x:Name="Output">
    <Interaction.Behaviors>
        <DataTriggerBehavior Binding="{Binding #Slider.Value}" ComparisonCondition="GreaterThan" Value="50">
            <ChangePropertyAction PropertyName="Text" Value="High" />
        </DataTriggerBehavior>
        <DataTriggerBehavior Binding="{Binding #Slider.Value}" ComparisonCondition="LessThanOrEqual" Value="50">
            <ChangePropertyAction PropertyName="Text" Value="Low" />
        </DataTriggerBehavior>
    </Interaction.Behaviors>
</TextBlock>

<!-- Focus a control when it appears -->
<TextBox>
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Loaded">
            <FocusControlAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</TextBox>
```

### How It Works

Behind the scenes behaviors rely on Avalonia's attached property and reactive object model. When you set `Interaction.Behaviors` on any `AvaloniaObject`, Avalonia's property system fires a property changed notification. The `Interaction` helper class listens for these changes and attaches each entry of the `BehaviorCollection` to the target control. Each behavior implements `IAttachedObject` and inherits from `Behavior` or `Behavior<T>` which provide `OnAttached` and `OnDetaching` methods. These methods are invoked by the framework so the behavior can subscribe to routed events, bind to properties or manipulate the visual tree. Because behaviors live in a collection you can add or remove them dynamically at runtime just like any other Avalonia property.

Triggers and actions follow the same pattern. A `Trigger` holds a collection of `IAction` objects. When the trigger condition is met—for example an event fires or a bound property changes—the trigger iterates its actions and calls their `Execute` method. Everything relies on base types such as `AvaloniaObject`, the binding engine and the routing/event system, so no reflection or markup extension magic is needed. Understanding these building blocks helps when creating custom behaviors or when troubleshooting how they interact with your controls.

### Advanced Usage

Behaviors can also be defined in styles or extended by creating your own implementations. Advanced scenarios typically involve keeping XAML clean or encapsulating reusable interaction logic.

1. **Attach behaviors from styles** – ideal when the same behavior should apply to multiple controls.
2. **Create custom behaviors** – extend `Behavior<T>` to implement unique interactions.

Below is a walkthrough of both approaches.

**Defining behaviors in a style**

```xaml
<Style Selector="Button.alert">
    <Setter Property="(Interaction.Behaviors)">
        <BehaviorCollectionTemplate>
            <BehaviorCollection>
                <EventTriggerBehavior EventName="PointerEnter">
                    <ChangePropertyAction PropertyName="Background" Value="Red" />
                </EventTriggerBehavior>
            </BehaviorCollection>
        </BehaviorCollectionTemplate>
    </Setter>
</Style>
```

To use the style, assign the `alert` class to any `Button` and it will change its background when the pointer enters the control.

**Creating a custom behavior**

```csharp
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

public class RotateOnClickBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        AssociatedObject.PointerPressed += OnPointerPressed;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PointerPressed -= OnPointerPressed;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        AssociatedObject.RenderTransform = new RotateTransform(45);
    }
}
```

1. **Derive from `Behavior<T>`** – specify the control type the behavior will attach to.
2. **Override `OnAttached` and `OnDetaching`** – wire up and remove event handlers or other logic.
3. **Consume the behavior** – add it in XAML like any built‑in behavior or through a style.

Custom triggers and actions follow the same pattern—derive from `Trigger` or `Action` and override the relevant methods. Use these techniques to build a library of reusable interactions tailored to your application.

## Docs

This section provides an overview of all available classes and their purpose in Avalonia XAML Behaviors. The classes are grouped into logical categories based on their functionality.

## Interactions

### Actions
- **AddClassAction**  
  *Adds a style class to a control’s class collection, making it easy to change the appearance dynamically.*

- **ChangeAvaloniaPropertyAction**  
  *Changes the value of an Avalonia property on a target object at runtime.*

- **CloseNotificationAction**  
  *Closes an open notification, for example, from a notification control.*

- **FocusControlAction**
  *Sets the keyboard focus on a specified control or the associated control.*

- **HideControlAction**
  *Hides a control by setting its `IsVisible` property to false.*

- **ShowControlAction**
  *Shows a control and gives it focus when executed.*

- **PopupAction**
  *Displays a popup window for showing additional UI content.*
- **ShowPopupAction**
  *Opens an existing popup for a control.*
- **HidePopupAction**
  *Closes an existing popup associated with a control.*

- **ShowFlyoutAction**
  *Shows a flyout attached to a control or specified explicitly.*

- **HideFlyoutAction**
  *Hides a flyout attached to a control or specified explicitly.*

- **RemoveClassAction**
  *Removes a style class from a control’s class collection.*

- **RemoveElementAction**
  *Removes a control from its parent container when executed.*

- **ShowContextMenuAction**
  *Displays a control's context menu programmatically.*

- **SplitViewTogglePaneAction**
  *Toggles the `IsPaneOpen` state of a `SplitView`.*

### Animations
- **FadeInBehavior**
  *Animates the fade-in effect for the associated element, gradually increasing its opacity.*

- **StartAnimationAction**
  *Triggers a defined animation on the target control when executed.*
- **AnimateOnAttachedBehavior**
  *Runs a specified animation when the control appears in the visual tree.*
- **StartBuiltAnimationAction**
  *Creates an animation in code using an <code>IAnimationBuilder</code> and starts it.*
- **RunAnimationTrigger**
  *Runs an animation and invokes actions when it completes.*
- **IAnimationBuilder**
  *Interface for providing animations from view models in an MVVM friendly way.*

- **PlayAnimationBehavior**
  *Runs a supplied animation automatically when the control appears in the visual tree.*

- **BeginAnimationAction**
  *Starts an animation on a specified control, allowing the target to be chosen explicitly.*

- **AnimationCompletedTrigger**
  *Plays an animation and invokes actions once the animation finishes running.*

### Transitions
- **TransitionsBehavior**
  *Sets the `Transitions` collection on the associated control when attached.*
- **AddTransitionAction**
  *Adds a transition to a control's `Transitions` collection.*
- **RemoveTransitionAction**
  *Removes a transition from a control's `Transitions` collection.*
- **ClearTransitionsAction**
  *Clears all transitions from a control.*
- **TransitionsChangedTrigger**
  *Triggers actions when the `Transitions` property of a control changes.*

### AutoCompleteBox
- **FocusAutoCompleteBoxTextBoxBehavior**
  *Ensures the text box within an AutoCompleteBox gets focus automatically.*

### Carousel
- **CarouselKeyNavigationBehavior**
  *Enables keyboard navigation for a Carousel using arrow keys.*
- **CarouselNextAction**
  *Advances the target Carousel to the next page.*
- **CarouselPreviousAction**
  *Moves the target Carousel to the previous page.*
- **CarouselSelectionChangedTrigger**
  *Triggers actions when the Carousel selection changes.*

### Button
- **ButtonClickEventTriggerBehavior**  
  *Listens for a button’s click event and triggers associated actions.*

- **ButtonExecuteCommandOnKeyDownBehavior**  
  *Executes a command when a specified key is pressed while the button is focused.*

- **ButtonHideFlyoutBehavior**  
  *Hides an attached flyout when the button is interacted with.*

- **ButtonHideFlyoutOnClickBehavior**
  *Automatically hides the flyout attached to the button when it is clicked.*
- **ButtonHidePopupOnClickBehavior**
  *Automatically closes the popup containing the button when it is clicked.*

### Clipboard
- **ClearClipboardAction**  
  *Clears all contents from the system clipboard.*

- **GetClipboardDataAction**  
  *Retrieves data from the clipboard in a specified format.*

- **GetClipboardFormatsAction**  
  *Retrieves the list of available formats from the clipboard.*

- **GetClipboardTextAction**  
  *Retrieves plain text from the clipboard.*

- **SetClipboardDataObjectAction**  
  *Places a custom data object onto the clipboard.*

- **SetClipboardTextAction**  
  *Places text onto the clipboard.*

### Notifications
- **NotificationManagerBehavior**
  *Creates a notification manager for a window when attached.*
- **ShowNotificationAction**
  *Shows a notification using an attached notification manager.*
- **ShowInformationNotificationAction**
  *Displays an information notification via a manager.*
- **ShowSuccessNotificationAction**
  *Displays a success notification via a manager.*
- **ShowWarningNotificationAction**
  *Displays a warning notification via a manager.*
- **ShowErrorNotificationAction**
  *Displays an error notification via a manager.*

### Composition
- **SelectingItemsControlBehavior**  
  *Animates selection transitions in items controls such as ListBox or TabControl.*

- **SlidingAnimation**
  *Provides static methods to apply sliding animations (from left, right, top, or bottom) to a control.*
- **FluidMoveBehavior**
  *Animates layout changes of a control or its children.*

### Control
- **BindPointerOverBehavior**  
  *Two‑way binds a boolean property to a control’s pointer-over state.*

- **BindTagToVisualRootDataContextBehavior**  
  *Binds the control’s Tag property to the DataContext of its visual root, enabling inherited data contexts.*

- **BoundsObserverBehavior**  
  *Observes a control’s bounds changes and updates two‑way bound Width and Height properties.*

- **DragControlBehavior**  
  *Enables a control to be moved (dragged) around by changing its RenderTransform during pointer events.*

- **HideAttachedFlyoutBehavior**  
  *Hides a flyout that is attached to a control when a condition is met.*

- **HideOnKeyPressedBehavior**  
  *Hides the target control when a specified key is pressed.*

- **HideOnLostFocusBehavior**
  *Hides the target control when it loses focus.*

- **InlineEditBehavior**
  *Toggles display and edit controls to enable in-place text editing.*

- **ShowPointerPositionBehavior**
  *Displays the current pointer position (x, y coordinates) in a TextBlock for debugging or UI feedback.*

- **SetCursorBehavior**
  *Applies a custom cursor to the associated control.*

- **PointerOverCursorBehavior**
  *Changes the cursor while the pointer is over the control and resets it on exit.*

- **SetCursorFromProviderBehavior**
  *Uses an `ICursorProvider` implementation to supply the cursor for the associated control.*

- **SizeChangedTrigger**
  *Triggers actions when the associated control's size changes.*

### Converters
- **PointerEventArgsConverter**  
  *Converts pointer event arguments into a tuple (x, y) representing the pointer’s location.*

### Core (General Infrastructure)
- **ActualThemeVariantChangedBehavior**  
  *A base class for behaviors that react to theme variant changes (e.g. switching from light to dark mode).*

- **ActualThemeVariantChangedTrigger**
  *Triggers actions when the actual theme variant of a control changes.*
- **ThemeVariantBehavior**
  *Applies a specific theme variant to the associated control.*
- **ThemeVariantTrigger**
  *Triggers actions when the theme variant of a control changes.*
- **SetThemeVariantAction**
  *Sets the requested theme variant on a target control.*

- **AttachedToLogicalTreeBehavior**  
  *A base class for behaviors that require notification when the associated object is added to the logical tree.*

- **AttachedToLogicalTreeTrigger**  
  *Triggers actions when an element is attached to the logical tree.*

- **AttachedToVisualTreeBehavior**  
  *A base class for behaviors that depend on the control being attached to the visual tree.*

- **AttachedToVisualTreeTrigger**  
  *Triggers actions when the associated element is added to the visual tree.*

- **BindingBehavior**  
  *Establishes a binding on a target property using an Avalonia binding.*

- **BindingTriggerBehavior**  
  *Monitors a binding’s value and triggers actions when a specified condition is met.*

- **CallMethodAction**  
  *Invokes a method on a target object when the action is executed.*

- **ChangePropertyAction**
  *Changes a property on a target object to a new value using type conversion if needed.*
- **LaunchUriOrFileAction**
  *Opens a URI or file using the default associated application.*

- **DataContextChangedBehavior**  
  *A base class for behaviors that react to changes in the DataContext.*

- **DataContextChangedTrigger**  
  *Triggers actions when the DataContext of a control changes.*

- **DataTriggerBehavior**
  *Evaluates a data binding against a given condition and triggers actions when the condition is true.*
- **DataTrigger**
  *Performs actions when the bound data meets a specified condition.*
- **PropertyChangedTrigger**
  *Triggers actions when a property value changes.*

- **DetachedFromLogicalTreeTrigger**
  *Triggers actions when the control is removed from the logical tree.*

- **DetachedFromVisualTreeTrigger**  
  *Triggers actions when the control is removed from the visual tree.*

- **DisposingBehavior**  
  *A base class for behaviors that manage disposable resources automatically.*

- **DisposingTrigger**
  *A base class for triggers that need to dispose of resources when detached.*
- **DisposableAction**
  *Executes a delegate when the object is disposed.*

- **EventTriggerBehavior**
  *Listens for a specified event on the associated object and triggers actions accordingly.*
- **EventTrigger**
  *Executes its actions when the configured event is raised.*
- **TimerTrigger**
  *Invokes actions repeatedly after a set interval.*

- **InitializedBehavior**  
  *A base class for behaviors that execute code when the associated object is initialized.*

- **InitializedTrigger**  
  *Triggers actions once the control is initialized.*

- **InvokeCommandAction**  
  *Executes a bound ICommand when the action is invoked.*

- **InvokeCommandActionBase**  
  *The base class for actions that invoke commands, with support for parameter conversion.*

- **LoadedBehavior**  
  *A base class for behaviors that run when a control is loaded into the visual tree.*

- **LoadedTrigger**  
  *Triggers actions when the control’s Loaded event fires.*

- **ResourcesChangedBehavior**  
  *A base class for behaviors that respond when a control’s resources change.*

- **ResourcesChangedTrigger**  
  *Triggers actions when the control’s resources are modified.*

- **RoutedEventTriggerBase**  
  *A base class for triggers that listen for a routed event and execute actions.*

- **RoutedEventTriggerBase<T>**
  *Generic version of RoutedEventTriggerBase for strongly typed routed event args.*

- **RoutedEventTriggerBehavior**  
  *Listens for a routed event on the associated object and triggers its actions.*

- **UnloadedTrigger**  
  *Triggers actions when the control is unloaded from the visual tree.*

- **ValueChangedTriggerBehavior**
  *Triggers actions when the value of a bound property changes.*

- **IfElseTrigger**
  *Executes one collection of actions when a condition is true and another when it is false.*

### DragAndDrop
- **ContextDragBehavior**
  *Enables drag operations using a “context” (data payload) that is carried during the drag–drop operation.*

- **ContextDragWithDirectionBehavior**
  *Starts a drag operation and includes the drag direction in the data object.*

- **ContextDropBehavior**  
  *Handles drop events and passes context data between the drag source and drop target.*

- **DropHandlerBase**  
  *Provides common helper methods (move, swap, insert) for implementing custom drop logic.*

- **IDragHandler**  
  *Interface for classes that handle additional logic before and after a drag–drop operation.*

- **IDropHandler**
  *Interface for classes that implement validation and handling of drop operations.*

- **DropBehaviorBase**
  *Base class for behaviors that handle drag-and-drop events and execute commands.*

- **ContextDragBehaviorBase**
  *Base class for context drag behaviors that initiate a drag using context data.*

- **ContextDropBehaviorBase**
  *Base class for context drop behaviors handling dropped context data.*

- **DragAndDropEventsBehavior**
  *Abstract behavior used to attach handlers for drag-and-drop events.*

- **FilesDropBehavior**
  *Executes a command with a collection of dropped files.*
- **ContentControlFilesDropBehavior**
  *Executes a command with dropped files on a ContentControl.*

- **TextDropBehavior**
  *Executes a command with dropped text.*

- **TypedDragBehaviorBase**
  *Base class for drag behaviors working with a specific data type.*

- **TypedDragBehavior**
  *Provides drag behavior for items of a specified data type.*

### Draggable
- **CanvasDragBehavior**  
  *Enables a control to be dragged within a Canvas by updating its RenderTransform based on pointer movements.*

- **GridDragBehavior**  
  *Allows grid cells (or items) to be swapped or repositioned by dragging within a Grid layout.*

- **ItemDragBehavior**
  *Enables reordering of items in an ItemsControl by dragging and dropping items.*
- **MouseDragElementBehavior**
  *Allows an element to be dragged using the mouse.*
- **MultiMouseDragElementBehavior**
  *Supports dragging multiple elements simultaneously with the mouse.*

- **SelectionAdorner**
  *A visual adorner used to indicate selection or to show drag outlines during drag–drop operations.*

### Events
- **InteractiveBehaviorBase**
  *Base class for behaviors that listen to UI events, providing common functionality for event triggers.*
- **InteractionTriggerBehavior**
  *Base behavior for creating custom event-based triggers.*

- **DoubleTappedEventBehavior**  
  *Listens for double-tap events and triggers its actions when detected.*

- **GotFocusEventBehavior**  
  *Executes actions when the associated control receives focus.*

- **KeyDownEventBehavior**  
  *Monitors key down events and triggers actions when the specified key is pressed.*

- **KeyUpEventBehavior**  
  *Monitors key up events and triggers actions when the specified key is released.*

- **LostFocusEventBehavior**  
  *Triggers actions when the control loses focus.*

- **PointerCaptureLostEventBehavior**  
  *Listens for events when pointer capture is lost and triggers associated actions.*

- **PointerEnteredEventBehavior**  
  *Triggers actions when the pointer enters the bounds of a control.*

- **PointerEventsBehavior**  
  *A base class that simplifies handling of pointer events (pressed, moved, released).*

- **PointerExitedEventBehavior**  
  *Triggers actions when the pointer exits a control.*

- **PointerMovedEventBehavior**  
  *Triggers actions when the pointer moves over a control.*

- **PointerPressedEventBehavior**  
  *Triggers actions on pointer press events.*

- **PointerReleasedEventBehavior**  
  *Triggers actions on pointer release events.*

- **PointerWheelChangedEventBehavior**  
  *Triggers actions when the pointer wheel (scroll) changes.*

- **RightTappedEventBehavior**  
  *Triggers actions when the control is right-tapped.*

- **ScrollGestureEndedEventBehavior**  
  *Triggers actions when a scroll gesture ends.*

- **ScrollGestureEventBehavior**  
  *Monitors scroll gestures and triggers actions when they occur.*

- **TappedEventBehavior**  
  *Triggers actions on simple tap events.*

- **TextInputEventBehavior**  
  *Listens for text input events and triggers actions accordingly.*

- **TextInputMethodClientRequestedEventBehavior**
  *Triggers actions when a text input method client is requested (for virtual keyboards, etc.).*

### Event Triggers
- **DoubleTappedEventTrigger**
  *Triggers actions when a double-tap gesture occurs.*
- **GotFocusEventTrigger**
  *Triggers actions when the control receives focus.*
- **KeyDownEventTrigger**
  *Triggers actions when a key is pressed.*
- **KeyUpEventTrigger**
  *Triggers actions when a key is released.*
- **LostFocusEventTrigger**
  *Triggers actions when the control loses focus.*
- **PointerCaptureLostEventTrigger**
  *Triggers actions when pointer capture is lost.*
- **PointerEnteredEventTrigger**
  *Triggers actions when the pointer enters the control.*
- **PointerEventsTrigger**
  *Triggers actions for pointer press, move, and release events.*
- **PointerExitedEventTrigger**
  *Triggers actions when the pointer exits the control.*
- **PointerMovedEventTrigger**
  *Triggers actions when the pointer moves.*
- **PointerPressedEventTrigger**
  *Triggers actions when the pointer is pressed.*
- **PointerReleasedEventTrigger**
  *Triggers actions when the pointer is released.*
- **PointerWheelChangedEventTrigger**
  *Triggers actions when the pointer wheel changes.*
- **RightTappedEventTrigger**
  *Triggers actions on a right-tap gesture.*
- **ScrollGestureEndedEventTrigger**
  *Triggers actions when a scroll gesture ends.*
- **ScrollGestureEventTrigger**
  *Triggers actions during a scroll gesture.*
- **TappedEventTrigger**
  *Triggers actions when the control is tapped.*
- **TextInputEventTrigger**
  *Triggers actions on text input events.*
- **TextInputMethodClientRequestedEventTrigger**
  *Triggers actions when a text input method client is requested.*
- **PopupOpenedTrigger**
  *Triggers actions when a popup is opened.*
- **PopupClosedTrigger**
  *Triggers actions when a popup is closed.*

### ExecuteCommand Core
- **ExecuteCommandBehaviorBase**  
  *Provides the core functionality for executing a command from within a behavior.*

- **ExecuteCommandOnKeyBehaviorBase**  
  *A base class for command behaviors triggered by key events.*

- **ExecuteCommandRoutedEventBehaviorBase**  
  *A base class for command behaviors that respond to routed events.*

### ExecuteCommand
- **ExecuteCommandOnActivatedBehavior**  
  *Executes a command when the main window (or target window) is activated.*

- **ExecuteCommandOnDoubleTappedBehavior**  
  *Executes a command when the associated control is double-tapped.*

- **ExecuteCommandOnGotFocusBehavior**  
  *Executes a command when the control gains focus.*

- **ExecuteCommandOnHoldingBehavior**  
  *Executes a command when a holding (long press) gesture is detected.*

- **ExecuteCommandOnKeyDownBehavior**  
  *Executes a command in response to a key down event matching a specified key or gesture.*

- **ExecuteCommandOnKeyUpBehavior**  
  *Executes a command in response to a key up event matching a specified key or gesture.*

- **ExecuteCommandOnLostFocusBehavior**  
  *Executes a command when the control loses focus.*

- **ExecuteCommandOnPinchBehavior**  
  *Executes a command when a pinch gesture is in progress.*

- **ExecuteCommandOnPinchEndedBehavior**  
  *Executes a command when a pinch gesture ends.*

- **ExecuteCommandOnPointerCaptureLostBehavior**  
  *Executes a command when pointer capture is lost from the control.*

- **ExecuteCommandOnPointerEnteredBehavior**  
  *Executes a command when the pointer enters the control’s area.*

- **ExecuteCommandOnPointerExitedBehavior**  
  *Executes a command when the pointer exits the control’s area.*

- **ExecuteCommandOnPointerMovedBehavior**  
  *Executes a command when the pointer moves over the control.*

- **ExecuteCommandOnPointerPressedBehavior**  
  *Executes a command when the pointer is pressed on the control.*

- **ExecuteCommandOnPointerReleasedBehavior**  
  *Executes a command when the pointer is released over the control.*

- **ExecuteCommandOnPointerTouchPadGestureMagnifyBehavior**  
  *Executes a command during a touchpad magnify gesture.*

- **ExecuteCommandOnPointerTouchPadGestureRotateBehavior**  
  *Executes a command during a touchpad rotation gesture.*

- **ExecuteCommandOnPointerTouchPadGestureSwipeBehavior**  
  *Executes a command during a touchpad swipe gesture.*

- **ExecuteCommandOnPointerWheelChangedBehavior**  
  *Executes a command when the pointer wheel delta changes.*

- **ExecuteCommandOnPullGestureBehavior**  
  *Executes a command when a pull gesture is detected.*

- **ExecuteCommandOnPullGestureEndedBehavior**  
  *Executes a command when a pull gesture ends.*

- **ExecuteCommandOnRightTappedBehavior**  
  *Executes a command when the control is right-tapped.*

- **ExecuteCommandOnScrollGestureBehavior**  
  *Executes a command during a scroll gesture.*

- **ExecuteCommandOnScrollGestureEndedBehavior**  
  *Executes a command when a scroll gesture ends.*

- **ExecuteCommandOnScrollGestureInertiaStartingBehavior**  
  *Executes a command when the inertia phase of a scroll gesture starts.*

- **ExecuteCommandOnTappedBehavior**  
  *Executes a command when a tap event occurs.*

- **ExecuteCommandOnTextInputBehavior**  
  *Executes a command in response to text input events.*

- **ExecuteCommandOnTextInputMethodClientRequestedBehavior**  
  *Executes a command when text input method (virtual keyboard) is requested.*

- **InvokeCommandBehaviorBase**  
  *The base class that supports converting parameters and invoking a bound command.*

### Focus
- **AutoFocusBehavior**
  *Automatically sets the focus on the associated control when it is loaded.*
- **FocusBehavior**
  *Exposes a two‑way bindable IsFocused property to control focus state.*

- **FocusBehaviorBase**  
  *Provides a base implementation for focus behaviors, including support for navigation methods and key modifiers.*

- **FocusControlBehavior**  
  *Forces focus onto a specified control when triggered.*

- **FocusOnAttachedBehavior**  
  *Immediately focuses the control when the behavior is attached.*

- **FocusOnAttachedToVisualTreeBehavior**  
  *Focuses the control as soon as it is attached to the visual tree.*

- **FocusOnPointerMovedBehavior**  
  *Sets focus on the control when pointer movement is detected.*

- **FocusOnPointerPressedBehavior**  
  *Focuses the control when a pointer press event occurs.*

- **FocusSelectedItemBehavior**  
  *Focuses the currently selected item in an ItemsControl.*

### Gestures
- **DoubleTappedGestureTrigger**  
  *Triggers actions when a double-tap gesture is detected.*

- **HoldingGestureTrigger**  
  *Triggers actions when a holding (long press) gesture is detected.*

- **PinchEndedGestureTrigger**  
  *Triggers actions when a pinch gesture has ended.*

- **PinchGestureTrigger**  
  *Triggers actions during a pinch gesture.*

- **PointerTouchPadGestureMagnifyGestureTrigger**  
  *Triggers actions during a touchpad magnification gesture.*

- **PointerTouchPadGestureRotateGestureTrigger**  
  *Triggers actions during a touchpad rotation gesture.*

- **PointerTouchPadGestureSwipeGestureTrigger**  
  *Triggers actions during a touchpad swipe gesture.*

- **PullGestureEndedGestureTrigger**  
  *Triggers actions when a pull gesture ends.*

- **PullGestureGestureTrigger**  
  *Triggers actions during a pull gesture.*

- **RightTappedGestureTrigger**  
  *Triggers actions on a right-tap gesture.*

- **ScrollGestureEndedGestureTrigger**  
  *Triggers actions when a scroll gesture completes.*

- **ScrollGestureGestureTrigger**  
  *Triggers actions during a scroll gesture.*

- **ScrollGestureInertiaStartingGestureTrigger**  
  *Triggers actions when the inertia phase of a scroll gesture begins.*

- **TappedGestureTrigger**  
  *Triggers actions on a simple tap gesture.*

### InputElement Actions
- **CapturePointerAction**  
  *Captures the pointer (mouse, touch) to a target control so that subsequent pointer events are routed there.*

- **ReleasePointerCaptureAction**
  *Releases a previously captured pointer from the control.*

- **SetCursorFromProviderAction**
  *Sets the cursor of a control using a cursor created by an `ICursorProvider`.*
- **SetCursorAction**
  *Sets the cursor of a control to a predefined cursor.*
- **SetEnabledAction**
  *Enables or disables the associated control.*
- **HideToolTipAction**
  *Hides the ToolTip of the target control.*
- **SetToolTipTipAction**
  *Sets the ToolTip's tip text on the associated or target control.*
- **ShowToolTipAction**
  *Shows the ToolTip for the associated or target control.*

### InputElement Triggers
- **DoubleTappedTrigger**  
  *Listens for a double-tap event and executes its actions.*

- **GotFocusTrigger**  
  *Triggers actions when the control receives focus.*

- **HoldingTrigger**  
  *Triggers actions when a holding gesture is detected.*

- **KeyDownTrigger**
  *Listens for key down events and triggers actions if the pressed key (or gesture) matches the specified criteria.*
- **KeyGestureTrigger**
  *Triggers actions based on a specified key gesture.*

- **KeyTrigger**
  *Listens for key down or key up events and triggers actions when the configured key or gesture occurs.*

- **KeyUpTrigger**
  *Listens for key up events and triggers actions when conditions are met.*

- **LostFocusTrigger**  
  *Triggers actions when the control loses focus.*

- **PointerCaptureLostTrigger**  
  *Triggers actions when pointer capture is lost by the control.*

- **PointerEnteredTrigger**  
  *Triggers actions when the pointer enters the control’s area.*

- **PointerExitedTrigger**  
  *Triggers actions when the pointer exits the control’s area.*

- **PointerMovedTrigger**  
  *Triggers actions on pointer movement over the control.*

- **PointerPressedTrigger**  
  *Triggers actions when the pointer is pressed on the control.*

- **PointerReleasedTrigger**  
  *Triggers actions when the pointer is released on the control.*

- **PointerWheelChangedTrigger**  
  *Triggers actions on mouse wheel (or equivalent) changes.*

- **TappedTrigger**
  *Triggers actions on a tap event.*
- **ToolTipOpeningTrigger**
  *Triggers actions when a tooltip is about to open.*
- **ToolTipClosingTrigger**
  *Triggers actions when a tooltip is closing.*

- **TextInputMethodClientRequestedTrigger**
  *Triggers actions when a text input method (virtual keyboard) is requested.*

- **TextInputTrigger**  
  *Triggers actions on text input events.*

### WriteableBitmap
- **IWriteableBitmapRenderer**
  *Defines a method used to render into a WriteableBitmap so view models can supply drawing logic.*
- **WriteableBitmapRenderBehavior**
  *Creates a writeable bitmap and updates it using a renderer on a timer.*
- **WriteableBitmapRenderAction**
  *Invokes a renderer to update a writeable bitmap.*
- **WriteableBitmapTimerTrigger**
  *Fires its actions on a timer and passes the writeable bitmap as a parameter.*
- **WriteableBitmapBehavior**
  *Creates a writeable bitmap and renders it once or on demand without animation.*
- **WriteableBitmapTrigger**
  *Manually executes its actions with the provided writeable bitmap when triggered.*

### RenderTargetBitmap
- **IRenderTargetBitmapRenderer**
  *Defines a method used to render into a RenderTargetBitmap.*
- **IRenderTargetBitmapSimpleRenderer**
  *Provides a simple rendering method for StaticRenderTargetBitmapBehavior.*
- **RenderRenderTargetBitmapAction**
  *Invokes IRenderTargetBitmapRenderHost.Render on the specified target.*
- **RenderTargetBitmapBehavior**
  *Creates and updates a RenderTargetBitmap via a renderer.*
- **StaticRenderTargetBitmapBehavior**
  *Draws once into a RenderTargetBitmap and assigns it to the associated Image.*
- **RenderTargetBitmapTrigger**
  *Triggers actions when RenderTargetBitmap rendering completes.*
### ItemsControl
- **ItemNudgeDropBehavior**  
  *Provides “nudge” effects for items in an ItemsControl during drag–drop reordering.*

- **ItemsControlContainerClearingTrigger**  
  *Triggers actions when the ItemsControl clears its container(s).*

- **ItemsControlContainerEventsBehavior**  
  *A base behavior that listens for container events (prepared, index changed, clearing) on an ItemsControl.*

- **ItemsControlContainerIndexChangedTrigger**  
  *Triggers actions when the index of an item’s container changes.*

- **ItemsControlContainerPreparedTrigger**
  *Triggers actions when a container for an item is prepared.*

- **ItemsControlPreparingContainerTrigger**
  *Executes actions when the ItemsControl raises the PreparingContainer event.*

- **ScrollToItemBehavior**
  *Automatically scrolls the ItemsControl to make a specified item visible.*

- **ScrollToItemIndexBehavior**
  *Scrolls to a specific item index in the ItemsControl.*

- **AddItemToItemsControlAction**
  *Adds an item to an ItemsControl's collection.*

- **InsertItemToItemsControlAction**
  *Inserts an item at a specific index in an ItemsControl.*

- **ClearItemsControlAction**
  *Clears all items from an ItemsControl.*

- **RemoveItemInItemsControlAction**
  *Removes the specified item from an ItemsControl.*

### ListBox
- **ListBoxSelectAllBehavior**
  *Selects all items in a ListBox when the behavior is attached.*
- **RemoveItemInListBoxAction**
  *Removes the specified item from a ListBox.*

- **ListBoxUnselectAllBehavior**
  *Clears the selection in a ListBox.*

### ListBoxItem
- **SelectListBoxItemOnPointerMovedBehavior**
  *Automatically selects a ListBoxItem when the pointer moves over it.*
- **InlineEditBehavior**
  *Toggles between a display element and a TextBox editor to enable inline editing (activated by double-tap or F2; ends on Enter, Escape, or losing focus).* 

### Responsive
- **AdaptiveBehavior**
  *Observes bounds changes of a control (or a specified source) and conditionally adds or removes CSS-style classes based on adaptive rules.*

- **AdaptiveClassSetter**
  *Specifies comparison conditions (min/max width/height) and the class to apply when those conditions are met.*

- **AspectRatioBehavior**
  *Observes bounds changes and toggles CSS-style classes when the control's aspect ratio matches specified rules.*

- **AspectRatioClassSetter**
  *Defines aspect ratio comparison conditions and the class to apply when those conditions are met.*

### ScrollViewer
- **HorizontalScrollViewerBehavior**
  *Enables horizontal scrolling via the pointer wheel. Optionally requires the Shift key and supports line or page scrolling.*
- **ViewportBehavior**
  *Tracks when the associated element enters or exits a ScrollViewer's viewport.*

### Screen
- **ActiveScreenBehavior**
  *Provides the currently active screen for a window.*
- **RequestScreenDetailsAction**
  *Requests extended screen information using a Screens instance.*
- **ScreensChangedTrigger**
  *Triggers actions when the available screens change.*

### SelectingItemsControl
- **SelectingItemsControlEventsBehavior**
  *Handles selection-changed events in controls that support item selection (like ListBox) to trigger custom actions.*
- **SelectingItemsControlSearchBehavior**
  *Enables searching and highlights matching items within a SelectingItemsControl.*

### Show
- **ShowBehaviorBase**  
  *A base class for behaviors that “show” (make visible) a target control when a trigger condition is met.*

- **ShowOnDoubleTappedBehavior**  
  *Shows a control when a double-tap gesture is detected.*

- **ShowOnKeyDownBehavior**  
  *Shows a control when a specified key (or key gesture) is pressed.*

- **ShowOnTappedBehavior**  
  *Shows the target control when it is tapped.*

### StorageProvider – Button
- **ButtonOpenFilePickerBehavior**  
  *Attaches to a Button to open a file picker dialog when clicked.*

- **ButtonOpenFolderPickerBehavior**  
  *Attaches to a Button to open a folder picker dialog when clicked.*

- **ButtonSaveFilePickerBehavior**  
  *Attaches to a Button to open a save file picker dialog when clicked.*

### StorageProvider – Converters
- **StorageFileToReadStreamConverter**  
  *Converts an IStorageFile into a read stream (asynchronously).*

- **StorageFileToWriteStreamConverter**  
  *Converts an IStorageFile into a write stream (asynchronously).*

- **StorageItemToPathConverter**  
  *Extracts the file system path from an IStorageItem.*

### StorageProvider – Core
- **PickerActionBase**  
  *Base class for actions that invoke file/folder picker dialogs.*

- **PickerBehaviorBase**  
  *Base class for behaviors that wrap file/folder picker functionality.*

### StorageProvider – MenuItem
- **MenuItemOpenFilePickerBehavior**  
  *Opens a file picker dialog when a MenuItem is clicked.*

- **MenuItemSaveFilePickerBehavior**  
  *Opens a save file picker dialog when a MenuItem is clicked.*

- **MenuItemOpenFolderPickerBehavior**  
  *Opens a folder picker dialog when a MenuItem is clicked.*

### StorageProvider
- **OpenFilePickerAction**  
  *Opens a file picker dialog and passes the selected file(s) as a command parameter.*

- **OpenFilePickerBehaviorBase**  
  *Base behavior for opening file picker dialogs.*

- **OpenFolderPickerAction**  
  *Opens a folder picker dialog and passes the selected folder(s) as a command parameter.*

- **OpenFolderPickerBehaviorBase**  
  *Base behavior for opening folder picker dialogs.*

- **SaveFilePickerAction**  
  *Opens a save file picker dialog and passes the chosen file as a command parameter.*

- **SaveFilePickerBehaviorBase**
  *Base behavior for saving files using a file picker dialog.*

### Scripting
- **ExecuteScriptAction**
  *Executes a C# script using the Roslyn scripting API.*

### ReactiveUI
- **ClearNavigationStackAction**
  *Resets the ReactiveUI navigation stack.*

- **NavigateAction**
  *Navigates to a specified `IRoutableViewModel` using a router.*

- **NavigateToAction**
  *Resolves and navigates to a view model type using a router.*

- **NavigateBackAction**
  *Navigates back within a `RoutingState` stack.*

- **NavigateAndReset**
  *Navigates to a view model and clears the navigation stack.*
- **NavigateToAndResetAction**
  *Resolves a view model type, clears the navigation stack, and navigates to it.*
- **ObservableTriggerBehavior**
  *Subscribes to an `IObservable` and executes actions whenever the observable emits a value.*

### TextBox
- **AutoSelectBehavior**
  *Selects all text in a TextBox when it is loaded.*
- **TextBoxSelectAllOnGotFocusBehavior**
  *Selects all text in a TextBox when it gains focus.*

- **TextBoxSelectAllTextBehavior**
  *Selects all text in a TextBox immediately upon attachment.*

### TreeViewItem
- **ToggleIsExpandedOnDoubleTappedBehavior**
  *Toggles the IsExpanded property of a TreeViewItem when it is double-tapped.*

### SplitView
- **SplitViewStateBehavior**
  *Automatically updates `DisplayMode`, `PanePlacement`, and `IsPaneOpen` based on size conditions.*
- **SplitViewStateSetter**
  *Specifies size conditions and target values used by SplitViewStateBehavior.*
- **SplitViewPaneOpeningTrigger**
  *Triggers actions when the pane is about to open.*
- **SplitViewPaneOpenedTrigger**
  *Triggers actions after the pane has opened.*
- **SplitViewPaneClosingTrigger**
  *Triggers actions when the pane is about to close.*
- **SplitViewPaneClosedTrigger**
  *Triggers actions after the pane has closed.*

### Window
- **DialogOpenedTrigger**
  *Triggers actions when a window is opened.*
- **DialogClosedTrigger**
  *Triggers actions when a window is closed.*
- **ShowDialogAction**
  *Shows a window as a dialog.*
- **CloseWindowAction**
  *Closes a window when executed.*

## Interactivity (Infrastructure)

### AvaloniaObject
- **Action**  
  *The base class for actions that can be executed by triggers.*

- **Behavior**  
  *The base class for behaviors that attach to Avalonia objects.*

- **Behavior<T>**
  *Generic base class for behaviors that require a specific type of associated object.*

- **Trigger**  
  *A base class for triggers that execute a collection of actions when activated.*

- **Trigger<T>**
  *Generic version of Trigger for strongly typed associated objects.*

### Collections
- **ActionCollection**  
  *A collection of actions that can be executed by a trigger.*

- **BehaviorCollection**  
  *A collection of behaviors attached to a single Avalonia object.*

### Contract
- **ComparisonConditionType**  
  *Defines the types of comparisons (equal, not equal, less than, etc.) used in data and adaptive triggers.*

- **IAction**  
  *Interface that defines the Execute method for custom actions.*

- **IBehavior**  
  *Interface for behaviors that can attach and detach from an Avalonia object.*

- **IBehaviorEventsHandler**  
  *Interface for handling events (loaded, attached, etc.) within behaviors.*

- **ITrigger**  
  *Interface for triggers that encapsulate a collection of actions.*

### StyledElement
- **StyledElementAction**  
  *A base class for actions that work with StyledElement objects.*

- **StyledElementBehavior**  
  *A base class for behaviors targeting StyledElement objects.*

- **StyledElementBehavior<T>**
  *Generic base class for behaviors that are attached to a specific type of StyledElement.*

- **StyledElementTrigger**  
  *A base trigger class for StyledElement objects.*

- **StyledElementTrigger<T>**
  *Generic version of the StyledElementTrigger for typed associated objects.*

### Templates
- **BehaviorCollectionTemplate**
  *Defines a XAML template for creating a collection of behaviors.*
- **ObjectTemplate**
  *A template for creating custom objects.*

### Interactivity
- **Interaction**  
  *A static helper class for managing behavior collections and executing actions associated with triggers.*

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Xaml.Behaviors)

## License

XAML Behaviors is licensed under the [MIT license](LICENSE.TXT).
