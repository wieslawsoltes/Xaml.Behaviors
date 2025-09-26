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
- **AddClassAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRemoveClassActionView.axaml))
  *Adds a style class to a control’s class collection, making it easy to change the appearance dynamically.*

- **ChangeAvaloniaPropertyAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ChangeAvaloniaPropertyActionView.axaml))
  *Changes the value of an Avalonia property on a target object at runtime.*

- **CloseNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowNotificationActionView.axaml))
  *Closes an open notification, for example, from a notification control.*

- **FocusControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusControlActionView.axaml))
  *Sets the keyboard focus on a specified control or the associated control.*

- **HideControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideShowControlActionView.axaml))
  *Hides a control by setting its `IsVisible` property to false.*

- **ShowControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideShowControlActionView.axaml))
  *Shows a control and gives it focus when executed.*
- **DelayedShowControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedShowControlActionView.axaml))
  *Shows a control after waiting for a specified delay.*

- **PopupAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHidePopupActionView.axaml))
  *Displays a popup window for showing additional UI content.*
- **ShowPopupAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Opens an existing popup for a control.*
- **HidePopupAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHidePopupActionView.axaml))
  *Closes an existing popup associated with a control.*

- **ShowFlyoutAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHideFlyoutActionView.axaml))
  *Shows a flyout attached to a control or specified explicitly.*

- **HideFlyoutAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHideFlyoutActionView.axaml))
  *Hides a flyout attached to a control or specified explicitly.*

- **RemoveClassAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRemoveClassActionView.axaml))
  *Removes a style class from a control’s class collection.*

- **RemoveElementAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveElementActionView.axaml))
  *Removes a control from its parent container when executed.*
- **MoveElementToPanelAction** (No sample available.)
  *Moves a control to the specified panel.*

- **ShowContextMenuAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowContextMenuActionView.axaml))
  *Displays a control's context menu programmatically.*

- **SplitViewTogglePaneAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Toggles the `IsPaneOpen` state of a `SplitView`.*
- **SetViewModelPropertyAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Sets a property on the DataContext to a specified value.*
- **IncrementViewModelPropertyAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Adds a numeric delta to a view model property.*
- **ToggleViewModelBooleanAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Toggles a boolean view model property.*

### Animations
- **FadeInBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FadeInBehaviorView.axaml))
  *Animates the fade-in effect for the associated element, gradually increasing its opacity.*

 - **StartAnimationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StartAnimationActionView.axaml))
  *Triggers a defined animation on the target control when executed.*
- **AnimateOnAttachedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimateOnAttachedBehaviorView.axaml))
  *Runs a specified animation when the control appears in the visual tree.*
- **StartBuiltAnimationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StartBuiltAnimationActionView.axaml))
  *Creates an animation in code using an <code>IAnimationBuilder</code> and starts it.*
- **RunAnimationTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RunAnimationTriggerView.axaml))
  *Runs an animation and invokes actions when it completes.*
 - **IAnimationBuilder** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBuilderView.axaml))
  *Interface for providing animations from view models in an MVVM friendly way.*

- **PlayAnimationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBehaviorView.axaml))
  *Runs a supplied animation automatically when the control appears in the visual tree.*

- **BeginAnimationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBehaviorView.axaml))
  *Starts an animation on a specified control, allowing the target to be chosen explicitly.*

- **AnimationCompletedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationCompletedTriggerView.axaml))
  *Plays an animation and invokes actions once the animation finishes running.*

### Transitions
- **TransitionsBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsBehaviorView.axaml))
  *Sets the `Transitions` collection on the associated control when attached.*
- **AddTransitionAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Adds a transition to a control's `Transitions` collection.*
- **RemoveTransitionAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Removes a transition from a control's `Transitions` collection.*
- **ClearTransitionsAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Clears all transitions from a control.*
- **TransitionsChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsChangedTriggerView.axaml))
  *Triggers actions when the `Transitions` property of a control changes.*

### AutoCompleteBox
 - **FocusAutoCompleteBoxTextBoxBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusAutoCompleteBoxTextBoxBehaviorView.axaml))
  *Ensures the text box within an AutoCompleteBox gets focus automatically.*
- **AutoCompleteBoxOpenDropDownOnFocusBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Opens the AutoCompleteBox drop-down when the control receives focus.*
- **AutoCompleteBoxSelectionChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Triggers actions when the AutoCompleteBox selection changes.*
- **ClearAutoCompleteBoxSelectionAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Clears the AutoCompleteBox selection and text.*

### Automation
- **AutomationNameBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Applies an automation name to the associated control.*
- **AutomationNameChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Triggers actions when the automation name of the control changes.*
- **SetAutomationIdAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Sets the automation ID on a target control.*

### Carousel
- **CarouselKeyNavigationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Enables keyboard navigation for a Carousel using arrow keys.*
- **CarouselNextAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Advances the target Carousel to the next page.*
- **CarouselPreviousAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Moves the target Carousel to the previous page.*
 - **CarouselSelectionChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselSelectionChangedTriggerView.axaml))
  *Triggers actions when the Carousel selection changes.*

### Collections
- **AddRangeAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRangeActionView.axaml))
  *Adds multiple items to an `IList`.*
 - **RemoveRangeAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveRangeActionView.axaml))
  *Removes multiple items from an `IList`.*
- **ClearCollectionAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRangeActionView.axaml))
  *Clears all items from an `IList`.*
- **CollectionChangedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CollectionChangedTriggerBehaviorView.axaml))
  *Invokes actions based on collection change notifications.*
- **CollectionChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CollectionChangedTriggerBehaviorView.axaml))
  *Triggers actions whenever the observed collection changes.*

### TabControl
- **TabControlKeyNavigationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Enables keyboard navigation for a TabControl using arrow keys.*
- **TabControlNextAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Advances the target TabControl to the next tab.*
- **TabControlPreviousAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Moves the target TabControl to the previous tab.*
- **TabControlSelectionChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlSelectionChangedTriggerView.axaml))
  *Triggers actions when the TabControl selection changes.*

