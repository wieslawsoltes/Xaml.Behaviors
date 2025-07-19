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
- **[AddClassAction](docs/actions/AddClassAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRemoveClassActionView.axaml))
  *Adds a style class to a control’s class collection, making it easy to change the appearance dynamically.*

- **[ChangeAvaloniaPropertyAction](docs/actions/ChangeAvaloniaPropertyAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ChangeAvaloniaPropertyActionView.axaml))
  *Changes the value of an Avalonia property on a target object at runtime.*

- **[CloseNotificationAction](docs/actions/CloseNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowNotificationActionView.axaml))
  *Closes an open notification, for example, from a notification control.*

- **[FocusControlAction](docs/actions/FocusControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusControlActionView.axaml))
  *Sets the keyboard focus on a specified control or the associated control.*

- **[HideControlAction](docs/actions/HideControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideShowControlActionView.axaml))
  *Hides a control by setting its `IsVisible` property to false.*

- **[ShowControlAction](docs/actions/ShowControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideShowControlActionView.axaml))
  *Shows a control and gives it focus when executed.*
- **[DelayedShowControlAction](docs/actions/DelayedShowControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedShowControlActionView.axaml))
  *Shows a control after waiting for a specified delay.*

- **[PopupAction](docs/actions/PopupAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHidePopupActionView.axaml))
  *Displays a popup window for showing additional UI content.*
- **[ShowPopupAction](docs/actions/ShowPopupAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Opens an existing popup for a control.*
- **[HidePopupAction](docs/actions/HidePopupAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHidePopupActionView.axaml))
  *Closes an existing popup associated with a control.*

- **[ShowFlyoutAction](docs/actions/ShowFlyoutAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHideFlyoutActionView.axaml))
  *Shows a flyout attached to a control or specified explicitly.*

- **[HideFlyoutAction](docs/actions/HideFlyoutAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowHideFlyoutActionView.axaml))
  *Hides a flyout attached to a control or specified explicitly.*

- **[RemoveClassAction](docs/actions/RemoveClassAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRemoveClassActionView.axaml))
  *Removes a style class from a control’s class collection.*

- **[RemoveElementAction](docs/actions/RemoveElementAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveElementActionView.axaml))
  *Removes a control from its parent container when executed.*
- **[MoveElementToPanelAction](docs/actions/MoveElementToPanelAction.md)** (No sample available.)
  *Moves a control to the specified panel.*

- **[ShowContextMenuAction](docs/actions/ShowContextMenuAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowContextMenuActionView.axaml))
  *Displays a control's context menu programmatically.*

- **[SplitViewTogglePaneAction](docs/actions/SplitViewTogglePaneAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Toggles the `IsPaneOpen` state of a `SplitView`.*
- **[SetViewModelPropertyAction](docs/actions/SetViewModelPropertyAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Sets a property on the DataContext to a specified value.*
- **[IncrementViewModelPropertyAction](docs/actions/IncrementViewModelPropertyAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Adds a numeric delta to a view model property.*
- **[ToggleViewModelBooleanAction](docs/actions/ToggleViewModelBooleanAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Toggles a boolean view model property.*

### Animations
- **[FadeInBehavior](docs/animations/FadeInBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FadeInBehaviorView.axaml))
  *Animates the fade-in effect for the associated element, gradually increasing its opacity.*

 - **StartAnimationAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StartAnimationActionView.axaml))
  *Triggers a defined animation on the target control when executed.*
- **[AnimateOnAttachedBehavior](docs/animations/AnimateOnAttachedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimateOnAttachedBehaviorView.axaml))
  *Runs a specified animation when the control appears in the visual tree.*
- **[StartBuiltAnimationAction](docs/animations/StartBuiltAnimationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StartBuiltAnimationActionView.axaml))
  *Creates an animation in code using an <code>IAnimationBuilder</code> and starts it.*
- **[RunAnimationTrigger](docs/animations/RunAnimationTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RunAnimationTriggerView.axaml))
  *Runs an animation and invokes actions when it completes.*
 - **IAnimationBuilder** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBuilderView.axaml))
  *Interface for providing animations from view models in an MVVM friendly way.*

- **[PlayAnimationBehavior](docs/animations/PlayAnimationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBehaviorView.axaml))
  *Runs a supplied animation automatically when the control appears in the visual tree.*

- **[BeginAnimationAction](docs/animations/BeginAnimationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationBehaviorView.axaml))
  *Starts an animation on a specified control, allowing the target to be chosen explicitly.*

- **[AnimationCompletedTrigger](docs/animations/AnimationCompletedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AnimationCompletedTriggerView.axaml))
  *Plays an animation and invokes actions once the animation finishes running.*

### Transitions
- **[TransitionsBehavior](docs/transitions/TransitionsBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsBehaviorView.axaml))
  *Sets the `Transitions` collection on the associated control when attached.*
- **[AddTransitionAction](docs/transitions/AddTransitionAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Adds a transition to a control's `Transitions` collection.*
- **[RemoveTransitionAction](docs/transitions/RemoveTransitionAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Removes a transition from a control's `Transitions` collection.*
- **[ClearTransitionsAction](docs/transitions/ClearTransitionsAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsActionsView.axaml))
  *Clears all transitions from a control.*
- **[TransitionsChangedTrigger](docs/transitions/TransitionsChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TransitionsChangedTriggerView.axaml))
  *Triggers actions when the `Transitions` property of a control changes.*

### AutoCompleteBox
 - **FocusAutoCompleteBoxTextBoxBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusAutoCompleteBoxTextBoxBehaviorView.axaml))
  *Ensures the text box within an AutoCompleteBox gets focus automatically.*
- **[AutoCompleteBoxOpenDropDownOnFocusBehavior](docs/autocompletebox/AutoCompleteBoxOpenDropDownOnFocusBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Opens the AutoCompleteBox drop-down when the control receives focus.*
- **[AutoCompleteBoxSelectionChangedTrigger](docs/autocompletebox/AutoCompleteBoxSelectionChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Triggers actions when the AutoCompleteBox selection changes.*
- **[ClearAutoCompleteBoxSelectionAction](docs/autocompletebox/ClearAutoCompleteBoxSelectionAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoCompleteBoxView.axaml))
  *Clears the AutoCompleteBox selection and text.*

### Automation
- **[AutomationNameBehavior](docs/automation/AutomationNameBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Applies an automation name to the associated control.*
- **[AutomationNameChangedTrigger](docs/automation/AutomationNameChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Triggers actions when the automation name of the control changes.*
- **[SetAutomationIdAction](docs/automation/SetAutomationIdAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutomationView.axaml))
  *Sets the automation ID on a target control.*

### Carousel
- **[CarouselKeyNavigationBehavior](docs/carousel/CarouselKeyNavigationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Enables keyboard navigation for a Carousel using arrow keys.*
- **[CarouselNextAction](docs/carousel/CarouselNextAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Advances the target Carousel to the next page.*
- **[CarouselPreviousAction](docs/carousel/CarouselPreviousAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselNavigationView.axaml))
  *Moves the target Carousel to the previous page.*
 - **CarouselSelectionChangedTrigger** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CarouselSelectionChangedTriggerView.axaml))
  *Triggers actions when the Carousel selection changes.*

### Collections
- **[AddRangeAction](docs/collections/AddRangeAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRangeActionView.axaml))
  *Adds multiple items to an `IList`.*
 - **RemoveRangeAction** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveRangeActionView.axaml))
  *Removes multiple items from an `IList`.*
- **[ClearCollectionAction](docs/collections/ClearCollectionAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddRangeActionView.axaml))
  *Clears all items from an `IList`.*
- **[CollectionChangedBehavior](docs/collections/CollectionChangedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CollectionChangedTriggerBehaviorView.axaml))
  *Invokes actions based on collection change notifications.*
- **[CollectionChangedTrigger](docs/collections/CollectionChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CollectionChangedTriggerBehaviorView.axaml))
  *Triggers actions whenever the observed collection changes.*

### TabControl
- **[TabControlKeyNavigationBehavior](docs/tabcontrol/TabControlKeyNavigationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Enables keyboard navigation for a TabControl using arrow keys.*
- **[TabControlNextAction](docs/tabcontrol/TabControlNextAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Advances the target TabControl to the next tab.*
- **[TabControlPreviousAction](docs/tabcontrol/TabControlPreviousAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlNavigationView.axaml))
  *Moves the target TabControl to the previous tab.*
- **[TabControlSelectionChangedTrigger](docs/tabcontrol/TabControlSelectionChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TabControlSelectionChangedTriggerView.axaml))
  *Triggers actions when the TabControl selection changes.*

