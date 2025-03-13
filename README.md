# XAML Behaviors


[![Build Status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/wieslawsoltes.AvaloniaBehaviors?repoName=wieslawsoltes%2FAvaloniaBehaviors&branchName=master)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=90&repoName=wieslawsoltes%2FAvaloniaBehaviors&branchName=master)
[![CI](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/Avalonia.Xaml.Behaviors.svg)](https://www.nuget.org/packages/Avalonia.Xaml.Behaviors)
[![NuGet](https://img.shields.io/nuget/dt/Avalonia.Xaml.Interactivity.svg)](https://www.nuget.org/packages/Avalonia.Xaml.Interactivity)
[![MyGet](https://img.shields.io/myget/xamlbehaviors-nightly/vpre/Avalonia.Xaml.Behaviors.svg?label=myget)](https://www.myget.org/gallery/xamlbehaviors-nightly) 

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

## Docs

### Interactions

#### Actions

- AddClassAction
- ChangeAvaloniaPropertyAction
- CloseNotificationAction
- FocusControlAction
- PopupAction
- RemoveClassAction

#### Animations

- FadeInBehavior
- StartAnimationAction

#### AutoCompleteBox

- FocusAutoCompleteBoxTextBoxBehavior

#### Button

- ButtonClickEventTriggerBehavior
- ButtonExecuteCommandOnKeyDownBehavior
- ButtonHideFlyoutBehavior
- ButtonHideFlyoutOnClickBehavior

#### Clipboard

- ClearClipboardAction
- GetClipboardDataAction
- GetClipboardFormatsAction
- GetClipboardTextAction
- SetClipboardDataObjectAction
- SetClipboardTextAction

#### Composition

- SelectingItemsControlBehavior
- SlidingAnimation

#### Control

- BindPointerOverBehavior
- BindTagToVisualRootDataContextBehavior
- BoundsObserverBehavior
- DragControlBehavior
- HideAttachedFlyoutBehavior
- HideOnKeyPressedBehavior
- HideOnLostFocusBehavior
- ShowPointerPositionBehavior

#### Converters

- PointerEventArgsConverter

#### Core

- ActualThemeVariantChangedBehavior
- ActualThemeVariantChangedTrigger
- AttachedToLogicalTreeBehavior
- AttachedToLogicalTreeTrigger
- AttachedToVisualTreeBehavior
- AttachedToVisualTreeTrigger
- BindingBehavior
- BindingTriggerBehavior
- CallMethodAction
- ChangePropertyAction
- DataContextChangedBehavior
- DataContextChangedTrigger
- DataTriggerBehavior
- DetachedFromLogicalTreeTrigger
- DetachedFromVisualTreeTrigger
- DisposingBehavior
- DisposingTrigger
- EventTriggerBehavior
- InitializedBehavior
- InitializedTrigger
- InvokeCommandAction
- InvokeCommandActionBase
- LoadedBehavior
- LoadedTrigger
- ResourcesChangedBehavior
- ResourcesChangedTrigger
- RoutedEventTriggerBase
- RoutedEventTriggerBaseOfT
- RoutedEventTriggerBehavior
- UnloadedTrigger
- ValueChangedTriggerBehavior

#### DragAndDrop

- ContextDragBehavior
- ContextDropBehavior
- DropHandlerBase
- IDragHandler
- IDropHandler
- TypedDragBehavior

#### Draggable

- CanvasDragBehavior
- GridDragBehavior
- ItemDragBehavior
- SelectionAdorner

#### Events

- InteractiveBehaviorBase
- DoubleTappedEventBehavior
- GotFocusEventBehavior
- KeyDownEventBehavior
- KeyUpEventBehavior
- LostFocusEventBehavior
- PointerCaptureLostEventBehavior
- PointerEnteredEventBehavior
- PointerEventsBehavior
- PointerExitedEventBehavior
- PointerMovedEventBehavior
- PointerPressedEventBehavior
- PointerReleasedEventBehavior
- PointerWheelChangedEventBehavior
- RightTappedEventBehavior
- ScrollGestureEndedEventBehavior
- ScrollGestureEventBehavior
- TappedEventBehavior
- TextInputEventBehavior
- TextInputMethodClientRequestedEventBehavior

#### ExecuteCommand Core

- ExecuteCommandBehaviorBase
- ExecuteCommandOnKeyBehaviorBase
- ExecuteCommandRoutedEventBehaviorBase

#### ExecuteCommand

- ExecuteCommandOnActivatedBehavior
- ExecuteCommandOnDoubleTappedBehavior
- ExecuteCommandOnGotFocusBehavior
- ExecuteCommandOnHoldingBehavior
- ExecuteCommandOnKeyDownBehavior
- ExecuteCommandOnKeyUpBehavior
- ExecuteCommandOnLostFocusBehavior
- ExecuteCommandOnPinchBehavior
- ExecuteCommandOnPinchEndedBehavior
- ExecuteCommandOnPointerCaptureLostBehavior
- ExecuteCommandOnPointerEnteredBehavior
- ExecuteCommandOnPointerExitedBehavior
- ExecuteCommandOnPointerMovedBehavior
- ExecuteCommandOnPointerPressedBehavior
- ExecuteCommandOnPointerReleasedBehavior
- ExecuteCommandOnPointerTouchPadGestureMagnifyBehavior
- ExecuteCommandOnPointerTouchPadGestureRotateBehavior
- ExecuteCommandOnPointerTouchPadGestureSwipeBehavior
- ExecuteCommandOnPointerWheelChangedBehavior
- ExecuteCommandOnPullGestureBehavior
- ExecuteCommandOnPullGestureEndedBehavior
- ExecuteCommandOnRightTappedBehavior
- ExecuteCommandOnScrollGestureBehavior
- ExecuteCommandOnScrollGestureEndedBehavior
- ExecuteCommandOnScrollGestureInertiaStartingBehavior
- ExecuteCommandOnTappedBehavior
- ExecuteCommandOnTextInputBehavior
- ExecuteCommandOnTextInputMethodClientRequestedBehavior
- InvokeCommandBehaviorBase

#### Focus

- FocusBehavior
- FocusBehaviorBase
- FocusControlBehavior
- FocusOnAttachedBehavior
- FocusOnAttachedToVisualTreeBehavior
- FocusOnPointerMovedBehavior
- FocusOnPointerPressedBehavior
- FocusSelectedItemBehavior

#### Gestures

- DoubleTappedGestureTrigger
- HoldingGestureTrigger
- PinchEndedGestureTrigger
- PinchGestureTrigger
- PointerTouchPadGestureMagnifyGestureTrigger
- PointerTouchPadGestureRotateGestureTrigger
- PointerTouchPadGestureSwipeGestureTrigger
- PullGestureEndedGestureTrigger
- PullGestureGestureTrigger
- RightTappedGestureTrigger
- ScrollGestureEndedGestureTrigger
- ScrollGestureGestureTrigger
- ScrollGestureInertiaStartingGestureTrigger
- TappedGestureTrigger

#### InputElement Actions

- CapturePointerAction
- ReleasePointerCaptureAction

#### InputElement Triggers

- DoubleTappedTrigger
- GotFocusTrigger
- HoldingTrigger
- KeyDownTrigger
- KeyUpTrigger
- LostFocusTrigger
- PointerCaptureLostTrigger
- PointerEnteredTrigger
- PointerExitedTrigger
- PointerMovedTrigger
- PointerPressedTrigger
- PointerReleasedTrigger
- PointerWheelChangedTrigger
- TappedTrigger
- TextInputMethodClientRequestedTrigger
- TextInputTrigger

#### ItemsControl

- ItemNudgeDropBehavior
- ItemsControlContainerClearingTrigger
- ItemsControlContainerEventsBehavior
- ItemsControlContainerIndexChangedTrigger
- ItemsControlContainerPreparedTrigger
- ScrollToItemBehavior
- ScrollToItemIndexBehavior

#### ListBox

- ListBoxSelectAllBehavior
- ListBoxUnselectAllBehavior

#### ListBoxItem

- SelectListBoxItemOnPointerMovedBehavior

#### Responsive

- AdaptiveBehavior
- AdaptiveClassSetter

#### ScrollViewer

- HorizontalScrollViewerBehavior

#### SelectingItemsControl

- SelectingItemsControlEventsBehavior

#### Show

- ShowBehaviorBase
- ShowOnDoubleTappedBehavior
- ShowOnKeyDownBehavior
- ShowOnTappedBehavior

#### StorageProvider Button

- ButtonOpenFilePickerBehavior
- ButtonOpenFolderPickerBehavior
- ButtonSaveFilePickerBehavior

#### StorageProvider Converters

- StorageFileToReadStreamConverter
- StorageFileToWriteStreamConverter
- StorageItemToPathConverter

#### StorageProvider Core

- PickerActionBase
- PickerBehaviorBase

#### StorageProvider MenuItem

- MenuItemOpenFilePickerBehavior
- MenuItemOpenFolderPickerBehavior
- MenuItemSaveFilePickerBehavior

#### StorageProvider Utilities

- FileFilterParser

#### StorageProvider

- OpenFilePickerAction
- OpenFilePickerBehaviorBase
- OpenFolderPickerAction
- OpenFolderPickerBehaviorBase
- SaveFilePickerAction
- SaveFilePickerBehaviorBase

#### TextBox

- TextBoxSelectAllOnGotFocusBehavior
- TextBoxSelectAllTextBehavior

#### TreeViewItem

- ToggleIsExpandedOnDoubleTappedBehavior

### Interactivity

#### AvaloniaObject

- Action
- Behavior
- BehaviorOfT
- Trigger
- TriggerOfT

#### Collections

- ActionCollection
- BehaviorCollection

#### Contract

- ComparisonConditionType
- IAction
- IBehavior
- IBehaviorEventsHandler
- ITrigger

#### Helpers

- ComparisonConditionTypeHelper
- TemplatedParentHelper
- TypeConverterHelper

#### StyledElement

- StyledElementAction
- StyledElementBehavior
- StyledElementBehaviorOfT
- StyledElementTrigger
- StyledElementTriggerOfT

#### Templates

- BehaviorCollectionTemplate

#### Interactivity

- Interaction

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Xaml.Behaviors)

## License

Avalonia XAML Behaviors is licensed under the [MIT license](LICENSE.TXT).