### Button
- **ButtonClickEventTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonClickEventTriggerBehaviorView.axaml))
  *Listens for a button’s click event and triggers associated actions.*

 - **ButtonExecuteCommandOnKeyDownBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonExecuteCommandOnKeyDownBehaviorView.axaml))
  *Executes a command when a specified key is pressed while the button is focused.*

 - **ButtonHideFlyoutBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonHideFlyoutBehaviorView.axaml))
  *Hides an attached flyout when the button is interacted with.*

 - **ButtonHideFlyoutOnClickBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonHideFlyoutOnClickBehaviorView.axaml))
  *Automatically hides the flyout attached to the button when it is clicked.*
- **ButtonHidePopupOnClickBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Automatically closes the popup containing the button when it is clicked.*

### Clipboard
- **ClearClipboardAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Clears all contents from the system clipboard.*

- **GetClipboardDataAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GetClipboardDataActionView.axaml))
  *Retrieves data from the clipboard in a specified format.*

- **GetClipboardFormatsAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GetClipboardFormatsActionView.axaml))
  *Retrieves the list of available formats from the clipboard.*

- **GetClipboardTextAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Retrieves plain text from the clipboard.*

- **SetClipboardDataObjectAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SetClipboardDataObjectActionView.axaml))
  *Places a custom data object onto the clipboard.*

- **SetClipboardTextAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Places text onto the clipboard.*

### Notifications
- **NotificationManagerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Creates a notification manager for a window when attached.*
- **ShowNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowNotificationActionView.axaml))
  *Shows a notification using an attached notification manager.*
- **ShowInformationNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays an information notification via a manager.*
- **ShowSuccessNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays a success notification via a manager.*
- **ShowWarningNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays a warning notification via a manager.*
- **ShowErrorNotificationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays an error notification via a manager.*

### Composition
 - **SelectingItemsControlBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SelectingItemsControlBehaviorView.axaml))
  *Animates selection transitions in items controls such as ListBox or TabControl.*

- **SlidingAnimation** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SlidingAnimationView.axaml))
  *Provides static methods to apply sliding animations (from left, right, top, or bottom) to a control.*
- **FluidMoveBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FluidMoveBehaviorView.axaml))
  *Animates layout changes of a control or its children.*

### ContextDialogs
- **ContextDialogBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Manages a popup-based context dialog for a control.*
- **ShowContextDialogAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Opens a context dialog using a specified behavior.*
- **HideContextDialogAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Closes a context dialog using a specified behavior.*
- **ContextDialogOpenedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Triggers actions when a context dialog is opened.*
- **ContextDialogClosedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Triggers actions when a context dialog is closed.*

### Control
- **BindPointerOverBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindPointerOverBehaviorView.axaml))
  *Two‑way binds a boolean property to a control’s pointer-over state.*

- **BindTagToVisualRootDataContextBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindTagToVisualRootDataContextBehaviorView.axaml))
  *Binds the control’s Tag property to the DataContext of its visual root, enabling inherited data contexts.*

- **BoundsObserverBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BoundsObserverBehaviorView.axaml))
  *Observes a control’s bounds changes and updates two‑way bound Width and Height properties.*

- **DragControlBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CustomBehaviorView.axaml))
  *Enables a control to be moved (dragged) around by changing its RenderTransform during pointer events.*

- **HideAttachedFlyoutBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideAttachedFlyoutBehaviorView.axaml))
  *Hides a flyout that is attached to a control when a condition is met.*

- **HideOnKeyPressedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnKeyPressedBehaviorView.axaml))
  *Hides the target control when a specified key is pressed.*

- **HideOnLostFocusBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnLostFocusBehaviorView.axaml))
  *Hides the target control when it loses focus.*

- **InlineEditBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Toggles display and edit controls to enable in-place text editing.*

- **ShowPointerPositionBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CustomActionView.axaml))
  *Displays the current pointer position (x, y coordinates) in a TextBlock for debugging or UI feedback.*

- **SetCursorBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CursorView.axaml))
  *Applies a custom cursor to the associated control.*

- **PointerOverCursorBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CursorView.axaml))
  *Changes the cursor while the pointer is over the control and resets it on exit.*

- **SetCursorFromProviderBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Uses an `ICursorProvider` implementation to supply the cursor for the associated control.*

- **SizeChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SizeChangedTriggerView.axaml))
  *Triggers actions when the associated control's size changes.*

### Converters
- **PointerEventArgsConverter** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Converts pointer event arguments into a tuple (x, y) representing the pointer’s location.*

### Core (General Infrastructure)
- **ActualThemeVariantChangedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ActualThemeVariantChangedBehaviorView.axaml))
  *A base class for behaviors that react to theme variant changes (e.g. switching from light to dark mode).*

- **ActualThemeVariantChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ActualThemeVariantChangedTriggerView.axaml))
  *Triggers actions when the actual theme variant of a control changes.*
- **ThemeVariantBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantView.axaml))
  *Applies a specific theme variant to the associated control.*
- **ThemeVariantTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantTriggerView.axaml))
  *Triggers actions when the theme variant of a control changes.*
- **SetThemeVariantAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantView.axaml))
  *Sets the requested theme variant on a target control.*

- **AttachedToLogicalTreeBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToLogicalTreeBehaviorView.axaml))
  *A base class for behaviors that require notification when the associated object is added to the logical tree.*

- **AttachedToLogicalTreeTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToLogicalTreeTriggerView.axaml))
  *Triggers actions when an element is attached to the logical tree.*

- **AttachedToVisualTreeBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedToVisualTreeBehaviorView.axaml))
  *A base class for behaviors that depend on the control being attached to the visual tree.*

- **AttachedToVisualTreeTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToVisualTreeTriggerView.axaml))
  *Triggers actions when the associated element is added to the visual tree.*

- **BindingBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindingBehaviorView.axaml))
  *Establishes a binding on a target property using an Avalonia binding.*