### Button
- **[ButtonClickEventTriggerBehavior](docs/button/ButtonClickEventTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonClickEventTriggerBehaviorView.axaml))
  *Listens for a button’s click event and triggers associated actions.*

 - **ButtonExecuteCommandOnKeyDownBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonExecuteCommandOnKeyDownBehaviorView.axaml))
  *Executes a command when a specified key is pressed while the button is focused.*

 - **ButtonHideFlyoutBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonHideFlyoutBehaviorView.axaml))
  *Hides an attached flyout when the button is interacted with.*

 - **ButtonHideFlyoutOnClickBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ButtonHideFlyoutOnClickBehaviorView.axaml))
  *Automatically hides the flyout attached to the button when it is clicked.*
- **[ButtonHidePopupOnClickBehavior](docs/button/ButtonHidePopupOnClickBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Automatically closes the popup containing the button when it is clicked.*

### Clipboard
- **[ClearClipboardAction](docs/clipboard/ClearClipboardAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Clears all contents from the system clipboard.*

- **[GetClipboardDataAction](docs/clipboard/GetClipboardDataAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GetClipboardDataActionView.axaml))
  *Retrieves data from the clipboard in a specified format.*

- **[GetClipboardFormatsAction](docs/clipboard/GetClipboardFormatsAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GetClipboardFormatsActionView.axaml))
  *Retrieves the list of available formats from the clipboard.*

- **[GetClipboardTextAction](docs/clipboard/GetClipboardTextAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Retrieves plain text from the clipboard.*

- **[SetClipboardDataObjectAction](docs/clipboard/SetClipboardDataObjectAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SetClipboardDataObjectActionView.axaml))
  *Places a custom data object onto the clipboard.*

- **[SetClipboardTextAction](docs/clipboard/SetClipboardTextAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClipboardView.axaml))
  *Places text onto the clipboard.*

### Notifications
- **[NotificationManagerBehavior](docs/notifications/NotificationManagerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Creates a notification manager for a window when attached.*
- **[ShowNotificationAction](docs/notifications/ShowNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ShowNotificationActionView.axaml))
  *Shows a notification using an attached notification manager.*
- **[ShowInformationNotificationAction](docs/notifications/ShowInformationNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays an information notification via a manager.*
- **[ShowSuccessNotificationAction](docs/notifications/ShowSuccessNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays a success notification via a manager.*
- **[ShowWarningNotificationAction](docs/notifications/ShowWarningNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays a warning notification via a manager.*
- **[ShowErrorNotificationAction](docs/notifications/ShowErrorNotificationAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotificationsView.axaml))
  *Displays an error notification via a manager.*

### Composition
 - **SelectingItemsControlBehavior** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SelectingItemsControlBehaviorView.axaml))
  *Animates selection transitions in items controls such as ListBox or TabControl.*

- **[SlidingAnimation](docs/composition/SlidingAnimation.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SlidingAnimationView.axaml))
  *Provides static methods to apply sliding animations (from left, right, top, or bottom) to a control.*
- **[FluidMoveBehavior](docs/composition/FluidMoveBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FluidMoveBehaviorView.axaml))
  *Animates layout changes of a control or its children.*

### ContextDialogs
- **[ContextDialogBehavior](docs/contextdialogs/ContextDialogBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Manages a popup-based context dialog for a control.*
- **[ShowContextDialogAction](docs/contextdialogs/ShowContextDialogAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Opens a context dialog using a specified behavior.*
- **[HideContextDialogAction](docs/contextdialogs/HideContextDialogAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Closes a context dialog using a specified behavior.*
- **[ContextDialogOpenedTrigger](docs/contextdialogs/ContextDialogOpenedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Triggers actions when a context dialog is opened.*
- **[ContextDialogClosedTrigger](docs/contextdialogs/ContextDialogClosedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContextDialogView.axaml))
  *Triggers actions when a context dialog is closed.*

### Control
- **[BindPointerOverBehavior](docs/control/BindPointerOverBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindPointerOverBehaviorView.axaml))
  *Two‑way binds a boolean property to a control’s pointer-over state.*

- **[BindTagToVisualRootDataContextBehavior](docs/control/BindTagToVisualRootDataContextBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindTagToVisualRootDataContextBehaviorView.axaml))
  *Binds the control’s Tag property to the DataContext of its visual root, enabling inherited data contexts.*

- **[BoundsObserverBehavior](docs/control/BoundsObserverBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BoundsObserverBehaviorView.axaml))
  *Observes a control’s bounds changes and updates two‑way bound Width and Height properties.*

- **[DragControlBehavior](docs/control/DragControlBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CustomBehaviorView.axaml))
  *Enables a control to be moved (dragged) around by changing its RenderTransform during pointer events.*

- **[HideAttachedFlyoutBehavior](docs/control/HideAttachedFlyoutBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideAttachedFlyoutBehaviorView.axaml))
  *Hides a flyout that is attached to a control when a condition is met.*

- **[HideOnKeyPressedBehavior](docs/control/HideOnKeyPressedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnKeyPressedBehaviorView.axaml))
  *Hides the target control when a specified key is pressed.*

- **[HideOnLostFocusBehavior](docs/control/HideOnLostFocusBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnLostFocusBehaviorView.axaml))
  *Hides the target control when it loses focus.*

- **[InlineEditBehavior](docs/control/InlineEditBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Toggles display and edit controls to enable in-place text editing.*

- **[ShowPointerPositionBehavior](docs/control/ShowPointerPositionBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CustomActionView.axaml))
  *Displays the current pointer position (x, y coordinates) in a TextBlock for debugging or UI feedback.*

- **[SetCursorBehavior](docs/control/SetCursorBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CursorView.axaml))
  *Applies a custom cursor to the associated control.*

- **[PointerOverCursorBehavior](docs/control/PointerOverCursorBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CursorView.axaml))
  *Changes the cursor while the pointer is over the control and resets it on exit.*

- **[SetCursorFromProviderBehavior](docs/control/SetCursorFromProviderBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Uses an `ICursorProvider` implementation to supply the cursor for the associated control.*

- **[SizeChangedTrigger](docs/control/SizeChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SizeChangedTriggerView.axaml))
  *Triggers actions when the associated control's size changes.*

### Converters
- **[PointerEventArgsConverter](docs/converters/PointerEventArgsConverter.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Converts pointer event arguments into a tuple (x, y) representing the pointer’s location.*

### Core (General Infrastructure)
- **[ActualThemeVariantChangedBehavior](docs/core-general-infrastructure/ActualThemeVariantChangedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ActualThemeVariantChangedBehaviorView.axaml))
  *A base class for behaviors that react to theme variant changes (e.g. switching from light to dark mode).*

- **[ActualThemeVariantChangedTrigger](docs/core-general-infrastructure/ActualThemeVariantChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ActualThemeVariantChangedTriggerView.axaml))
  *Triggers actions when the actual theme variant of a control changes.*
- **[ThemeVariantBehavior](docs/core-general-infrastructure/ThemeVariantBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantView.axaml))
  *Applies a specific theme variant to the associated control.*
- **[ThemeVariantTrigger](docs/core-general-infrastructure/ThemeVariantTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantTriggerView.axaml))
  *Triggers actions when the theme variant of a control changes.*
- **[SetThemeVariantAction](docs/core-general-infrastructure/SetThemeVariantAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ThemeVariantView.axaml))
  *Sets the requested theme variant on a target control.*

- **[AttachedToLogicalTreeBehavior](docs/core-general-infrastructure/AttachedToLogicalTreeBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToLogicalTreeBehaviorView.axaml))
  *A base class for behaviors that require notification when the associated object is added to the logical tree.*

- **[AttachedToLogicalTreeTrigger](docs/core-general-infrastructure/AttachedToLogicalTreeTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToLogicalTreeTriggerView.axaml))
  *Triggers actions when an element is attached to the logical tree.*

- **[AttachedToVisualTreeBehavior](docs/core-general-infrastructure/AttachedToVisualTreeBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedToVisualTreeBehaviorView.axaml))
  *A base class for behaviors that depend on the control being attached to the visual tree.*

- **[AttachedToVisualTreeTrigger](docs/core-general-infrastructure/AttachedToVisualTreeTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AttachedToVisualTreeTriggerView.axaml))
  *Triggers actions when the associated element is added to the visual tree.*

- **[BindingBehavior](docs/core-general-infrastructure/BindingBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindingBehaviorView.axaml))
  *Establishes a binding on a target property using an Avalonia binding.*

- **[BindingTriggerBehavior](docs/core-general-infrastructure/BindingTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/BindingTriggerBehaviorView.axaml))
  *Monitors a binding’s value and triggers actions when a specified condition is met.*

- **[CallMethodAction](docs/core-general-infrastructure/CallMethodAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/CallMethodActionView.axaml))
  *Invokes a method on a target object when the action is executed.*

- **[ChangePropertyAction](docs/core-general-infrastructure/ChangePropertyAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ChangePropertyActionView.axaml))
  *Changes a property on a target object to a new value using type conversion if needed.*
- **[LaunchUriOrFileAction](docs/core-general-infrastructure/LaunchUriOrFileAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/LaunchUriOrFileActionView.axaml))
  *Opens a URI or file using the default associated application.*

- **[DataContextChangedBehavior](docs/core-general-infrastructure/DataContextChangedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataContextChangedBehaviorView.axaml))
  *A base class for behaviors that react to changes in the DataContext.*

- **[DataContextChangedTrigger](docs/core-general-infrastructure/DataContextChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataContextChangedTriggerView.axaml))
  *Triggers actions when the DataContext of a control changes.*

- **[DataTriggerBehavior](docs/core-general-infrastructure/DataTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataTriggerBehaviorView.axaml))
  *Evaluates a data binding against a given condition and triggers actions when the condition is true.*
- **[DataTrigger](docs/core-general-infrastructure/DataTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DataTriggerBehaviorView.axaml))
  *Performs actions when the bound data meets a specified condition.*
- **[PropertyChangedTrigger](docs/core-general-infrastructure/PropertyChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PropertyChangedTriggerView.axaml))
  *Triggers actions when a property value changes.*
- **[ViewModelPropertyChangedTrigger](docs/core-general-infrastructure/ViewModelPropertyChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Invokes actions when a DataContext property changes.*

- **[DetachedFromLogicalTreeTrigger](docs/core-general-infrastructure/DetachedFromLogicalTreeTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DetachedFromLogicalTreeTriggerView.axaml))
  *Triggers actions when the control is removed from the logical tree.*

- **[DetachedFromVisualTreeTrigger](docs/core-general-infrastructure/DetachedFromVisualTreeTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DetachedFromVisualTreeTriggerView.axaml))
  *Triggers actions when the control is removed from the visual tree.*

- **[DisposingBehavior](docs/core-general-infrastructure/DisposingBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposingBehaviorView.axaml))
  *A base class for behaviors that manage disposable resources automatically.*

- **[DisposingTrigger](docs/core-general-infrastructure/DisposingTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposingTriggerView.axaml))
  *A base class for triggers that need to dispose of resources when detached.*
- **[DisposableAction](docs/core-general-infrastructure/DisposableAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DisposableActionView.axaml))
  *Executes a delegate when the object is disposed.*

- **[EventTriggerBehavior](docs/core-general-infrastructure/EventTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EventTriggerBehaviorView.axaml))
  *Listens for a specified event on the associated object and triggers actions accordingly.*
- **[EventTrigger](docs/core-general-infrastructure/EventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBehaviorView.axaml))
  *Executes its actions when the configured event is raised.*
- **[TimerTrigger](docs/core-general-infrastructure/TimerTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TimerTriggerView.axaml))
  *Invokes actions repeatedly after a set interval.*

- **[InitializedBehavior](docs/core-general-infrastructure/InitializedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InitializedBehaviorView.axaml))
  *A base class for behaviors that execute code when the associated object is initialized.*

- **[InitializedTrigger](docs/core-general-infrastructure/InitializedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InitializedTriggerView.axaml))
  *Triggers actions once the control is initialized.*

- **[InvokeCommandAction](docs/core-general-infrastructure/InvokeCommandAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InvokeCommandActionView.axaml))
  *Executes a bound ICommand when the action is invoked.*

- **[InvokeCommandActionBase](docs/core-general-infrastructure/InvokeCommandActionBase.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InvokeCommandActionBaseView.axaml))
  *The base class for actions that invoke commands, with support for parameter conversion.*

- **[LoadedBehavior](docs/core-general-infrastructure/LoadedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/LoadedBehaviorView.axaml))
  *A base class for behaviors that run when a control is loaded into the visual tree.*
- **[SetViewModelPropertyOnLoadBehavior](docs/core-general-infrastructure/SetViewModelPropertyOnLoadBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewModelPropertyActionsView.axaml))
  *Sets a view model property when the associated control is loaded.*

- **[LoadedTrigger](docs/core-general-infrastructure/LoadedTrigger.md)** (No sample available.)
  *Triggers actions when the control’s Loaded event fires.*
- **[DelayedLoadBehavior](docs/core-general-infrastructure/DelayedLoadBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedLoadBehaviorView.axaml))
  *Hides the control then shows it after a specified delay when attached to the visual tree.*
- **[DelayedLoadTrigger](docs/core-general-infrastructure/DelayedLoadTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DelayedLoadTriggerView.axaml))
  *Invokes actions after the control is loaded and a delay period expires.*

- **[ResourcesChangedBehavior](docs/core-general-infrastructure/ResourcesChangedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ResourcesChangedBehaviorView.axaml))
  *A base class for behaviors that respond when a control’s resources change.*

- **[ResourcesChangedTrigger](docs/core-general-infrastructure/ResourcesChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ResourcesChangedTriggerView.axaml))
  *Triggers actions when the control’s resources are modified.*

- **[RoutedEventTriggerBase](docs/core-general-infrastructure/RoutedEventTriggerBase.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBaseView.axaml))
  *A base class for triggers that listen for a routed event and execute actions.*

- **[RoutedEventTriggerBase<T>](docs/core-general-infrastructure/RoutedEventTriggerBaseT.md)** (No sample available.)
  *Generic version of RoutedEventTriggerBase for strongly typed routed event args.*

- **[RoutedEventTriggerBehavior](docs/core-general-infrastructure/RoutedEventTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RoutedEventTriggerBehaviorView.axaml))
  *Listens for a routed event on the associated object and triggers its actions.*

- **[UnloadedTrigger](docs/core-general-infrastructure/UnloadedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UnloadedTriggerView.axaml))
  *Triggers actions when the control is unloaded from the visual tree.*

- **[ValueChangedTriggerBehavior](docs/core-general-infrastructure/ValueChangedTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ValueChangedTriggerBehaviorView.axaml))
  *Triggers actions when the value of a bound property changes.*

- **[IfElseTrigger](docs/core-general-infrastructure/IfElseTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IfElseTriggerView.axaml))
  *Executes one collection of actions when a condition is true and another when it is false.*

### DragAndDrop
- **[ContextDragBehavior](docs/draganddrop/ContextDragBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Enables drag operations using a “context” (data payload) that is carried during the drag–drop operation.*

- **[ContextDragWithDirectionBehavior](docs/draganddrop/ContextDragWithDirectionBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDragTreeViewView.axaml))
  *Starts a drag operation and includes the drag direction in the data object.*

- **[ContextDropBehavior](docs/draganddrop/ContextDropBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TypedDragBehaviorView.axaml))
  *Handles drop events and passes context data between the drag source and drop target.*

- **[DropHandlerBase](docs/draganddrop/DropHandlerBase.md)** (No sample available.)
  *Provides common helper methods (move, swap, insert) for implementing custom drop logic.*

- **[IDragHandler](docs/draganddrop/IDragHandler.md)** (No sample available.)
  *Interface for classes that handle additional logic before and after a drag–drop operation.*

- **[IDropHandler](docs/draganddrop/IDropHandler.md)** (No sample available.)
  *Interface for classes that implement validation and handling of drop operations.*

- **[DropBehaviorBase](docs/draganddrop/DropBehaviorBase.md)** (No sample available.)
  *Base class for behaviors that handle drag-and-drop events and execute commands.*

- **[ContextDragBehaviorBase](docs/draganddrop/ContextDragBehaviorBase.md)** (No sample available.)
  *Base class for context drag behaviors that initiate a drag using context data.*

- **[ContextDropBehaviorBase](docs/draganddrop/ContextDropBehaviorBase.md)** (No sample available.)
  *Base class for context drop behaviors handling dropped context data.*

- **[DragAndDropEventsBehavior](docs/draganddrop/DragAndDropEventsBehavior.md)** (No sample available.)
  *Abstract behavior used to attach handlers for drag-and-drop events.*

- **[FilesDropBehavior](docs/draganddrop/FilesDropBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContentControlFilesDropBehaviorView.axaml))
  *Executes a command with a collection of dropped files.*
- **[ContentControlFilesDropBehavior](docs/draganddrop/ContentControlFilesDropBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ContentControlFilesDropBehaviorView.axaml))
  *Executes a command with dropped files on a ContentControl.*

- **[TextDropBehavior](docs/draganddrop/TextDropBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FileDropHandlerView.axaml))
  *Executes a command with dropped text.*

- **[TypedDragBehaviorBase](docs/draganddrop/TypedDragBehaviorBase.md)** (No sample available.)
  *Base class for drag behaviors working with a specific data type.*