- **BindingTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindingTriggerBehaviorView.axaml))
  *Monitors a binding’s value and triggers actions when a specified condition is met.*

- **CallMethodAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CallMethodActionView.axaml))
  *Invokes a method on a target object when the action is executed.*

- **ChangePropertyAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ChangePropertyActionView.axaml))
  *Changes a property on a target object to a new value using type conversion if needed.*
- **LaunchUriOrFileAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/LaunchUriOrFileActionView.axaml))
  *Opens a URI or file using the default associated application.*

- **DataContextChangedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataContextChangedBehaviorView.axaml))
  *A base class for behaviors that react to changes in the DataContext.*

- **DataContextChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataContextChangedTriggerView.axaml))
  *Triggers actions when the DataContext of a control changes.*

- **DataTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataTriggerBehaviorView.axaml))
  *Evaluates a data binding against a given condition and triggers actions when the condition is true.*
- **DataTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataTriggerBehaviorView.axaml))
  *Performs actions when the bound data meets a specified condition.*
- **PropertyChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PropertyChangedTriggerView.axaml))
  *Triggers actions when a property value changes.*
- **ViewModelPropertyChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Invokes actions when a DataContext property changes.*

- **DetachedFromLogicalTreeTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DetachedFromLogicalTreeTriggerView.axaml))
  *Triggers actions when the control is removed from the logical tree.*

- **DetachedFromVisualTreeTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DetachedFromVisualTreeTriggerView.axaml))
  *Triggers actions when the control is removed from the visual tree.*

- **DisposingBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposingBehaviorView.axaml))
  *A base class for behaviors that manage disposable resources automatically.*

- **DisposingTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposingTriggerView.axaml))
  *A base class for triggers that need to dispose of resources when detached.*
- **DisposableAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposableActionView.axaml))
  *Executes a delegate when the object is disposed.*

- **EventTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EventTriggerBehaviorView.axaml))
  *Listens for a specified event on the associated object and triggers actions accordingly.*
- **EventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBehaviorView.axaml))
  *Executes its actions when the configured event is raised.*
- **TimerTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TimerTriggerView.axaml))
  *Invokes actions repeatedly after a set interval.*

- **InitializedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InitializedBehaviorView.axaml))
  *A base class for behaviors that execute code when the associated object is initialized.*

- **InitializedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InitializedTriggerView.axaml))
  *Triggers actions once the control is initialized.*

- **InvokeCommandAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InvokeCommandActionView.axaml))
  *Executes a bound ICommand when the action is invoked.*

- **InvokeCommandActionBase** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InvokeCommandActionBaseView.axaml))
  *The base class for actions that invoke commands, with support for parameter conversion.*

- **LoadedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/LoadedBehaviorView.axaml))
  *A base class for behaviors that run when a control is loaded into the visual tree.*
- **SetViewModelPropertyOnLoadBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Sets a view model property when the associated control is loaded.*

- **LoadedTrigger** (No sample available.)
  *Triggers actions when the control’s Loaded event fires.*
- **DelayedLoadBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedLoadBehaviorView.axaml))
  *Hides the control then shows it after a specified delay when attached to the visual tree.*
- **DelayedLoadTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedLoadTriggerView.axaml))
  *Invokes actions after the control is loaded and a delay period expires.*

- **ResourcesChangedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ResourcesChangedBehaviorView.axaml))
  *A base class for behaviors that respond when a control’s resources change.*

- **ResourcesChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ResourcesChangedTriggerView.axaml))
  *Triggers actions when the control’s resources are modified.*

- **RoutedEventTriggerBase** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBaseView.axaml))
  *A base class for triggers that listen for a routed event and execute actions.*

- **RoutedEventTriggerBase<T>** (No sample available.)
  *Generic version of RoutedEventTriggerBase for strongly typed routed event args.*

- **RoutedEventTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBehaviorView.axaml))
  *Listens for a routed event on the associated object and triggers its actions.*

- **UnloadedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UnloadedTriggerView.axaml))
  *Triggers actions when the control is unloaded from the visual tree.*

- **ValueChangedTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ValueChangedTriggerBehaviorView.axaml))
  *Triggers actions when the value of a bound property changes.*

- **IfElseTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IfElseTriggerView.axaml))
  *Executes one collection of actions when a condition is true and another when it is false.*

### DragAndDrop
- **ContextDragBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Enables drag operations using a “context” (data payload) that is carried during the drag–drop operation.*

- **ContextDragWithDirectionBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDragTreeViewView.axaml))
  *Starts a drag operation and includes the drag direction in the data object.*

- **ContextDropBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TypedDragBehaviorView.axaml))
  *Handles drop events and passes context data between the drag source and drop target.*

- **DropHandlerBase** (No sample available.)
  *Provides common helper methods (move, swap, insert) for implementing custom drop logic.*

- **IDragHandler** (No sample available.)
  *Interface for classes that handle additional logic before and after a drag–drop operation.*

- **IDropHandler** (No sample available.)
  *Interface for classes that implement validation and handling of drop operations.*

- **DropBehaviorBase** (No sample available.)
  *Base class for behaviors that handle drag-and-drop events and execute commands.*

- **ContextDragBehaviorBase** (No sample available.)
  *Base class for context drag behaviors that initiate a drag using context data.*

- **ContextDropBehaviorBase** (No sample available.)
  *Base class for context drop behaviors handling dropped context data.*

- **DragAndDropEventsBehavior** (No sample available.)
  *Abstract behavior used to attach handlers for drag-and-drop events.*

- **FilesDropBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContentControlFilesDropBehaviorView.axaml))
  *Executes a command with a collection of dropped files.*
- **ContentControlFilesDropBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContentControlFilesDropBehaviorView.axaml))
  *Executes a command with dropped files on a ContentControl.*

- **TextDropBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FileDropHandlerView.axaml))
  *Executes a command with dropped text.*

- **TypedDragBehaviorBase** (No sample available.)
  *Base class for drag behaviors working with a specific data type.*