- **[TypedDragBehavior](docs/draganddrop/TypedDragBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TypedDragBehaviorView.axaml))
  *Provides drag behavior for items of a specified data type.*
- **[PanelDragBehavior](docs/draganddrop/PanelDragBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Starts drag operations using the dragged control as context so it can be moved between panels.*
- **[PanelDropBehavior](docs/draganddrop/PanelDropBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Accepts dragged controls and inserts them into the target panel.*

### Draggable
- **[CanvasDragBehavior](docs/draggable/CanvasDragBehavior.md)** (No sample available.)
  *Enables a control to be dragged within a Canvas by updating its RenderTransform based on pointer movements.*

- **[GridDragBehavior](docs/draggable/GridDragBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DraggableView.axaml))
  *Allows grid cells (or items) to be swapped or repositioned by dragging within a Grid layout.*

- **[ItemDragBehavior](docs/draggable/ItemDragBehavior.md)** (No sample available.)
  *Enables reordering of items in an ItemsControl by dragging and dropping items.*
- **[MouseDragElementBehavior](docs/draggable/MouseDragElementBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/MouseDragBehaviorView.axaml))
  *Allows an element to be dragged using the mouse.*
- **[MultiMouseDragElementBehavior](docs/draggable/MultiMouseDragElementBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/MouseDragBehaviorView.axaml))
  *Supports dragging multiple elements simultaneously with the mouse.*

- **[SelectionAdorner](docs/draggable/SelectionAdorner.md)** (No sample available.)
  *A visual adorner used to indicate selection or to show drag outlines during drag–drop operations.*

### Events
- **[InteractiveBehaviorBase](docs/events/InteractiveBehaviorBase.md)** (No sample available.)
  *Base class for behaviors that listen to UI events, providing common functionality for event triggers.*
- **[InteractionTriggerBehavior](docs/events/InteractionTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/InteractionTriggerBehaviorView.axaml))
  *Base behavior for creating custom event-based triggers.*

- **[DoubleTappedEventBehavior](docs/events/DoubleTappedEventBehavior.md)** (No sample available.)
  *Listens for double-tap events and triggers its actions when detected.*

- **[GotFocusEventBehavior](docs/events/GotFocusEventBehavior.md)** (No sample available.)
  *Executes actions when the associated control receives focus.*

- **[KeyDownEventBehavior](docs/events/KeyDownEventBehavior.md)** (No sample available.)
  *Monitors key down events and triggers actions when the specified key is pressed.*

- **[KeyUpEventBehavior](docs/events/KeyUpEventBehavior.md)** (No sample available.)
  *Monitors key up events and triggers actions when the specified key is released.*

- **[LostFocusEventBehavior](docs/events/LostFocusEventBehavior.md)** (No sample available.)
  *Triggers actions when the control loses focus.*

- **[PointerCaptureLostEventBehavior](docs/events/PointerCaptureLostEventBehavior.md)** (No sample available.)
  *Listens for events when pointer capture is lost and triggers associated actions.*

- **[PointerEnteredEventBehavior](docs/events/PointerEnteredEventBehavior.md)** (No sample available.)
  *Triggers actions when the pointer enters the bounds of a control.*

- **[PointerEventsBehavior](docs/events/PointerEventsBehavior.md)** (No sample available.)
  *A base class that simplifies handling of pointer events (pressed, moved, released).*

- **[PointerExitedEventBehavior](docs/events/PointerExitedEventBehavior.md)** (No sample available.)
  *Triggers actions when the pointer exits a control.*

- **[PointerMovedEventBehavior](docs/events/PointerMovedEventBehavior.md)** (No sample available.)
  *Triggers actions when the pointer moves over a control.*

- **[PointerPressedEventBehavior](docs/events/PointerPressedEventBehavior.md)** (No sample available.)
  *Triggers actions on pointer press events.*

- **[PointerReleasedEventBehavior](docs/events/PointerReleasedEventBehavior.md)** (No sample available.)
  *Triggers actions on pointer release events.*

- **[PointerWheelChangedEventBehavior](docs/events/PointerWheelChangedEventBehavior.md)** (No sample available.)
  *Triggers actions when the pointer wheel (scroll) changes.*

- **[RightTappedEventBehavior](docs/events/RightTappedEventBehavior.md)** (No sample available.)
  *Triggers actions when the control is right-tapped.*

- **[ScrollGestureEndedEventBehavior](docs/events/ScrollGestureEndedEventBehavior.md)** (No sample available.)
  *Triggers actions when a scroll gesture ends.*

- **[ScrollGestureEventBehavior](docs/events/ScrollGestureEventBehavior.md)** (No sample available.)
  *Monitors scroll gestures and triggers actions when they occur.*

- **[TappedEventBehavior](docs/events/TappedEventBehavior.md)** (No sample available.)
  *Triggers actions on simple tap events.*

- **[TextInputEventBehavior](docs/events/TextInputEventBehavior.md)** (No sample available.)
  *Listens for text input events and triggers actions accordingly.*

- **[TextInputMethodClientRequestedEventBehavior](docs/events/TextInputMethodClientRequestedEventBehavior.md)** (No sample available.)
  *Triggers actions when a text input method client is requested (for virtual keyboards, etc.).*

### Event Triggers
- **[DoubleTappedEventTrigger](docs/event-triggers/DoubleTappedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when a double-tap gesture occurs.*
- **[GotFocusEventTrigger](docs/event-triggers/GotFocusEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusEventTriggersView.axaml))
  *Triggers actions when the control receives focus.*
- **[KeyDownEventTrigger](docs/event-triggers/KeyDownEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a key is pressed.*
- **[KeyUpEventTrigger](docs/event-triggers/KeyUpEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a key is released.*
- **[LostFocusEventTrigger](docs/event-triggers/LostFocusEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusEventTriggersView.axaml))
  *Triggers actions when the control loses focus.*
- **[PointerCaptureLostEventTrigger](docs/event-triggers/PointerCaptureLostEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when pointer capture is lost.*
- **[PointerEnteredEventTrigger](docs/event-triggers/PointerEnteredEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer enters the control.*
- **[PointerEventsTrigger](docs/event-triggers/PointerEventsTrigger.md)** (No sample available.)
  *Triggers actions for pointer press, move, and release events.*
- **[PointerExitedEventTrigger](docs/event-triggers/PointerExitedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer exits the control.*
- **[PointerMovedEventTrigger](docs/event-triggers/PointerMovedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer moves.*
- **[PointerPressedEventTrigger](docs/event-triggers/PointerPressedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EventsBehaviorsView.axaml))
  *Triggers actions when the pointer is pressed.*
- **[PointerReleasedEventTrigger](docs/event-triggers/PointerReleasedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer is released.*
- **[PointerWheelChangedEventTrigger](docs/event-triggers/PointerWheelChangedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerEventTriggersView.axaml))
  *Triggers actions when the pointer wheel changes.*
- **[RightTappedEventTrigger](docs/event-triggers/RightTappedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions on a right-tap gesture.*
- **[ScrollGestureEndedEventTrigger](docs/event-triggers/ScrollGestureEndedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when a scroll gesture ends.*
- **[ScrollGestureEventTrigger](docs/event-triggers/ScrollGestureEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions during a scroll gesture.*
- **[TappedEventTrigger](docs/event-triggers/TappedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureEventTriggersView.axaml))
  *Triggers actions when the control is tapped.*
- **[TextInputEventTrigger](docs/event-triggers/TextInputEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions on text input events.*
- **[TextInputMethodClientRequestedEventTrigger](docs/event-triggers/TextInputMethodClientRequestedEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyEventTriggersView.axaml))
  *Triggers actions when a text input method client is requested.*
- **[PopupOpenedTrigger](docs/event-triggers/PopupOpenedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Triggers actions when a popup is opened.*
- **[PopupClosedTrigger](docs/event-triggers/PopupClosedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PopupEventTriggersView.axaml))
  *Triggers actions when a popup is closed.*
- **[DragEnterEventTrigger](docs/event-triggers/DragEnterEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Triggers actions when a drag operation enters the element.*
- **[DragLeaveEventTrigger](docs/event-triggers/DragLeaveEventTrigger.md)** (No sample available.)
  *Triggers actions when a drag operation leaves the element.*
- **[DragOverEventTrigger](docs/event-triggers/DragOverEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FilesPreviewView.axaml))
  *Triggers actions while a drag is over the element.*
- **[DropEventTrigger](docs/event-triggers/DropEventTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DragBetweenPanelsView.axaml))
  *Triggers actions when an item is dropped on the element.*

### ExecuteCommand Core
- **[ExecuteCommandBehaviorBase](docs/executecommand-core/ExecuteCommandBehaviorBase.md)** (No sample available.)
  *Provides the core functionality for executing a command from within a behavior.*

- **[ExecuteCommandOnKeyBehaviorBase](docs/executecommand-core/ExecuteCommandOnKeyBehaviorBase.md)** (No sample available.)
  *A base class for command behaviors triggered by key events.*

- **[ExecuteCommandRoutedEventBehaviorBase](docs/executecommand-core/ExecuteCommandRoutedEventBehaviorBase.md)** (No sample available.)
  *A base class for command behaviors that respond to routed events.*

### ExecuteCommand
- **[ExecuteCommandOnActivatedBehavior](docs/executecommand/ExecuteCommandOnActivatedBehavior.md)** (No sample available.)
  *Executes a command when the main window (or target window) is activated.*

- **[ExecuteCommandOnDoubleTappedBehavior](docs/executecommand/ExecuteCommandOnDoubleTappedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the associated control is double-tapped.*

- **[ExecuteCommandOnGotFocusBehavior](docs/executecommand/ExecuteCommandOnGotFocusBehavior.md)** (No sample available.)
  *Executes a command when the control gains focus.*

- **[ExecuteCommandOnHoldingBehavior](docs/executecommand/ExecuteCommandOnHoldingBehavior.md)** (No sample available.)
  *Executes a command when a holding (long press) gesture is detected.*

- **[ExecuteCommandOnKeyDownBehavior](docs/executecommand/ExecuteCommandOnKeyDownBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command in response to a key down event matching a specified key or gesture.*

- **[ExecuteCommandOnKeyUpBehavior](docs/executecommand/ExecuteCommandOnKeyUpBehavior.md)** (No sample available.)
  *Executes a command in response to a key up event matching a specified key or gesture.*

- **[ExecuteCommandOnLostFocusBehavior](docs/executecommand/ExecuteCommandOnLostFocusBehavior.md)** (No sample available.)
  *Executes a command when the control loses focus.*

- **[ExecuteCommandOnPinchBehavior](docs/executecommand/ExecuteCommandOnPinchBehavior.md)** (No sample available.)
  *Executes a command when a pinch gesture is in progress.*

- **[ExecuteCommandOnPinchEndedBehavior](docs/executecommand/ExecuteCommandOnPinchEndedBehavior.md)** (No sample available.)
  *Executes a command when a pinch gesture ends.*

- **[ExecuteCommandOnPointerCaptureLostBehavior](docs/executecommand/ExecuteCommandOnPointerCaptureLostBehavior.md)** (No sample available.)
  *Executes a command when pointer capture is lost from the control.*

- **[ExecuteCommandOnPointerEnteredBehavior](docs/executecommand/ExecuteCommandOnPointerEnteredBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the pointer enters the control’s area.*

- **[ExecuteCommandOnPointerExitedBehavior](docs/executecommand/ExecuteCommandOnPointerExitedBehavior.md)** (No sample available.)
  *Executes a command when the pointer exits the control’s area.*

- **[ExecuteCommandOnPointerMovedBehavior](docs/executecommand/ExecuteCommandOnPointerMovedBehavior.md)** (No sample available.)
  *Executes a command when the pointer moves over the control.*

- **[ExecuteCommandOnPointerPressedBehavior](docs/executecommand/ExecuteCommandOnPointerPressedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteCommandBehaviorsView.axaml))
  *Executes a command when the pointer is pressed on the control.*

- **[ExecuteCommandOnPointerReleasedBehavior](docs/executecommand/ExecuteCommandOnPointerReleasedBehavior.md)** (No sample available.)
  *Executes a command when the pointer is released over the control.*

- **[ExecuteCommandOnPointerTouchPadGestureMagnifyBehavior](docs/executecommand/ExecuteCommandOnPointerTouchPadGestureMagnifyBehavior.md)** (No sample available.)
  *Executes a command during a touchpad magnify gesture.*

- **[ExecuteCommandOnPointerTouchPadGestureRotateBehavior](docs/executecommand/ExecuteCommandOnPointerTouchPadGestureRotateBehavior.md)** (No sample available.)
  *Executes a command during a touchpad rotation gesture.*

- **[ExecuteCommandOnPointerTouchPadGestureSwipeBehavior](docs/executecommand/ExecuteCommandOnPointerTouchPadGestureSwipeBehavior.md)** (No sample available.)
  *Executes a command during a touchpad swipe gesture.*

- **[ExecuteCommandOnPointerWheelChangedBehavior](docs/executecommand/ExecuteCommandOnPointerWheelChangedBehavior.md)** (No sample available.)
  *Executes a command when the pointer wheel delta changes.*

- **[ExecuteCommandOnPullGestureBehavior](docs/executecommand/ExecuteCommandOnPullGestureBehavior.md)** (No sample available.)
  *Executes a command when a pull gesture is detected.*

- **[ExecuteCommandOnPullGestureEndedBehavior](docs/executecommand/ExecuteCommandOnPullGestureEndedBehavior.md)** (No sample available.)
  *Executes a command when a pull gesture ends.*

- **[ExecuteCommandOnRightTappedBehavior](docs/executecommand/ExecuteCommandOnRightTappedBehavior.md)** (No sample available.)
  *Executes a command when the control is right-tapped.*

- **[ExecuteCommandOnScrollGestureBehavior](docs/executecommand/ExecuteCommandOnScrollGestureBehavior.md)** (No sample available.)
  *Executes a command during a scroll gesture.*

- **[ExecuteCommandOnScrollGestureEndedBehavior](docs/executecommand/ExecuteCommandOnScrollGestureEndedBehavior.md)** (No sample available.)
  *Executes a command when a scroll gesture ends.*

- **[ExecuteCommandOnScrollGestureInertiaStartingBehavior](docs/executecommand/ExecuteCommandOnScrollGestureInertiaStartingBehavior.md)** (No sample available.)
  *Executes a command when the inertia phase of a scroll gesture starts.*

- **[ExecuteCommandOnTappedBehavior](docs/executecommand/ExecuteCommandOnTappedBehavior.md)** (No sample available.)
  *Executes a command when a tap event occurs.*

- **[ExecuteCommandOnTextInputBehavior](docs/executecommand/ExecuteCommandOnTextInputBehavior.md)** (No sample available.)
  *Executes a command in response to text input events.*

- **[ExecuteCommandOnTextInputMethodClientRequestedBehavior](docs/executecommand/ExecuteCommandOnTextInputMethodClientRequestedBehavior.md)** (No sample available.)
  *Executes a command when text input method (virtual keyboard) is requested.*

- **[InvokeCommandBehaviorBase](docs/executecommand/InvokeCommandBehaviorBase.md)** (No sample available.)
  *The base class that supports converting parameters and invoking a bound command.*

### FileUpload
- **[ButtonUploadFileBehavior](docs/fileupload/ButtonUploadFileBehavior.md)** (No sample available.)
  *Opens a file dialog and uploads the selected file when the button is clicked.*
- **[UploadFileBehaviorBase](docs/fileupload/UploadFileBehaviorBase.md)** (No sample available.)
  *Base behavior for uploading a file to a specified URL.*
- **[UploadFileAction](docs/fileupload/UploadFileAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UploadFileView.axaml))
  *Uploads a file asynchronously and invokes a command when finished.*
- **[UploadCompletedTrigger](docs/fileupload/UploadCompletedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/UploadFileView.axaml))
  *Invokes actions when an upload is marked complete.*

### Focus
- **[AutoFocusBehavior](docs/focus/AutoFocusBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoFocusBehaviorView.axaml))
  *Automatically sets the focus on the associated control when it is loaded.*
- **[FocusBehavior](docs/focus/FocusBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/HideOnLostFocusBehaviorView.axaml))
  *Exposes a two‑way bindable IsFocused property to control focus state.*

- **[FocusBehaviorBase](docs/focus/FocusBehaviorBase.md)** (No sample available.)
  *Provides a base implementation for focus behaviors, including support for navigation methods and key modifiers.*

- **[FocusControlBehavior](docs/focus/FocusControlBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusControlBehaviorView.axaml))
  *Forces focus onto a specified control when triggered.*

- **[FocusOnAttachedBehavior](docs/focus/FocusOnAttachedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedBehaviorView.axaml))
  *Immediately focuses the control when the behavior is attached.*

- **[FocusOnAttachedToVisualTreeBehavior](docs/focus/FocusOnAttachedToVisualTreeBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnAttachedToVisualTreeBehaviorView.axaml))
  *Focuses the control as soon as it is attached to the visual tree.*

- **[FocusOnPointerMovedBehavior](docs/focus/FocusOnPointerMovedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnPointerMovedBehaviorView.axaml))
  *Sets focus on the control when pointer movement is detected.*