- **TypedDragBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TypedDragBehaviorView.axaml))
  *Provides drag behavior for items of a specified data type.*
- **PanelDragBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Starts drag operations using the dragged control as context so it can be moved between panels.*
- **PanelDropBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Accepts dragged controls and inserts them into the target panel.*

### Draggable
- **CanvasDragBehavior** (No sample available.)
  *Enables a control to be dragged within a Canvas by updating its RenderTransform based on pointer movements.*

- **GridDragBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DraggableView.axaml))
  *Allows grid cells (or items) to be swapped or repositioned by dragging within a Grid layout.*

- **ItemDragBehavior** (No sample available.)
  *Enables reordering of items in an ItemsControl by dragging and dropping items.*
- **MouseDragElementBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/MouseDragBehaviorView.axaml))
  *Allows an element to be dragged using the mouse.*
- **MultiMouseDragElementBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/MouseDragBehaviorView.axaml))
  *Supports dragging multiple elements simultaneously with the mouse.*

- **SelectionAdorner** (No sample available.)
  *A visual adorner used to indicate selection or to show drag outlines during drag–drop operations.*

### Events
- **InteractiveBehaviorBase** (No sample available.)
  *Base class for behaviors that listen to UI events, providing common functionality for event triggers.*
- **InteractionTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/InteractionTriggerBehaviorView.axaml))
  *Base behavior for creating custom event-based triggers.*

- **DoubleTappedEventBehavior** (No sample available.)
  *Listens for double-tap events and triggers its actions when detected.*

- **GotFocusEventBehavior** (No sample available.)
  *Executes actions when the associated control receives focus.*

- **KeyDownEventBehavior** (No sample available.)
  *Monitors key down events and triggers actions when the specified key is pressed.*

- **KeyUpEventBehavior** (No sample available.)
  *Monitors key up events and triggers actions when the specified key is released.*

- **LostFocusEventBehavior** (No sample available.)
  *Triggers actions when the control loses focus.*

- **PointerCaptureLostEventBehavior** (No sample available.)
  *Listens for events when pointer capture is lost and triggers associated actions.*

- **PointerEnteredEventBehavior** (No sample available.)
  *Triggers actions when the pointer enters the bounds of a control.*

- **PointerEventsBehavior** (No sample available.)
  *A base class that simplifies handling of pointer events (pressed, moved, released).*

- **PointerExitedEventBehavior** (No sample available.)
  *Triggers actions when the pointer exits a control.*

- **PointerMovedEventBehavior** (No sample available.)
  *Triggers actions when the pointer moves over a control.*

- **PointerPressedEventBehavior** (No sample available.)
  *Triggers actions on pointer press events.*

- **PointerReleasedEventBehavior** (No sample available.)
  *Triggers actions on pointer release events.*

- **PointerWheelChangedEventBehavior** (No sample available.)
  *Triggers actions when the pointer wheel (scroll) changes.*

- **RightTappedEventBehavior** (No sample available.)
  *Triggers actions when the control is right-tapped.*

- **ScrollGestureEndedEventBehavior** (No sample available.)
  *Triggers actions when a scroll gesture ends.*

- **ScrollGestureEventBehavior** (No sample available.)
  *Monitors scroll gestures and triggers actions when they occur.*

- **TappedEventBehavior** (No sample available.)
  *Triggers actions on simple tap events.*

- **TextInputEventBehavior** (No sample available.)
  *Listens for text input events and triggers actions accordingly.*

- **TextInputMethodClientRequestedEventBehavior** (No sample available.)
  *Triggers actions when a text input method client is requested (for virtual keyboards, etc.).*

### Event Triggers
- **DoubleTappedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when a double-tap gesture occurs.*
- **GotFocusEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusEventTriggersView.axaml))
  *Triggers actions when the control receives focus.*
- **KeyDownEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a key is pressed.*
- **KeyUpEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a key is released.*
- **LostFocusEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusEventTriggersView.axaml))
  *Triggers actions when the control loses focus.*
- **PointerCaptureLostEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when pointer capture is lost.*
- **PointerEnteredEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer enters the control.*
- **PointerEventsTrigger** (No sample available.)
  *Triggers actions for pointer press, move, and release events.*
- **PointerExitedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer exits the control.*
- **PointerMovedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer moves.*
- **PointerPressedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EventsBehaviorsView.axaml))
  *Triggers actions when the pointer is pressed.*
- **PointerReleasedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer is released.*
- **PointerWheelChangedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer wheel changes.*
- **RightTappedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions on a right-tap gesture.*
- **ScrollGestureEndedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when a scroll gesture ends.*
- **ScrollGestureEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions during a scroll gesture.*
- **TappedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when the control is tapped.*
- **TextInputEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions on text input events.*
- **TextInputMethodClientRequestedEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a text input method client is requested.*
- **PopupOpenedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Triggers actions when a popup is opened.*
- **PopupClosedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Triggers actions when a popup is closed.*
- **DragEnterEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Triggers actions when a drag operation enters the element.*
- **DragLeaveEventTrigger** (No sample available.)
  *Triggers actions when a drag operation leaves the element.*
- **DragOverEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FilesPreviewView.axaml))
  *Triggers actions while a drag is over the element.*
- **DropEventTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Triggers actions when an item is dropped on the element.*

### ExecuteCommand Core
- **ExecuteCommandBehaviorBase** (No sample available.)
  *Provides the core functionality for executing a command from within a behavior.*

- **ExecuteCommandOnKeyBehaviorBase** (No sample available.)
  *A base class for command behaviors triggered by key events.*

- **ExecuteCommandRoutedEventBehaviorBase** (No sample available.)
  *A base class for command behaviors that respond to routed events.*

### ExecuteCommand
- **ExecuteCommandOnActivatedBehavior** (No sample available.)
  *Executes a command when the main window (or target window) is activated.*

- **ExecuteCommandOnDoubleTappedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the associated control is double-tapped.*

- **ExecuteCommandOnGotFocusBehavior** (No sample available.)
  *Executes a command when the control gains focus.*