- **[FocusOnPointerPressedBehavior](docs/focus/FocusOnPointerPressedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusOnPointerPressedBehaviorView.axaml))
  *Focuses the control when a pointer press event occurs.*

- **[FocusSelectedItemBehavior](docs/focus/FocusSelectedItemBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusSelectedItemBehaviorView.axaml))
  *Focuses the currently selected item in an ItemsControl.*

### Gestures
- **[DoubleTappedGestureTrigger](docs/gestures/DoubleTappedGestureTrigger.md)** (No sample available.)
  *Triggers actions when a double-tap gesture is detected.*

- **[HoldingGestureTrigger](docs/gestures/HoldingGestureTrigger.md)** (No sample available.)
  *Triggers actions when a holding (long press) gesture is detected.*

- **[PinchEndedGestureTrigger](docs/gestures/PinchEndedGestureTrigger.md)** (No sample available.)
  *Triggers actions when a pinch gesture has ended.*

- **[PinchGestureTrigger](docs/gestures/PinchGestureTrigger.md)** (No sample available.)
  *Triggers actions during a pinch gesture.*

- **[PointerTouchPadGestureMagnifyGestureTrigger](docs/gestures/PointerTouchPadGestureMagnifyGestureTrigger.md)** (No sample available.)
  *Triggers actions during a touchpad magnification gesture.*

- **[PointerTouchPadGestureRotateGestureTrigger](docs/gestures/PointerTouchPadGestureRotateGestureTrigger.md)** (No sample available.)
  *Triggers actions during a touchpad rotation gesture.*

- **[PointerTouchPadGestureSwipeGestureTrigger](docs/gestures/PointerTouchPadGestureSwipeGestureTrigger.md)** (No sample available.)
  *Triggers actions during a touchpad swipe gesture.*

- **[PullGestureEndedGestureTrigger](docs/gestures/PullGestureEndedGestureTrigger.md)** (No sample available.)
  *Triggers actions when a pull gesture ends.*

- **[PullGestureGestureTrigger](docs/gestures/PullGestureGestureTrigger.md)** (No sample available.)
  *Triggers actions during a pull gesture.*

- **[RightTappedGestureTrigger](docs/gestures/RightTappedGestureTrigger.md)** (No sample available.)
  *Triggers actions on a right-tap gesture.*

- **[ScrollGestureEndedGestureTrigger](docs/gestures/ScrollGestureEndedGestureTrigger.md)** (No sample available.)
  *Triggers actions when a scroll gesture completes.*

- **[ScrollGestureGestureTrigger](docs/gestures/ScrollGestureGestureTrigger.md)** (No sample available.)
  *Triggers actions during a scroll gesture.*

- **[ScrollGestureInertiaStartingGestureTrigger](docs/gestures/ScrollGestureInertiaStartingGestureTrigger.md)** (No sample available.)
  *Triggers actions when the inertia phase of a scroll gesture begins.*

- **[TappedGestureTrigger](docs/gestures/TappedGestureTrigger.md)** (No sample available.)
  *Triggers actions on a simple tap gesture.*

### Icon
- **[PathIconDataBehavior](docs/icon/PathIconDataBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Sets the Data property of a PathIcon when attached.*
- **[SetPathIconDataAction](docs/icon/SetPathIconDataAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Changes the Data of a PathIcon when executed.*
- **[PathIconDataChangedTrigger](docs/icon/PathIconDataChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/IconView.axaml))
  *Triggers actions when a PathIcon's Data changes.*

### InputElement Actions
- **[CapturePointerAction](docs/inputelement-actions/CapturePointerAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Captures the pointer (mouse, touch) to a target control so that subsequent pointer events are routed there.*

- **[ReleasePointerCaptureAction](docs/inputelement-actions/ReleasePointerCaptureAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Releases a previously captured pointer from the control.*

- **[SetCursorFromProviderAction](docs/inputelement-actions/SetCursorFromProviderAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Sets the cursor of a control using a cursor created by an `ICursorProvider`.*
- **[SetCursorAction](docs/inputelement-actions/SetCursorAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DrawnCursorView.axaml))
  *Sets the cursor of a control to a predefined cursor.*
- **[SetEnabledAction](docs/inputelement-actions/SetEnabledAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SetEnabledActionView.axaml))
  *Enables or disables the associated control.*
- **[HideToolTipAction](docs/inputelement-actions/HideToolTipAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Hides the ToolTip of the target control.*
- **[SetToolTipTipAction](docs/inputelement-actions/SetToolTipTipAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Sets the ToolTip's tip text on the associated or target control.*
- **[ShowToolTipAction](docs/inputelement-actions/ShowToolTipAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Shows the ToolTip for the associated or target control.*

-### InputElement Triggers
- **[DoubleTappedTrigger](docs/inputelement-actions/DoubleTappedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Listens for a double-tap event and executes its actions.*

- **[GotFocusTrigger](docs/inputelement-actions/GotFocusTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusTriggersView.axaml))
  *Triggers actions when the control receives focus.*

- **[HoldingTrigger](docs/inputelement-actions/HoldingTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Triggers actions when a holding gesture is detected.*

- **[KeyDownTrigger](docs/inputelement-actions/KeyDownTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Listens for key down events and triggers actions if the pressed key (or gesture) matches the specified criteria.*
- **[KeyGestureTrigger](docs/inputelement-actions/KeyGestureTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyGestureTriggerView.axaml))
  *Triggers actions based on a specified key gesture.*

- **[KeyTrigger](docs/inputelement-actions/KeyTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyTriggerView.axaml))
  *Listens for key down or key up events and triggers actions when the configured key or gesture occurs.*

- **[KeyUpTrigger](docs/inputelement-actions/KeyUpTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Listens for key up events and triggers actions when conditions are met.*

- **[LostFocusTrigger](docs/inputelement-actions/LostFocusTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/FocusTriggersView.axaml))
  *Triggers actions when the control loses focus.*

- **[PointerCaptureLostTrigger](docs/inputelement-actions/PointerCaptureLostTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when pointer capture is lost by the control.*

- **[PointerEnteredTrigger](docs/inputelement-actions/PointerEnteredTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when the pointer enters the control’s area.*

- **[PointerExitedTrigger](docs/inputelement-actions/PointerExitedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions when the pointer exits the control’s area.*

- **[PointerMovedTrigger](docs/inputelement-actions/PointerMovedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions on pointer movement over the control.*

- **[PointerPressedTrigger](docs/inputelement-actions/PointerPressedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions when the pointer is pressed on the control.*

- **[PointerReleasedTrigger](docs/inputelement-actions/PointerReleasedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerTriggersView.axaml))
  *Triggers actions when the pointer is released on the control.*

- **[PointerWheelChangedTrigger](docs/inputelement-actions/PointerWheelChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/PointerExtraTriggersView.axaml))
  *Triggers actions on mouse wheel (or equivalent) changes.*

- **[TappedTrigger](docs/inputelement-actions/TappedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/GestureTriggersView.axaml))
  *Triggers actions on a tap event.*
- **[ToolTipOpeningTrigger](docs/inputelement-actions/ToolTipOpeningTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Triggers actions when a tooltip is about to open.*
- **[ToolTipClosingTrigger](docs/inputelement-actions/ToolTipClosingTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToolTipHelpersView.axaml))
  *Triggers actions when a tooltip is closing.*

- **[TextInputMethodClientRequestedTrigger](docs/inputelement-actions/TextInputMethodClientRequestedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Triggers actions when a text input method (virtual keyboard) is requested.*

- **[TextInputTrigger](docs/inputelement-actions/TextInputTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/KeyInputTriggersView.axaml))
  *Triggers actions on text input events.*

### WriteableBitmap
- **[IWriteableBitmapRenderer](docs/writeablebitmap/IWriteableBitmapRenderer.md)** (No sample available.)
  *Defines a method used to render into a WriteableBitmap so view models can supply drawing logic.*
- **[WriteableBitmapRenderBehavior](docs/writeablebitmap/WriteableBitmapRenderBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/WriteableBitmapView.axaml))
  *Creates a writeable bitmap and updates it using a renderer on a timer.*
- **[WriteableBitmapRenderAction](docs/writeablebitmap/WriteableBitmapRenderAction.md)** (No sample available.)
  *Invokes a renderer to update a writeable bitmap.*
- **[WriteableBitmapTimerTrigger](docs/writeablebitmap/WriteableBitmapTimerTrigger.md)** (No sample available.)
  *Fires its actions on a timer and passes the writeable bitmap as a parameter.*
- **[WriteableBitmapBehavior](docs/writeablebitmap/WriteableBitmapBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/WriteableBitmapView.axaml))
  *Creates a writeable bitmap and renders it once or on demand without animation.*
- **[WriteableBitmapTrigger](docs/writeablebitmap/WriteableBitmapTrigger.md)** (No sample available.)
  *Manually executes its actions with the provided writeable bitmap when triggered.*

### RenderTargetBitmap
- **[IRenderTargetBitmapRenderer](docs/rendertargetbitmap/IRenderTargetBitmapRenderer.md)** (No sample available.)
  *Defines a method used to render into a RenderTargetBitmap.*
- **[IRenderTargetBitmapSimpleRenderer](docs/rendertargetbitmap/IRenderTargetBitmapSimpleRenderer.md)** (No sample available.)
  *Provides a simple rendering method for StaticRenderTargetBitmapBehavior.*
- **[RenderRenderTargetBitmapAction](docs/rendertargetbitmap/RenderRenderTargetBitmapAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Invokes IRenderTargetBitmapRenderHost.Render on the specified target.*
- **[RenderTargetBitmapBehavior](docs/rendertargetbitmap/RenderTargetBitmapBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Creates and updates a RenderTargetBitmap via a renderer.*
- **[StaticRenderTargetBitmapBehavior](docs/rendertargetbitmap/StaticRenderTargetBitmapBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RenderTargetBitmapView.axaml))
  *Draws once into a RenderTargetBitmap and assigns it to the associated Image.*
- **[RenderTargetBitmapTrigger](docs/rendertargetbitmap/RenderTargetBitmapTrigger.md)** (No sample available.)
  *Triggers actions when RenderTargetBitmap rendering completes.*
### ItemsControl
- **[ItemNudgeDropBehavior](docs/itemscontrol/ItemNudgeDropBehavior.md)** (No sample available.)
  *Provides “nudge” effects for items in an ItemsControl during drag–drop reordering.*

- **[ItemsControlContainerClearingTrigger](docs/itemscontrol/ItemsControlContainerClearingTrigger.md)** (No sample available.)
  *Triggers actions when the ItemsControl clears its container(s).*

- **[ItemsControlContainerEventsBehavior](docs/itemscontrol/ItemsControlContainerEventsBehavior.md)** (No sample available.)
  *A base behavior that listens for container events (prepared, index changed, clearing) on an ItemsControl.*

- **[ItemsControlContainerIndexChangedTrigger](docs/itemscontrol/ItemsControlContainerIndexChangedTrigger.md)** (No sample available.)
  *Triggers actions when the index of an item’s container changes.*

- **[ItemsControlContainerPreparedTrigger](docs/itemscontrol/ItemsControlContainerPreparedTrigger.md)** (No sample available.)
  *Triggers actions when a container for an item is prepared.*

- **[ItemsControlPreparingContainerTrigger](docs/itemscontrol/ItemsControlPreparingContainerTrigger.md)** (No sample available.)
  *Executes actions when the ItemsControl raises the PreparingContainer event.*

- **[ScrollToItemBehavior](docs/itemscontrol/ScrollToItemBehavior.md)** (No sample available.)
  *Automatically scrolls the ItemsControl to make a specified item visible.*

- **[ScrollToItemIndexBehavior](docs/itemscontrol/ScrollToItemIndexBehavior.md)** (No sample available.)
  *Scrolls to a specific item index in the ItemsControl.*

- **[AddItemToItemsControlAction](docs/itemscontrol/AddItemToItemsControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AddItemToItemsControlActionView.axaml))
  *Adds an item to an ItemsControl's collection.*

- **[InsertItemToItemsControlAction](docs/itemscontrol/InsertItemToItemsControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/InsertItemToItemsControlActionView.axaml))
  *Inserts an item at a specific index in an ItemsControl.*

- **[ClearItemsControlAction](docs/itemscontrol/ClearItemsControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ClearItemsControlActionView.axaml))
  *Clears all items from an ItemsControl.*

- **[RemoveItemInItemsControlAction](docs/itemscontrol/RemoveItemInItemsControlAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveItemInItemsControlActionView.axaml))
  *Removes the specified item from an ItemsControl.*

### ListBox
- **[ListBoxSelectAllBehavior](docs/listbox/ListBoxSelectAllBehavior.md)** (No sample available.)
  *Selects all items in a ListBox when the behavior is attached.*
- **[RemoveItemInListBoxAction](docs/listbox/RemoveItemInListBoxAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/RemoveItemInListBoxActionView.axaml))
  *Removes the specified item from a ListBox.*

- **[ListBoxUnselectAllBehavior](docs/listbox/ListBoxUnselectAllBehavior.md)** (No sample available.)
  *Clears the selection in a ListBox.*

### ListBoxItem
- **[SelectListBoxItemOnPointerMovedBehavior](docs/listboxitem/SelectListBoxItemOnPointerMovedBehavior.md)** (No sample available.)
  *Automatically selects a ListBoxItem when the pointer moves over it.*
- **[InlineEditBehavior](docs/listboxitem/InlineEditBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/EditableDraggableListBoxView.axaml))
  *Toggles between a display element and a TextBox editor to enable inline editing (activated by double-tap or F2; ends on Enter, Escape, or losing focus).* 

### Responsive
- **[AdaptiveBehavior](docs/responsive/AdaptiveBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AdaptiveBehaviorView.axaml))
  *Observes bounds changes of a control (or a specified source) and conditionally adds or removes CSS-style classes based on adaptive rules.*

- **[AdaptiveClassSetter](docs/responsive/AdaptiveClassSetter.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AdaptiveBehaviorView.axaml))
  *Specifies comparison conditions (min/max width/height) and the class to apply when those conditions are met.*

- **[AspectRatioBehavior](docs/responsive/AspectRatioBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AspectRatioBehaviorView.axaml))
  *Observes bounds changes and toggles CSS-style classes when the control's aspect ratio matches specified rules.*

- **[AspectRatioClassSetter](docs/responsive/AspectRatioClassSetter.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AspectRatioBehaviorView.axaml))
  *Defines aspect ratio comparison conditions and the class to apply when those conditions are met.*

### ScrollViewer
- **[HorizontalScrollViewerBehavior](docs/scrollviewer/HorizontalScrollViewerBehavior.md)** (No sample available.)
  *Enables horizontal scrolling via the pointer wheel. Optionally requires the Shift key and supports line or page scrolling.*
- **[ViewportBehavior](docs/scrollviewer/ViewportBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ViewportBehaviorView.axaml))
  *Tracks when the associated element enters or exits a ScrollViewer's viewport.*

### Screen
- **[ActiveScreenBehavior](docs/screen/ActiveScreenBehavior.md)** (No sample available.)
  *Provides the currently active screen for a window.*
- **[RequestScreenDetailsAction](docs/screen/RequestScreenDetailsAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ScreenView.axaml))
  *Requests extended screen information using a Screens instance.*
- **[ScreensChangedTrigger](docs/screen/ScreensChangedTrigger.md)** (No sample available.)
  *Triggers actions when the available screens change.*

### SelectingItemsControl
- **[SelectingItemsControlEventsBehavior](docs/selectingitemscontrol/SelectingItemsControlEventsBehavior.md)** (No sample available.)
  *Handles selection-changed events in controls that support item selection (like ListBox) to trigger custom actions.*
  - **SelectingItemsControlSearchBehavior** ([Sample](samples/BehaviorsTestApplication/Views/MainView.axaml))
    *Enables searching and highlights matching items within a SelectingItemsControl.*
    Sorting can be enabled with `EnableSorting` and configured using the `SortOrder` property (ascending by default).

### Show
- **[ShowBehaviorBase](docs/show/ShowBehaviorBase.md)** (No sample available.)
  *A base class for behaviors that “show” (make visible) a target control when a trigger condition is met.*

- **[ShowOnDoubleTappedBehavior](docs/show/ShowOnDoubleTappedBehavior.md)** (No sample available.)
  *Shows a control when a double-tap gesture is detected.*

- **[ShowOnKeyDownBehavior](docs/show/ShowOnKeyDownBehavior.md)** (No sample available.)
  *Shows a control when a specified key (or key gesture) is pressed.*

- **[ShowOnTappedBehavior](docs/show/ShowOnTappedBehavior.md)** (No sample available.)
  *Shows the target control when it is tapped.*