- **ExecuteCommandOnHoldingBehavior** (No sample available.)
  *Executes a command when a holding (long press) gesture is detected.*

- **ExecuteCommandOnKeyDownBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command in response to a key down event matching a specified key or gesture.*

- **ExecuteCommandOnKeyUpBehavior** (No sample available.)
  *Executes a command in response to a key up event matching a specified key or gesture.*

- **ExecuteCommandOnLostFocusBehavior** (No sample available.)
  *Executes a command when the control loses focus.*

- **ExecuteCommandOnPinchBehavior** (No sample available.)
  *Executes a command when a pinch gesture is in progress.*

- **ExecuteCommandOnPinchEndedBehavior** (No sample available.)
  *Executes a command when a pinch gesture ends.*

- **ExecuteCommandOnPointerCaptureLostBehavior** (No sample available.)
  *Executes a command when pointer capture is lost from the control.*

- **ExecuteCommandOnPointerEnteredBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the pointer enters the control’s area.*

- **ExecuteCommandOnPointerExitedBehavior** (No sample available.)
  *Executes a command when the pointer exits the control’s area.*

- **ExecuteCommandOnPointerMovedBehavior** (No sample available.)
  *Executes a command when the pointer moves over the control.*

- **ExecuteCommandOnPointerPressedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the pointer is pressed on the control.*

- **ExecuteCommandOnPointerReleasedBehavior** (No sample available.)
  *Executes a command when the pointer is released over the control.*

- **ExecuteCommandOnPointerTouchPadGestureMagnifyBehavior** (No sample available.)
  *Executes a command during a touchpad magnify gesture.*

- **ExecuteCommandOnPointerTouchPadGestureRotateBehavior** (No sample available.)
  *Executes a command during a touchpad rotation gesture.*

- **ExecuteCommandOnPointerTouchPadGestureSwipeBehavior** (No sample available.)
  *Executes a command during a touchpad swipe gesture.*

- **ExecuteCommandOnPointerWheelChangedBehavior** (No sample available.)
  *Executes a command when the pointer wheel delta changes.*

- **ExecuteCommandOnPullGestureBehavior** (No sample available.)
  *Executes a command when a pull gesture is detected.*

- **ExecuteCommandOnPullGestureEndedBehavior** (No sample available.)
  *Executes a command when a pull gesture ends.*

- **ExecuteCommandOnRightTappedBehavior** (No sample available.)
  *Executes a command when the control is right-tapped.*

- **ExecuteCommandOnScrollGestureBehavior** (No sample available.)
  *Executes a command during a scroll gesture.*

- **ExecuteCommandOnScrollGestureEndedBehavior** (No sample available.)
  *Executes a command when a scroll gesture ends.*

- **ExecuteCommandOnScrollGestureInertiaStartingBehavior** (No sample available.)
  *Executes a command when the inertia phase of a scroll gesture starts.*

- **ExecuteCommandOnTappedBehavior** (No sample available.)
  *Executes a command when a tap event occurs.*

- **ExecuteCommandOnTextInputBehavior** (No sample available.)
  *Executes a command in response to text input events.*

- **ExecuteCommandOnTextInputMethodClientRequestedBehavior** (No sample available.)
  *Executes a command when text input method (virtual keyboard) is requested.*

- **InvokeCommandBehaviorBase** (No sample available.)
  *The base class that supports converting parameters and invoking a bound command.*

### FileUpload
- **ButtonUploadFileBehavior** (No sample available.)
  *Opens a file dialog and uploads the selected file when the button is clicked.*
- **UploadFileBehaviorBase** (No sample available.)
  *Base behavior for uploading a file to a specified URL.*
- **UploadFileAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UploadFileView.axaml))
  *Uploads a file asynchronously and invokes a command when finished.*
- **UploadCompletedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UploadFileView.axaml))
  *Invokes actions when an upload is marked complete.*

### Focus
- **AutoFocusBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoFocusBehaviorView.axaml))
  *Automatically sets the focus on the associated control when it is loaded.*
- **FocusBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnLostFocusBehaviorView.axaml))
  *Exposes a two‑way bindable IsFocused property to control focus state.*

- **FocusBehaviorBase** (No sample available.)
  *Provides a base implementation for focus behaviors, including support for navigation methods and key modifiers.*

- **FocusControlBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusControlBehaviorView.axaml))
  *Forces focus onto a specified control when triggered.*

- **FocusOnAttachedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedBehaviorView.axaml))
  *Immediately focuses the control when the behavior is attached.*

- **FocusOnAttachedToVisualTreeBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedToVisualTreeBehaviorView.axaml))
  *Focuses the control as soon as it is attached to the visual tree.*

- **FocusOnPointerMovedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnPointerMovedBehaviorView.axaml))
  *Sets focus on the control when pointer movement is detected.*

- **FocusOnPointerPressedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnPointerPressedBehaviorView.axaml))
  *Focuses the control when a pointer press event occurs.*

- **FocusOnVisibleBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnVisibleBehaviorView.axaml))
  *Automatically focuses the control when its IsVisible property changes to true.*

- **FocusSelectedItemBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusSelectedItemBehaviorView.axaml))
  *Focuses the currently selected item in an ItemsControl.*

### Gestures
- **DoubleTappedGestureTrigger** (No sample available.)
  *Triggers actions when a double-tap gesture is detected.*

- **HoldingGestureTrigger** (No sample available.)
  *Triggers actions when a holding (long press) gesture is detected.*

- **PinchEndedGestureTrigger** (No sample available.)
  *Triggers actions when a pinch gesture has ended.*

- **PinchGestureTrigger** (No sample available.)
  *Triggers actions during a pinch gesture.*

- **PointerTouchPadGestureMagnifyGestureTrigger** (No sample available.)
  *Triggers actions during a touchpad magnification gesture.*

- **PointerTouchPadGestureRotateGestureTrigger** (No sample available.)
  *Triggers actions during a touchpad rotation gesture.*