### StorageProvider – Button
- **[ButtonOpenFilePickerBehavior](docs/storageprovider---button/ButtonOpenFilePickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a file picker dialog when clicked.*

- **[ButtonOpenFolderPickerBehavior](docs/storageprovider---button/ButtonOpenFolderPickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a folder picker dialog when clicked.*

- **[ButtonSaveFilePickerBehavior](docs/storageprovider---button/ButtonSaveFilePickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Attaches to a Button to open a save file picker dialog when clicked.*

### StorageProvider – Converters
- **[StorageFileToReadStreamConverter](docs/storageprovider---converters/StorageFileToReadStreamConverter.md)** (No sample available.)
  *Converts an IStorageFile into a read stream (asynchronously).*

- **[StorageFileToWriteStreamConverter](docs/storageprovider---converters/StorageFileToWriteStreamConverter.md)** (No sample available.)
  *Converts an IStorageFile into a write stream (asynchronously).*

- **[StorageItemToPathConverter](docs/storageprovider---converters/StorageItemToPathConverter.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Extracts the file system path from an IStorageItem.*

### StorageProvider – Core
- **[PickerActionBase](docs/storageprovider---core/PickerActionBase.md)** (No sample available.)
  *Base class for actions that invoke file/folder picker dialogs.*

- **[PickerBehaviorBase](docs/storageprovider---core/PickerBehaviorBase.md)** (No sample available.)
  *Base class for behaviors that wrap file/folder picker functionality.*

### StorageProvider – MenuItem
- **[MenuItemOpenFilePickerBehavior](docs/storageprovider---menuitem/MenuItemOpenFilePickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a file picker dialog when a MenuItem is clicked.*

- **[MenuItemSaveFilePickerBehavior](docs/storageprovider---menuitem/MenuItemSaveFilePickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a save file picker dialog when a MenuItem is clicked.*

- **[MenuItemOpenFolderPickerBehavior](docs/storageprovider---menuitem/MenuItemOpenFolderPickerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a folder picker dialog when a MenuItem is clicked.*

### StorageProvider
- **[OpenFilePickerAction](docs/storageprovider/OpenFilePickerAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a file picker dialog and passes the selected file(s) as a command parameter.*

- **[OpenFilePickerBehaviorBase](docs/storageprovider/OpenFilePickerBehaviorBase.md)** (No sample available.)
  *Base behavior for opening file picker dialogs.*

- **[OpenFolderPickerAction](docs/storageprovider/OpenFolderPickerAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a folder picker dialog and passes the selected folder(s) as a command parameter.*

- **[OpenFolderPickerBehaviorBase](docs/storageprovider/OpenFolderPickerBehaviorBase.md)** (No sample available.)
  *Base behavior for opening folder picker dialogs.*

- **[SaveFilePickerAction](docs/storageprovider/SaveFilePickerAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/StorageProviderView.axaml))
  *Opens a save file picker dialog and passes the chosen file as a command parameter.*

- **[SaveFilePickerBehaviorBase](docs/storageprovider/SaveFilePickerBehaviorBase.md)** (No sample available.)
  *Base behavior for saving files using a file picker dialog.*

### Scripting
- **[ExecuteScriptAction](docs/scripting/ExecuteScriptAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ExecuteScriptActionView.axaml))
  *Executes a C# script using the Roslyn scripting API.*

### ReactiveUI
- **[ClearNavigationStackAction](docs/reactiveui/ClearNavigationStackAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resets the ReactiveUI navigation stack.*

- **[NavigateAction](docs/reactiveui/NavigateAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates to a specified `IRoutableViewModel` using a router.*

- **[NavigateToAction](docs/reactiveui/NavigateToAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resolves and navigates to a view model type using a router.*

- **[NavigateBackAction](docs/reactiveui/NavigateBackAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates back within a `RoutingState` stack.*

- **[NavigateAndReset](docs/reactiveui/NavigateAndReset.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Navigates to a view model and clears the navigation stack.*
- **[NavigateToAndResetAction](docs/reactiveui/NavigateToAndResetAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ReactiveNavigationView.axaml))
  *Resolves a view model type, clears the navigation stack, and navigates to it.*
- **[ObservableTriggerBehavior](docs/reactiveui/ObservableTriggerBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/ReactiveUI/ObservableTriggerBehaviorView.axaml))
  *Subscribes to an `IObservable` and executes actions whenever the observable emits a value.*

### TextBox
- **[AutoSelectBehavior](docs/textbox/AutoSelectBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/AutoSelectBehaviorView.axaml))
  *Selects all text in a TextBox when it is loaded.*
- **[TextBoxSelectAllOnGotFocusBehavior](docs/textbox/TextBoxSelectAllOnGotFocusBehavior.md)** (No sample available.)
  *Selects all text in a TextBox when it gains focus.*

- **[TextBoxSelectAllTextBehavior](docs/textbox/TextBoxSelectAllTextBehavior.md)** (No sample available.)
  *Selects all text in a TextBox immediately upon attachment.*

### TreeView
- **[TreeViewFilterBehavior](docs/treeview/TreeViewFilterBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Filters tree view nodes based on a search box.*
- **[TreeViewFilterTextChangedTrigger](docs/treeview/TreeViewFilterTextChangedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Triggers actions when the search box text changes.*
- **[ApplyTreeViewFilterAction](docs/treeview/ApplyTreeViewFilterAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TreeViewFilterView.axaml))
  *Filters the target TreeView using a provided query.*

### TreeViewItem
- **[ToggleIsExpandedOnDoubleTappedBehavior](docs/treeviewitem/ToggleIsExpandedOnDoubleTappedBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ToggleIsExpandedOnDoubleTappedBehaviorView.axaml))
  *Toggles the IsExpanded property of a TreeViewItem when it is double-tapped.*

### SplitView
- **[SplitViewStateBehavior](docs/splitview/SplitViewStateBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Automatically updates `DisplayMode`, `PanePlacement`, and `IsPaneOpen` based on size conditions.*
- **[SplitViewStateSetter](docs/splitview/SplitViewStateSetter.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Specifies size conditions and target values used by SplitViewStateBehavior.*
- **[SplitViewPaneOpeningTrigger](docs/splitview/SplitViewPaneOpeningTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions when the pane is about to open.*
- **[SplitViewPaneOpenedTrigger](docs/splitview/SplitViewPaneOpenedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions after the pane has opened.*
- **[SplitViewPaneClosingTrigger](docs/splitview/SplitViewPaneClosingTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions when the pane is about to close.*
- **[SplitViewPaneClosedTrigger](docs/splitview/SplitViewPaneClosedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SplitViewStateBehaviorView.axaml))
  *Triggers actions after the pane has closed.*

### Validation
- **[ComboBoxValidationBehavior](docs/validation/ComboBoxValidationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/ComboBoxValidationBehaviorView.axaml))
  *Validates the selected item of a ComboBox.*
- **[DatePickerValidationBehavior](docs/validation/DatePickerValidationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Validates the selected date of a DatePicker.*
- **[SliderValidationBehavior](docs/validation/SliderValidationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/SliderValidationBehaviorView.axaml))
  *Validates the value of a range-based control like Slider.*
- **[NumericUpDownValidationBehavior](docs/validation/NumericUpDownValidationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Validates the value of a NumericUpDown.*
- **[TextBoxValidationBehavior](docs/validation/TextBoxValidationBehavior.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Validates the text value of a TextBox.*
- **[PropertyValidationBehavior](docs/validation/PropertyValidationBehavior.md)** (No sample available.)
  *Base behavior for validating an Avalonia property using rules.*
- **[IValidationRule](docs/validation/IValidationRule.md)** (No sample available.)
  *Defines a method used to validate a value.*
- **[RequiredTextValidationRule](docs/validation/RequiredTextValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Ensures text is not empty.*
- **[RequiredDecimalValidationRule](docs/validation/RequiredDecimalValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Ensures a decimal value is provided.*
- **[RequiredDateValidationRule](docs/validation/RequiredDateValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Ensures a date value is provided.*
- **[MaxValueValidationRule](docs/validation/MaxValueValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Checks that a numeric value does not exceed a maximum.*
- **[MinValueValidationRule](docs/validation/MinValueValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NumericUpDownValidationBehaviorView.axaml))
  *Checks that a numeric value is not below a minimum.*
- **[MinLengthValidationRule](docs/validation/MinLengthValidationRule.md)** (No sample available.)
  *Requires a string to have a minimum length.*
- **[NotNullValidationRule](docs/validation/NotNullValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/NotNullValidationRuleView.axaml))
  *Ensures an object is not null.*
- **[RangeValidationRule](docs/validation/RangeValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DatePickerValidationBehaviorView.axaml))
  *Validates that a value is within a specified range.*
- **[RegexValidationRule](docs/validation/RegexValidationRule.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/TextBoxValidationBehaviorView.axaml))
  *Validates a string using a regular expression.*

### Window
- **[DialogOpenedTrigger](docs/window/DialogOpenedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Triggers actions when a window is opened.*
- **[DialogClosedTrigger](docs/window/DialogClosedTrigger.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Triggers actions when a window is closed.*
- **[ShowDialogAction](docs/window/ShowDialogAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
  *Shows a window as a dialog.*
- **[CloseWindowAction](docs/window/CloseWindowAction.md)** ([Sample](samples/BehaviorsTestApplication/Views/Pages/DialogActionView.axaml))
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