- **PointerTouchPadGestureSwipeGestureTrigger** (No sample available.)
  *Triggers actions during a touchpad swipe gesture.*

- **PullGestureEndedGestureTrigger** (No sample available.)
  *Triggers actions when a pull gesture ends.*

- **PullGestureGestureTrigger** (No sample available.)
  *Triggers actions during a pull gesture.*

- **RightTappedGestureTrigger** (No sample available.)
  *Triggers actions on a right-tap gesture.*

- **ScrollGestureEndedGestureTrigger** (No sample available.)
  *Triggers actions when a scroll gesture completes.*

- **ScrollGestureGestureTrigger** (No sample available.)
  *Triggers actions during a scroll gesture.*

- **ScrollGestureInertiaStartingGestureTrigger** (No sample available.)
  *Triggers actions when the inertia phase of a scroll gesture begins.*

- **TappedGestureTrigger** (No sample available.)
  *Triggers actions on a simple tap gesture.*

### Icon
- **PathIconDataBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Sets the Data property of a PathIcon when attached.*
- **SetPathIconDataAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Changes the Data of a PathIcon when executed.*
- **PathIconDataChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Triggers actions when a PathIcon's Data changes.*

### InputElement Actions
- **CapturePointerAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Captures the pointer (mouse, touch) to a target control so that subsequent pointer events are routed there.*

- **ReleasePointerCaptureAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Releases a previously captured pointer from the control.*

- **SetCursorFromProviderAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Sets the cursor of a control using a cursor created by an `ICursorProvider`.*
- **SetCursorAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Sets the cursor of a control to a predefined cursor.*
- **SetEnabledAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SetEnabledActionView.axaml))
  *Enables or disables the associated control.*
- **HideToolTipAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Hides the ToolTip of the target control.*
- **SetToolTipTipAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Sets the ToolTip's tip text on the associated or target control.*
- **ShowToolTipAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Shows the ToolTip for the associated or target control.*

-### InputElement Triggers
- **DoubleTappedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Listens for a double-tap event and executes its actions.*

- **GotFocusTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusTriggersView.axaml))
  *Triggers actions when the control receives focus.*

- **HoldingTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Triggers actions when a holding gesture is detected.*

- **KeyDownTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Listens for key down events and triggers actions if the pressed key (or gesture) matches the specified criteria.*
- **KeyGestureTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyGestureTriggerView.axaml))
  *Triggers actions based on a specified key gesture.*

- **KeyTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyTriggerView.axaml))
  *Listens for key down or key up events and triggers actions when the configured key or gesture occurs.*

- **KeyUpTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Listens for key up events and triggers actions when conditions are met.*

- **LostFocusTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusTriggersView.axaml))
  *Triggers actions when the control loses focus.*

- **PointerCaptureLostTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when pointer capture is lost by the control.*

- **PointerEnteredTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when the pointer enters the control’s area.*

- **PointerExitedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when the pointer exits the control’s area.*

- **PointerMovedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions on pointer movement over the control.*

- **PointerPressedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions when the pointer is pressed on the control.*

- **PointerReleasedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions when the pointer is released on the control.*

- **PointerWheelChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions on mouse wheel (or equivalent) changes.*

- **TappedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Triggers actions on a tap event.*
- **ToolTipOpeningTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Triggers actions when a tooltip is about to open.*
- **ToolTipClosingTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Triggers actions when a tooltip is closing.*

- **TextInputMethodClientRequestedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Triggers actions when a text input method (virtual keyboard) is requested.*

- **TextInputTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Triggers actions on text input events.*

### WriteableBitmap
- **IWriteableBitmapRenderer** (No sample available.)
  *Defines a method used to render into a WriteableBitmap so view models can supply drawing logic.*
- **WriteableBitmapRenderBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/WriteableBitmapView.axaml))
  *Creates a writeable bitmap and updates it using a renderer on a timer.*
- **WriteableBitmapRenderAction** (No sample available.)
  *Invokes a renderer to update a writeable bitmap.*
- **WriteableBitmapTimerTrigger** (No sample available.)
  *Fires its actions on a timer and passes the writeable bitmap as a parameter.*
- **WriteableBitmapBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/WriteableBitmapView.axaml))
  *Creates a writeable bitmap and renders it once or on demand without animation.*
- **WriteableBitmapTrigger** (No sample available.)
  *Manually executes its actions with the provided writeable bitmap when triggered.*

### RenderTargetBitmap
- **IRenderTargetBitmapRenderer** (No sample available.)
  *Defines a method used to render into a RenderTargetBitmap.*
- **IRenderTargetBitmapSimpleRenderer** (No sample available.)
  *Provides a simple rendering method for StaticRenderTargetBitmapBehavior.*
- **RenderRenderTargetBitmapAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Invokes IRenderTargetBitmapRenderHost.Render on the specified target.*
- **RenderTargetBitmapBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Creates and updates a RenderTargetBitmap via a renderer.*
- **StaticRenderTargetBitmapBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Draws once into a RenderTargetBitmap and assigns it to the associated Image.*
- **RenderTargetBitmapTrigger** (No sample available.)
  *Triggers actions when RenderTargetBitmap rendering completes.*
### ItemsControl
- **ItemNudgeDropBehavior** (No sample available.)
  *Provides “nudge” effects for items in an ItemsControl during drag–drop reordering.*

- **ItemsControlContainerClearingTrigger** (No sample available.)
  *Triggers actions when the ItemsControl clears its container(s).*

- **ItemsControlContainerEventsBehavior** (No sample available.)
  *A base behavior that listens for container events (prepared, index changed, clearing) on an ItemsControl.*

- **ItemsControlContainerIndexChangedTrigger** (No sample available.)
  *Triggers actions when the index of an item’s container changes.*

- **ItemsControlContainerPreparedTrigger** (No sample available.)
  *Triggers actions when a container for an item is prepared.*

- **ItemsControlPreparingContainerTrigger** (No sample available.)
  *Executes actions when the ItemsControl raises the PreparingContainer event.*

- **ScrollToItemBehavior** (No sample available.)
  *Automatically scrolls the ItemsControl to make a specified item visible.*

- **ScrollToItemIndexBehavior** (No sample available.)
  *Scrolls to a specific item index in the ItemsControl.*

- **AddItemToItemsControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddItemToItemsControlActionView.axaml))
  *Adds an item to an ItemsControl's collection.*

- **InsertItemToItemsControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InsertItemToItemsControlActionView.axaml))
  *Inserts an item at a specific index in an ItemsControl.*

- **ClearItemsControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClearItemsControlActionView.axaml))
  *Clears all items from an ItemsControl.*

- **RemoveItemInItemsControlAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveItemInItemsControlActionView.axaml))
  *Removes the specified item from an ItemsControl.*

### ListBox
- **ListBoxSelectAllBehavior** (No sample available.)
  *Selects all items in a ListBox when the behavior is attached.*
- **RemoveItemInListBoxAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveItemInListBoxActionView.axaml))
  *Removes the specified item from a ListBox.*

- **ListBoxUnselectAllBehavior** (No sample available.)
  *Clears the selection in a ListBox.*

### ListBoxItem
- **SelectListBoxItemOnPointerMovedBehavior** (No sample available.)
  *Automatically selects a ListBoxItem when the pointer moves over it.*
- **InlineEditBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Toggles between a display element and a TextBox editor to enable inline editing (activated by double-tap or F2; ends on Enter, Escape, or losing focus).* 

### Responsive
- **AdaptiveBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AdaptiveBehaviorView.axaml))
  *Observes bounds changes of a control (or a specified source) and conditionally adds or removes CSS-style classes based on adaptive rules.*

- **AdaptiveClassSetter** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AdaptiveBehaviorView.axaml))
  *Specifies comparison conditions (min/max width/height) and the class to apply when those conditions are met.*

- **AspectRatioBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AspectRatioBehaviorView.axaml))
  *Observes bounds changes and toggles CSS-style classes when the control's aspect ratio matches specified rules.*

- **AspectRatioClassSetter** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AspectRatioBehaviorView.axaml))
  *Defines aspect ratio comparison conditions and the class to apply when those conditions are met.*

### ScrollViewer
- **HorizontalScrollViewerBehavior** (No sample available.)
  *Enables horizontal scrolling via the pointer wheel. Optionally requires the Shift key and supports line or page scrolling.*
- **ViewportBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewportBehaviorView.axaml))
  *Tracks when the associated element enters or exits a ScrollViewer's viewport.*

### Screen
- **ActiveScreenBehavior** (No sample available.)
  *Provides the currently active screen for a window.*
- **RequestScreenDetailsAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ScreenView.axaml))
  *Requests extended screen information using a Screens instance.*
- **ScreensChangedTrigger** (No sample available.)
  *Triggers actions when the available screens change.*

### SelectingItemsControl
- **SelectingItemsControlEventsBehavior** (No sample available.)
  *Handles selection-changed events in controls that support item selection (like ListBox) to trigger custom actions.*
  - **SelectingItemsControlSearchBehavior** ([Sample](samples/BehaviorsTestApplication/Views/MainView.axaml))
    *Enables searching and highlights matching items within a SelectingItemsControl.*
    Sorting can be enabled with `EnableSorting` and configured using the `SortOrder` property (ascending by default).

### Show
- **ShowBehaviorBase** (No sample available.)
  *A base class for behaviors that “show” (make visible) a target control when a trigger condition is met.*

- **ShowOnDoubleTappedBehavior** (No sample available.)
  *Shows a control when a double-tap gesture is detected.*

- **ShowOnKeyDownBehavior** (No sample available.)
  *Shows a control when a specified key (or key gesture) is pressed.*

- **ShowOnTappedBehavior** (No sample available.)
  *Shows the target control when it is tapped.*

### StorageProvider – Button
- **ButtonOpenFilePickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a file picker dialog when clicked.*

- **ButtonOpenFolderPickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a folder picker dialog when clicked.*

- **ButtonSaveFilePickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a save file picker dialog when clicked.*

### StorageProvider – Converters
- **StorageFileToReadStreamConverter** (No sample available.)
  *Converts an IStorageFile into a read stream (asynchronously).*

- **StorageFileToWriteStreamConverter** (No sample available.)
  *Converts an IStorageFile into a write stream (asynchronously).*

- **StorageItemToPathConverter** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Extracts the file system path from an IStorageItem.*

### StorageProvider – Core
- **PickerActionBase** (No sample available.)
  *Base class for actions that invoke file/folder picker dialogs.*

- **PickerBehaviorBase** (No sample available.)
  *Base class for behaviors that wrap file/folder picker functionality.*

### StorageProvider – MenuItem
- **MenuItemOpenFilePickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a file picker dialog when a MenuItem is clicked.*

- **MenuItemSaveFilePickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a save file picker dialog when a MenuItem is clicked.*

- **MenuItemOpenFolderPickerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a folder picker dialog when a MenuItem is clicked.*

### StorageProvider
- **OpenFilePickerAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a file picker dialog and passes the selected file(s) as a command parameter.*

- **OpenFilePickerBehaviorBase** (No sample available.)
  *Base behavior for opening file picker dialogs.*

- **OpenFolderPickerAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a folder picker dialog and passes the selected folder(s) as a command parameter.*

- **OpenFolderPickerBehaviorBase** (No sample available.)
  *Base behavior for opening folder picker dialogs.*

- **SaveFilePickerAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a save file picker dialog and passes the chosen file as a command parameter.*

- **SaveFilePickerBehaviorBase** (No sample available.)
  *Base behavior for saving files using a file picker dialog.*

### Scripting
- **ExecuteScriptAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteScriptActionView.axaml))
  *Executes a C# script using the Roslyn scripting API.*

### ReactiveUI
- **ClearNavigationStackAction** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resets the ReactiveUI navigation stack.*

- **NavigateAction** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates to a specified `IRoutableViewModel` using a router.*

- **NavigateToAction** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resolves and navigates to a view model type using a router.*

- **NavigateBackAction** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates back within a `RoutingState` stack.*

- **NavigateAndReset** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates to a view model and clears the navigation stack.*
- **NavigateToAndResetAction** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resolves a view model type, clears the navigation stack, and navigates to it.*
- **ObservableTriggerBehavior** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ObservableTriggerBehaviorView.axaml))
  *Subscribes to an `IObservable` and executes actions whenever the observable emits a value.*

### TextBox
- **AutoSelectBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoSelectBehaviorView.axaml))
  *Selects all text in a TextBox when it is loaded.*
- **TextBoxSelectAllOnGotFocusBehavior** (No sample available.)
  *Selects all text in a TextBox when it gains focus.*

- **TextBoxSelectAllTextBehavior** (No sample available.)
  *Selects all text in a TextBox immediately upon attachment.*

### TreeView
- **TreeViewFilterBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Filters tree view nodes based on a search box.*
- **TreeViewFilterTextChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Triggers actions when the search box text changes.*
- **ApplyTreeViewFilterAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Filters the target TreeView using a provided query.*

### TreeViewItem
- **ToggleIsExpandedOnDoubleTappedBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToggleIsExpandedOnDoubleTappedBehaviorView.axaml))
  *Toggles the IsExpanded property of a TreeViewItem when it is double-tapped.*

### SplitView
- **SplitViewStateBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Automatically updates `DisplayMode`, `PanePlacement`, and `IsPaneOpen` based on size conditions.*
- **SplitViewStateSetter** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Specifies size conditions and target values used by SplitViewStateBehavior.*
- **SplitViewPaneOpeningTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions when the pane is about to open.*
- **SplitViewPaneOpenedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions after the pane has opened.*
- **SplitViewPaneClosingTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions when the pane is about to close.*
- **SplitViewPaneClosedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions after the pane has closed.*

### Validation
- **ComboBoxValidationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ComboBoxValidationBehaviorView.axaml))
  *Validates the selected item of a ComboBox.*
- **DatePickerValidationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Validates the selected date of a DatePicker.*
- **SliderValidationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SliderValidationBehaviorView.axaml))
  *Validates the value of a range-based control like Slider.*
- **NumericUpDownValidationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Validates the value of a NumericUpDown.*
- **TextBoxValidationBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Validates the text value of a TextBox.*
- **PropertyValidationBehavior** (No sample available.)
  *Base behavior for validating an Avalonia property using rules.*
- **IValidationRule** (No sample available.)
  *Defines a method used to validate a value.*
- **RequiredTextValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Ensures text is not empty.*
- **RequiredDecimalValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Ensures a decimal value is provided.*
- **RequiredDateValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Ensures a date value is provided.*
- **MaxValueValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Checks that a numeric value does not exceed a maximum.*
- **MinValueValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Checks that a numeric value is not below a minimum.*
- **MinLengthValidationRule** (No sample available.)
  *Requires a string to have a minimum length.*
- **NotNullValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotNullValidationRuleView.axaml))
  *Ensures an object is not null.*
- **RangeValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Validates that a value is within a specified range.*
- **RegexValidationRule** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Validates a string using a regular expression.*

### Window
- **DialogOpenedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Triggers actions when a window is opened.*
- **DialogClosedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Triggers actions when a window is closed.*
- **ShowDialogAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Shows a window as a dialog.*
- **CloseWindowAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Closes a window when executed.*

## Interactivity (Infrastructure)

### AvaloniaObject
- **Action** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveElementActionView.axaml))
  *The base class for actions that can be executed by triggers.*

- **Behavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CustomBehaviorView.axaml))
  *The base class for behaviors that attach to Avalonia objects.*

- **Behavior<T>** (No sample available.)
  *Generic base class for behaviors that require a specific type of associated object.*

- **Trigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *A base class for triggers that execute a collection of actions when activated.*

- **Trigger<T>** (No sample available.)
  *Generic version of Trigger for strongly typed associated objects.*

### Collections
- **ActionCollection** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ActionCollectionTemplateView.axaml))
  *A collection of actions that can be executed by a trigger.*

- **BehaviorCollection** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BehaviorCollectionTemplateView.axaml))
  *A collection of behaviors attached to a single Avalonia object.*

### Contract
- **ComparisonConditionType** (No sample available.)
  *Defines the types of comparisons (equal, not equal, less than, etc.) used in data and adaptive triggers.*

- **IAction** (No sample available.)
  *Interface that defines the Execute method for custom actions.*

- **IBehavior** (No sample available.)
  *Interface for behaviors that can attach and detach from an Avalonia object.*

- **IBehaviorEventsHandler** (No sample available.)
  *Interface for handling events (loaded, attached, etc.) within behaviors.*

- **ITrigger** (No sample available.)
  *Interface for triggers that encapsulate a collection of actions.*

### StyledElement
- **StyledElementAction** (No sample available.)
  *A base class for actions that work with StyledElement objects.*

- **StyledElementBehavior** (No sample available.)
  *A base class for behaviors targeting StyledElement objects.*

- **StyledElementBehavior<T>** (No sample available.)
  *Generic base class for behaviors that are attached to a specific type of StyledElement.*

- **StyledElementTrigger** (No sample available.)
  *A base trigger class for StyledElement objects.*

- **StyledElementTrigger<T>** (No sample available.)
  *Generic version of the StyledElementTrigger for typed associated objects.*

### Templates
- **BehaviorCollectionTemplate** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BehaviorCollectionTemplateView.axaml))
  *Defines a XAML template for creating a collection of behaviors.*
- **ObjectTemplate** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ObjectTemplateView.axaml))
  *A template for creating custom objects.*

### Interactivity
- **Interaction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/WindowInteractionsView.axaml))
  *A static helper class for managing behavior collections and executing actions associated with triggers.*

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Xaml.Behaviors)

## License

XAML Behaviors is licensed under the [MIT license](LICENSE.TXT).
