# Behavior differences: Uno Platform vs Avalonia

The Uno Platform packages compile the Avalonia sources with the `UNO` symbol. Most behaviors, triggers and actions
work the same on both platforms. This article lists every place where they do not: what you observe when you use a
member on Uno Platform, the API that exists only on Uno Platform, and the features that are not available.

Members whose only difference is the underlying API call (for example `Visibility` instead of `IsVisible`, or
`ChangeView` instead of setting `Offset`) are not listed. The [known issues](known-issues.md) article lists the bugs
fixed during the port.

## How to read this article

* "On Uno Platform" describes the observable difference from the Avalonia package.
* Type names are the names you write in XAML or C#. Avalonia types used by the shared sources map to WinUI types; the
  most visible ones are listed below.
* File paths are relative to the repository root. `src/Uno/<Project>` holds the Uno-only sources of a package.

### Namespaces

Namespaces drop the `Avalonia.` prefix. WinUI XAML has no `XmlnsDefinition` (the Avalonia packages map their
namespaces to `https://github.com/avaloniaui`), so declare each namespace with `using:`.

| Avalonia | Uno Platform | Typical prefix |
|----------|--------------|----------------|
| `Avalonia.Xaml.Interactivity` | `Xaml.Interactivity` | `i` |
| `Avalonia.Xaml.Interactions.Core` | `Xaml.Interactions.Core` | `ic` |
| `Avalonia.Xaml.Interactions.Custom` (also the Animations package) | `Xaml.Interactions.Custom` | `icustom` |
| `Avalonia.Xaml.Interactions.Events` | `Xaml.Interactions.Events` | `ie` |
| `Avalonia.Xaml.Interactions.DragAndDrop` | `Xaml.Interactions.DragAndDrop` | `idd` |
| `Avalonia.Xaml.Interactions.Draggable` | `Xaml.Interactions.Draggable` | `idrag` |
| `Avalonia.Xaml.Interactions.Responsive` | `Xaml.Interactions.Responsive` | `ir` |
| `Avalonia.Xaml.Interactions.ReactiveUI` | `Xaml.Interactions.ReactiveUI` | `irx` |
| `Avalonia.Xaml.Interactions.Scripting` | `Xaml.Interactions.Scripting` | |
| `Avalonia.Xaml.Interactions.FileSystem`, `.Network` | `Xaml.Interactions.FileSystem`, `.Network` | |

### Packages

| Avalonia package | Uno Platform package |
|------------------|----------------------|
| `Xaml.Behaviors.Interactivity` | `Xaml.Behaviors.Uno.Interactivity` |
| `Xaml.Behaviors.Interactions` | `Xaml.Behaviors.Uno.Interactions` |
| `Xaml.Behaviors.Interactions.Events` | `Xaml.Behaviors.Uno.Interactions.Events` |
| `Xaml.Behaviors.Interactions.Custom` | `Xaml.Behaviors.Uno.Interactions.Custom` |
| `Xaml.Behaviors.Interactions.Responsive` | `Xaml.Behaviors.Uno.Interactions.Responsive` |
| `Xaml.Behaviors.Interactions.Draggable` | `Xaml.Behaviors.Uno.Interactions.Draggable` |
| `Xaml.Behaviors.Interactions.DragAndDrop` | `Xaml.Behaviors.Uno.Interactions.DragAndDrop` |
| `Xaml.Behaviors.Interactions.DragAndDrop.DataGrid` | `Xaml.Behaviors.Uno.Interactions.DragAndDrop.DataGrid` |
| `Xaml.Behaviors.Animations` | `Xaml.Behaviors.Uno.Animations` |
| `Xaml.Behaviors.Interactions.ReactiveUI` | `Xaml.Behaviors.Uno.Interactions.ReactiveUI` |
| `Xaml.Behaviors.Interactions.Scripting` | `Xaml.Behaviors.Uno.Interactions.Scripting` |
| `Xaml.Behaviors` (single assembly) | `Xaml.Behaviors.Uno` |
| `Xaml.Behaviors.Avalonia` (meta package) | `Xaml.Behaviors.Uno.All` |
| `Xaml.Behaviors.SourceGenerators` | same package, WinUI output |
| `Avalonia.Headless.XUnit` (Avalonia's own) | `Xaml.Behaviors.Uno.Headless`, `.Headless.XUnit`, `.Headless.Host` |

The single assembly and the meta package contain the same components on both platforms: Interactivity, Animations,
Interactions, Custom, DragAndDrop, Draggable, Events and Responsive. ReactiveUI, Scripting and DragAndDrop.DataGrid
are separate packages on both. The Uno packages target `net10.0` only; the Avalonia packages target `net8.0` and
`net10.0`.

### Type mapping

These Avalonia names become WinUI types on Uno Platform (`src/Uno/UnoPortAliases.props`). Wherever a member of an
Avalonia type uses one of them, the WinUI type is used on Uno Platform.

| Avalonia | Uno Platform (WinUI) |
|----------|----------------------|
| `AvaloniaObject`, `AvaloniaProperty` | `DependencyObject`, `DependencyProperty` |
| `Control`, `StyledElement`, `Layoutable` | `FrameworkElement` |
| `Visual`, `InputElement`, `Interactive` | `UIElement` |
| `TemplatedControl` | `Microsoft.UI.Xaml.Controls.Control` |
| `MenuItem` | `MenuFlyoutItem` |
| `Key`, `KeyModifiers` | `Windows.System.VirtualKey`, `VirtualKeyModifiers` (Alt is `Menu`, Meta is `Windows`) |
| `Point`, `Vector`, `Rect`, `Size` | `Windows.Foundation.Point` (also for `Vector`), `Rect`, `Size` |
| `PointerPressedEventArgs`, `PointerReleasedEventArgs`, `PointerEventArgs`, `PointerWheelEventArgs`, `PointerCaptureLostEventArgs` | `PointerRoutedEventArgs` |
| `KeyEventArgs` | `KeyRoutedEventArgs` |
| `TappedEventArgs` | `TappedRoutedEventArgs` (double and right taps use their own WinUI argument types) |
| `TextInputEventArgs` | `CharacterReceivedRoutedEventArgs` |
| `GotFocusEventArgs`, `FocusChangedEventArgs` | `RoutedEventArgs` |
| `NavigationDirection` | `FocusNavigationDirection` |
| `DragDropEffects` | `DataPackageOperation` (same values) |
| `Transitions`, `TransitionBase` | `TransitionCollection`, `Transition` (theme transitions) |
| `Animation` (in the animation APIs) | `Storyboard` |
| `CompositionVisual` | `Microsoft.UI.Composition.Visual` |

Controls that do not exist on WinUI map to their counterparts:

| Avalonia control | Uno Platform control |
|------------------|----------------------|
| `ListBox` | `ListView`/`GridView` (`ListViewBase`) or any `Selector`, depending on the member |
| `SelectingItemsControl` | `Selector` |
| `TabControl`, `TabItem` | `TabView`, `TabViewItem` |
| `AutoCompleteBox` | `AutoSuggestBox` |
| `Carousel` | `FlipView` |
| `Window` used as a dialog | `ContentDialog` |
| `ItemsControl` container events | `ItemsRepeater` |
| `ThemeVariant`, `ThemeVariantScope` | `ElementTheme`, any `FrameworkElement` |
| `NumericUpDown` | none (`NumberBox` has `double` values) |

## Platform-wide differences

### Lifecycle and event order

WinUI has no logical tree and no `Initialized` or `AttachedToVisualTree` events. `Interaction` raises the Avalonia
lifecycle of the behaviors from the WinUI `Loaded` and `Unloaded` events (`src/Xaml.Behaviors.Interactivity/Interaction.cs`).

| Stage | Avalonia | Uno Platform |
|-------|----------|--------------|
| `OnInitializedEvent` | `Initialized` (end of XAML initialization) | `Loaded` |
| `OnAttachedToLogicalTree` | `AttachedToLogicalTree` | `Loaded` |
| `OnAttachedToVisualTree` | `AttachedToVisualTree` | `Loaded` |
| `OnLoaded` | `Loaded` | `Loaded` |
| `OnUnloaded`, `OnDetachedFromVisualTree`, `OnDetachedFromLogicalTree` | separate events | `Unloaded`, in this order |
| `OnDataContextChangedEvent` | `DataContextChanged` | `DataContextChanged`; a change raised before the behaviors are loaded is delivered once, after `OnLoaded` |
| `OnActualThemeVariantChangedEvent` | `ActualThemeVariantChanged` | `FrameworkElement.ActualThemeChanged` |
| `OnResourcesChangedEvent` | `ResourcesChanged` | never raised (WinUI has no resources changed notification) |

* All attach stages run in one notification, in the order initialized → logical tree → visual tree → loaded. The
  detach stages run in reverse order.
* Only `FrameworkElement` hosts receive lifecycle notifications. A behavior on another `DependencyObject` is attached
  but receives no stage notification.
* A behavior added to an element that is already loaded receives all four stages at once. A behavior added from a
  `Loaded` handler is synchronized after the existing behaviors have received all four stages.
* A WinUI `Window` is not a `DependencyObject` and cannot host behaviors. Attach window-level behaviors to the window
  content.
* `Behaviors` collections created by XAML (the getter path) are detached on `Unloaded` and attached again on `Loaded`,
  like on Avalonia's `DetachedFromVisualTree`. A collection assigned with `SetBehaviors` (or `BehaviorsTemplate`) stays
  attached; only the stage notifications repeat.
* Every lifecycle trigger and behavior follows this mapping: `InitializedTrigger`, `InitializedBehavior`,
  `AttachedToLogicalTreeTrigger`, `AttachedToVisualTreeTrigger` and their behaviors fire at load time.
  `EventTriggerBehavior` with its default `EventName` (`AttachedToVisualTree`) fires on `Loaded`.

### x:Bind timing

Behaviors declared in XAML are created and attached (`OnAttached`) with the view. WinUI applies `x:Bind` values when
the view loads, so they are not set yet in `OnAttached`. Read `x:Bind` values in `OnLoaded` or later. Triggers that
evaluate a condition (`DataTriggerBehavior`, `MultiDataTriggerBehavior`, `ThemeVariantTrigger`) do not evaluate before
the element is loaded, and early data context changes are delivered after `OnLoaded`.

### Routed events

WinUI routed events always bubble. The Uno packages keep the Avalonia `RoutingStrategies` enum (`Xaml.Interactivity.RoutingStrategies`)
and map it (`src/Uno/Xaml.Behaviors.Interactivity/Compat/RoutedEvents.cs`):

| `RoutingStrategies` value | Uno Platform subscription |
|---------------------------|---------------------------|
| `Bubble` | the bubbling WinUI event |
| `Direct` | the same as `Bubble`: events raised by descendants are also received (no source filter, [#391](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391)) |
| `Tunnel` only, key events | `PreviewKeyDown`/`PreviewKeyUp` |
| `Tunnel` only, other events | the bubbling event with handled events included (`handledEventsToo`) |
| `Tunnel` + `Bubble` | one bubbling subscription with handled events included: one invocation per event, not two |

Consequences:

* A tunnel handler runs after the handlers of the element and its descendants, not before them. It can no longer
  prevent a control from handling a pointer press.
* Triggers that set `e.Handled = MarkAsHandled` (the `RoutedEventTriggerBase<T>` triggers) can clear the `Handled` flag
  of an event a control already handled, because they now receive handled events.
* Behaviors that return early on `e.Handled` (the `ExecuteCommandOn*` behaviors) do not run for events a control
  handles, even with `Tunnel`.
* `GotFocus` and `LostFocus` are plain CLR events on WinUI. Routing strategies and `handledEventsToo` are ignored, and
  `Handled` cannot be set (it always reads `false`).
* `TextInput` maps to `CharacterReceived`: one event per character, and the text is that single character.
* `Button.Click` is a CLR event, not a routed event; use `TappedEvent` for a routed trigger. WinUI buttons click on
  key **up**, so a simulated key press must include the release.
* `DragEnter` and `DragOver` handlers see the Avalonia default: an `AcceptedOperation` left at `None` is set to the
  allowed operations before the handler runs.
* `RoutedEventArgs.Source` is `OriginalSource`. `Handled` exists only on the WinUI argument types that support it.
* Remove handlers with the compat `RemoveRoutedEventHandler`. WinUI's `RemoveHandler(RoutedEvent, object)` silently
  removes nothing when given a method group (the Uno projects treat CS8974 as an error).

### Keyboard

* `Key` properties are `VirtualKey` values. XAML uses `VirtualKey` names (`Number1`, not `D1`).
* `KeyModifiers` properties are `VirtualKeyModifiers` values: write `Menu` for Alt and `Windows` for Meta.
* Key modifiers are read from the current keyboard state (`InputKeyboardSource`) when a handler asks for them, not
  captured with the event.
* `KeyGesture` is a Uno-only public type, `Xaml.Interactivity.KeyGesture`, created from a XAML string. Modifiers:
  `Ctrl`/`Control`, `Shift`, `Alt`/`Menu`, `Win`/`Windows`/`Meta`/`Cmd`. Keys: `VirtualKey` names, digits
  (`Number0`–`Number9`), `+ , - . ;`, `Plus`, `Minus`, `OemPlus`, `OemComma`, `OemMinus`, `OemPeriod`, `Return`,
  `Esc`, `Del`, `Ins`, `PgUp`, `PgDn`. Other names throw `ArgumentException`. A gesture matches only with exactly the
  same modifiers.
* Handlers that Avalonia registers on the `TopLevel` (global hot keys, default and cancel keys) are registered on the
  content of the element's `XamlRoot`. They see the keys of that window only.

### Focus

* Programmatic focus uses `Focus(FocusState.Programmatic)`. Properties typed `NavigationMethod` on Avalonia are
  `FocusState` on Uno Platform (`Unfocused`, `Pointer`, `Keyboard`, `Programmatic`).
* `IsFocused` means `FocusState != Unfocused`.
* Focus order comes from WinUI `FocusManager` (`TabIndex`, `IsTabStop`, `TabFocusNavigation`), not from Avalonia
  `KeyboardNavigation`.
* WinUI cannot clear the focus. Behaviors that clear it on Avalonia move it to the `XamlRoot` content, which must be
  focusable.

### Visibility and IsEnabled

* `IsVisible = false` is `Visibility.Collapsed`.
* `IsEnabled` exists only on `Microsoft.UI.Xaml.Controls.Control`. Panels, borders and text blocks are always
  enabled. Members that set or check `IsEnabled` (`UseCommandCanExecuteForIsEnabled`, `SetEnabledAction`, the drop
  behaviors) only consider a `Control`.

### Bindings in XAML

| Avalonia | Uno Platform |
|----------|--------------|
| `[AssignBinding]` properties receive the binding object | WinUI applies a binding instead of assigning it. The `[AssignBinding]` properties are `object` properties that receive the bound **value** (`Condition.Binding`, `BindingTriggerBehavior.Binding`, `BindingBehavior.Binding`), like `DataTriggerBehavior.Binding` on both platforms. A `BindingBase` assigned in code is still applied. |
| `[ResolveByName]`: `SourceObject="Name"`, `TargetObject="Name"`, `#name` | not available. Use `{x:Bind Name}` or `{Binding ElementName=Name}`. |
| default binding modes (for example `TwoWay`) | WinUI has no per-property default binding mode. Write `Mode=TwoWay`. `x:Bind` defaults to `OneTime`. |
| value inheritance (`Inherits`) | not available |
| `StringFormat`, `x:Static`, `x:TypeArguments`, `$parent[...]`, `OneWayToSource` | not available. Use a function in `x:Bind`, `{x:Bind ns:Type.Member}` and **closed generic subclasses** (for example `class Int32ObservableTriggerBehavior : ObservableTriggerBehavior<int>`). |
| dependency property identifiers in XAML | `{x:Bind mux:TextBlock.TextProperty}` (for example `Condition.Property`, `BindingBehavior.TargetProperty`) |

Properties that bind `TwoWay` by default on Avalonia need `Mode=TwoWay` on Uno Platform: `FocusBehavior.IsFocused`,
`BindPointerOverBehavior.IsPointerOver`, `BoundsObserverBehavior.Width` and `Height`, and
`PropertyValidationBehavior.IsValid`.

`IValueConverter` is the WinUI interface: `Convert` and `ConvertBack` take a language string instead of a
`CultureInfo`.

### Property system

* **Override metadata.** `OverrideMetadata` defaults are applied as **local values** by the behavior and action
  constructors (WinUI has one default per property). `IsSet` is then `true`, a style setter cannot override the value,
  and `ClearValue` reverts to the base default.
* **SetCurrentValue.** WinUI has no current-value layer. `SetCurrentValue` sets a local value, which replaces a one-way
  binding. Every behavior that sets a value on its associated object this way (for example `ShowBehaviorBase`,
  `SelectListBoxItemOnPointerMovedBehavior`, `ContentControlFilesDropBehavior`) replaces bindings on Uno Platform.
* **Temporary values.** Reversible changes (`ChangePropertyAction` with `IsReversible`, `UseCommandCanExecuteForIsEnabled`)
  use `DependencyPropertyValuePrecedences.Animations`. WinUI has one animation value per property, so stacked changes
  are emulated (the newest wins, the value is cleared when the last change is reverted).
* **Typed reversible changes** (`ReversiblePropertyChange<TTarget, TValue>`, used by generated change property
  actions) have no overlay: the value is set as a local value and `Revert` restores the captured value, so bindings on
  the target are overwritten.
* **Property access by name** (`ChangePropertyAction.PropertyName`, conditions, validation) resolves the static
  `{Name}Property` field or property by reflection. `(Owner.Property)` names resolve WinUI owner types (write
  `(FrameworkElement.DataContext)`, not `(StyledElement.DataContext)`). WinUI has no direct (field-backed) properties.
  The property type comes from the CLR property or the `Get{Name}` accessor, otherwise from the default value.
* **String conversion** uses `XamlBindingHelper.ConvertValue`. The Avalonia parse table (easings, key splines, cursors,
  key gestures, geometries, transforms, matrices, grid definitions, ...) does not exist. A string that cannot be
  converted becomes `null`.

### Dispatcher

`Dispatcher.UIThread` uses the main view's `CoreDispatcher` at normal priority. Dispatcher priorities are not
supported. `InvokeAsync` always queues; `Invoke` from another thread blocks until the work ran. `DispatcherTimer`
is the WinUI timer: `Tick` passes an `object` argument.

### Style classes and visual states

WinUI has no style classes. Behaviors that add or remove Avalonia classes or pseudo classes (Responsive, Draggable,
drop handlers) change visual states instead (`src/Uno/Xaml.Behaviors.Interactivity/Compat/StyleClasses.cs`):

* Adding a class goes to the visual state of that name (a leading `:` is removed). If that state does not exist, the
  PascalCase name is tried (`-` and `_` start a new word): `:dragging` → `dragging` → `Dragging`.
* Removing a class acts only when the class is the current state of its group. It goes to `Not{Name}`, then `Normal`,
  then `Default`.
* States are looked up on the element when it is a `Control`, or on the parent `Control` when the element is the root
  of its template or content. Otherwise the class is recorded but has no visual effect.

The style class actions (`AddClassAction`, `RemoveClassAction`, `ToggleClassAction`) are not available.

### Templates

| Avalonia | Uno Platform |
|----------|--------------|
| `BehaviorCollectionTemplate` | `i:Interaction.BehaviorsTemplate`: a `DataTemplate` whose root is an `i:BehaviorCollectionHost` containing the behaviors. Every element gets its own collection, so the template can be set from a style setter. Setting it replaces `Interaction.Behaviors`; `null` clears them. Another root type throws `InvalidOperationException`. |
| `ActionCollectionTemplate`, `NotificationTemplate` | not available |
| `ObjectTemplate` (build an object per use) | not available. `AddItemToItemsControlAction` and `InsertItemToItemsControlAction` take an `ItemFactory` instead. |
| `ITemplate` properties (`PlaceholderTemplate`, `Item`) | `DataTemplate`, expanded with `LoadContent()` (always a UI element) |

```xml
<Style TargetType="Button">
  <Setter Property="i:Interaction.BehaviorsTemplate">
    <Setter.Value>
      <DataTemplate>
        <i:BehaviorCollectionHost>
          <ic:EventTriggerBehavior EventName="Click">
            <ic:InvokeCommandAction Command="{Binding SaveCommand}" />
          </ic:EventTriggerBehavior>
        </i:BehaviorCollectionHost>
      </DataTemplate>
    </Setter.Value>
  </Setter>
</Style>
```

### Logical tree

* The logical parent of an element is `FrameworkElement.Parent`, otherwise its visual parent.
* Actions reach the element of their trigger through the **action tree**: the trigger publishes its associated object
  to its actions when it is attached (Avalonia uses the logical parent chain, on logical attach). Every action of every
  trigger gets it.
* `StyledElementBehavior`, `StyledElementTrigger` and `StyledElementAction` derive from `Behavior`/`Trigger`/`Action`
  (`DependencyObject`), not from `StyledElement`. They have no `Parent`, `TemplatedParent`, `Name`, `Classes` or
  `Resources`. `OnAttachedToLogicalTree` and `OnDetachedFromLogicalTree` receive the Uno-only
  `LogicalTreeAttachmentEventArgs` (one `Host` property).
* Name lookups (`SourceName`) use `FrameworkElement.FindName` on the element and each parent.

### TopLevel and Window

* The top level of an element is the `Microsoft.UI.Xaml.Window` whose `Content` shares the element's `XamlRoot`. An
  element that is not loaded has no top level.
* `ExecuteCommandBehaviorBase.TopLevel` is a `UIElement`; when it is not set, the `XamlRoot` content is used.
* A WinUI window has no data context, no `Width`/`Height` and no bindable window state.

### Clipboard and storage

* The clipboard is the application-wide WinUI clipboard (`SystemClipboard.Instance`), not the clipboard of a
  `TopLevel`. Clipboard actions accept other `IClipboard` implementations.
* File and folder pickers use the application-wide `SystemStorageProvider.Instance` (Windows.Storage pickers). Picker
  actions and behaviors accept other `IStorageProvider` implementations.
* Storage items are `Windows.Storage.IStorageFile`, `IStorageFolder` and `IStorageItem`.

### Packaging and trimming

* The Avalonia packages are marked `IsTrimmable` and `IsAotCompatible`; the Uno packages are not ([#398](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/398); the trim analyzer
  still runs). Property access by name always uses reflection on Uno Platform; the reflection-based members keep the
  same `[RequiresUnreferencedCode]` contract on both platforms.

## Interactivity

`Xaml.Behaviors.Uno.Interactivity`, namespace `Xaml.Interactivity`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `Behavior`, `Trigger`, `Action` | Base `DependencyObject`. Lifecycle from `Loaded`/`Unloaded` (see [Lifecycle](#lifecycle-and-event-order)). `OnPropertyChanged(DependencyPropertyChangedEventArgs)` is a Uno-only protected virtual; only generated properties route to it. `SetCurrentValue` sets a local value. |
| `Behavior.OnInitializedEvent` | Raised when the element loads, not at the end of XAML initialization. |
| `Behavior.OnResourcesChangedEvent` | Never raised. |
| `StyledElementBehavior`, `StyledElementTrigger`, `StyledElementAction` | Not `StyledElement`s (see [Logical tree](#logical-tree)). `StyledElementAction` has no `Initialize`/`IsInitialized` state. `StyledElementTrigger.Actions` is created as a local value. |
| `Interaction.BehaviorsTemplate`, `BehaviorCollectionHost` | Uno-only. Replace `BehaviorCollectionTemplate` (see [Templates](#templates)). |
| `BehaviorCollection`, `ActionCollection`, `ConditionCollection` | `DependencyObjectCollection` based. `BehaviorCollection` raises `VectorChanged` only; the action and condition collections also raise `CollectionChanged`. |
| `EventTriggerBehavior` (`EventTriggerBase`) | Default `EventName` (`AttachedToVisualTree`) fires on `Loaded`, also when the element is already loaded (`IsLoaded`). Other names are WinUI event names: `ButtonBase.Click` (not routed), `MenuFlyoutItem.Click`, `FlyoutBase.Opened`/`Closed` (raised asynchronously) are registered without reflection; any other event is found by reflection on the WinUI type. `SourceObject` does not resolve element names. |
| `InteractiveBehaviorBase.RoutingStrategies`, `InteractiveTriggerBase.RoutingStrategies` | Uno enum with the same values; mapped as described in [Routed events](#routed-events). |
| `InvokeCommandActionBase.UseCommandCanExecuteForIsEnabled` | Drives `IsEnabled` of the trigger's associated object (Avalonia walks the logical ancestors). Only a `Control` is affected. The value is a temporary (animation precedence) value, so a local value or binding of `IsEnabled` comes back. |
| `InvokeCommandBehaviorBase.UseCommandCanExecuteForIsEnabled` | Only a `Control` is affected. |
| `InvokeCommandActionBase.InputConverterLanguage`, `InvokeCommandBehaviorBase.InputConverterLanguage` | Passed as is to the WinUI converter (`""` when unset). Avalonia builds a `CultureInfo` (current culture when unset). |
| `Condition.Binding` | `object`: receives the bound value (see [Bindings in XAML](#bindings-in-xaml)). |
| `Condition.Property` | `DependencyProperty`, set from code or `x:Bind`. |
| `Condition.PropertyChanged` | Uno-only public event raised for every property change. |
| `RoutingStrategies` | Uno-only public enum (same values as Avalonia's). |
| `KeyGesture` | Uno-only public type (see [Keyboard](#keyboard)). |
| `RoutedEvent<TEventArgs>` | Uno-only public type: wraps a WinUI `RoutedEvent` (implicit conversion) or a CLR focus event. Used by `RoutedEventTriggerBase<T>.RoutedEvent`. |
| `DelegateAddEventHandler<TTarget, THandler>` | Uno-only public type: registers events with WinUI delegate types in `AddEventHandlerRegistry` without reflection. |
| `LogicalTreeAttachmentEventArgs` | Uno-only public type passed to `OnAttachedToLogicalTree`/`OnDetachedFromLogicalTree`. |
| `ReversiblePropertyChange<TTarget, TValue>` | Sets local values (no overlay), see [Property system](#property-system). |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `AvaloniaObjectBehaviorsExtensions` (C# 14 `Behaviors` extension property, `<Border.Behaviors>`) | Defined on `AvaloniaObject`; use `i:Interaction.Behaviors`. |
| `BehaviorCollectionTemplate`, `ActionCollectionTemplate`, `ObjectTemplate`, `NotificationTemplate` (`Templates/**`) | Avalonia `ITemplate`; WinUI templates only create UI elements. `BehaviorCollectionTemplate` is replaced by `BehaviorsTemplate`. |
| `TemplatedParentHelper` | WinUI has no settable templated parent. |
| `EventTriggerBehavior` with `EventName="ToolTipOpening"`/`"ToolTipClosing"` | WinUI elements have no such events. |
| `TopLevel.Opened` lifecycle and deferred detach of top-level behaviors | A WinUI `Window` is not an element. |
| Dispatcher priorities | Not supported by the compat dispatcher. |

## Interactions

`Xaml.Behaviors.Uno.Interactions`, namespaces `Xaml.Interactions.Core`, `.FileSystem`, `.Network`.

### Core

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `DataTriggerBehavior`, `MultiDataTriggerBehavior` | Do not evaluate before the element is loaded; changes before that are not evaluated. |
| `MultiDataTriggerBehavior` (`SourceName` of a condition) | Resolved with `FrameworkElement.FindName` on the element and each parent. |
| `TimerTrigger` | Actions receive the WinUI `DispatcherTimer.Tick` argument (`object`). |
| `EventTriggerBehavior`, `EventTrigger` | See [Interactivity](#interactivity). |
| `InvokeCommandAction` | See `InvokeCommandActionBase` in [Interactivity](#interactivity). |
| `ChangePropertyAction.PropertyName` | Names resolve WinUI owner types; no direct properties. |
| `ChangePropertyAction` (`IsReversible`) | Temporary value at animation precedence; stacked changes emulated. |
| `ChangePropertyAction.TargetObject`, `CallMethodAction.TargetObject` | Element names are not resolved: use `x:Bind` or `ElementName`. |

### Clipboard

The Avalonia clipboard sources are replaced by Uno implementations with the same public names (`src/Uno/Xaml.Behaviors.Interactions/Clipboard`).

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ClearClipboardAction`, `GetClipboardDataAction`, `GetClipboardFormatsAction`, `GetClipboardTextAction`, `SetClipboardDataObjectAction`, `SetClipboardTextAction` (`Clipboard`) | Typed `IClipboard`. When unset, `SystemClipboard.Instance` (application clipboard) is used. Avalonia uses the sender's `TopLevel.Clipboard` and does nothing without one. |
| `SetClipboardDataObjectAction.DataTransfer` | Typed `Windows.ApplicationModel.DataTransfer.DataPackage` (Avalonia: `IAsyncDataTransfer`). |
| `GetClipboardDataAction` | `"Files"` returns storage items, `"FileNames"` their non-empty paths, any other format the raw `DataPackageView.GetDataAsync` object. No application string or byte decoding. The command runs on the UI thread. |
| `GetClipboardFormatsAction` | Text and storage items are reported as `"Text"` and `"Files"`; other formats are platform format ids. |
| `IClipboard`, `SystemClipboard` (`Instance`, `TextFormat`, `FilesFormat`, `FileNamesFormat`) | Uno-only public types. |

### Storage pickers

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| Picker actions and behaviors (`StorageProvider`) | Typed `IStorageProvider` (Uno-only). When unset, `SystemStorageProvider.Instance`. |
| `SuggestedStartLocation`, `SuggestedStartLocationPath` | Typed `Windows.Storage.IStorageFolder`. The path is resolved, but `SystemStorageProvider` always starts in the documents library; a custom provider receives the folder. |
| `CreateSuggestedStartLocationDirectory` | Still creates the directory. |
| `Title` | Ignored by `SystemStorageProvider`. |
| `OpenFilePickerAction.SuggestedFileName` (open pickers) | Ignored. |
| `FileTypeFilter` (open pickers) | Only patterns are used, as a flat extension list (`*.txt` → `.txt`, `*` and `*.*` → `*`). Type names, MIME types and Apple UTIs are ignored. |
| `FileTypeChoices`, `DefaultExtension` (save pickers) | `*` patterns are dropped. Without a remaining choice, one choice for `DefaultExtension` (or `.txt`) is added. |
| `ShowOverwritePrompt` | Ignored. |
| `OpenFolderPickerAction.AllowMultiple` (folder pickers) | Ignored: at most one folder. |
| `FilePickerEventArgs.Files`, `SaveFilePickerEventArgs.File`, `FolderPickerEventArgs.Folders` and command parameters | `Windows.Storage.IStorageFile`/`IStorageFolder`. |
| `ButtonOpenFilePickerBehavior`, `ButtonOpenFolderPickerBehavior`, `ButtonSaveFilePickerBehavior` | Attach to a WinUI `Button` (`Click`). |
| `MenuItemOpenFilePickerBehavior`, `MenuItemOpenFolderPickerBehavior`, `MenuItemSaveFilePickerBehavior` | Attach to a `MenuFlyoutItem`. |
| `IStorageProvider`, `SystemStorageProvider`, `PickerOptions`, `FilePickerOpenOptions`, `FilePickerSaveOptions`, `FolderPickerOpenOptions`, `FilePickerFileType` | Uno-only public types in `Xaml.Interactions.Core` (Avalonia uses `Avalonia.Platform.Storage`). |

### Storage converters

| Converter | On Uno Platform |
|-----------|-----------------|
| `StorageItemToPathConverter` | Returns a `string` path (Avalonia: a `Uri`). |
| `StorageFileToReadStreamConverter`, `StorageFileToWriteStreamConverter` | Same result (`Task<Stream>`); WinUI converter signature. |

All three return `UnsetValue` instead of `DoNothing` for unsupported values.

### File system and network

`CreateDirectoryAction`, `DeleteDirectoryAction`, `DeleteFileAction`, `WriteTextToFileAction`,
`FileSystemWatcherTrigger`, `HttpRequestAction` and `NetworkInformationTrigger` behave the same on both platforms
(they use `System.IO` and `System.Net`).

### Not available on Uno Platform

Nothing public. The per-`TopLevel` clipboard and storage provider, clipboard byte decoding and the picker options listed
above are not supported by the system services.

## Events

`Xaml.Behaviors.Uno.Interactions.Events`, namespace `Xaml.Interactions.Events`. Every trigger and behavior subscribes
to the WinUI event of the same name; routing follows [Routed events](#routed-events).

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `PointerPressedEventTrigger`/`Behavior`, `PointerReleasedEventTrigger`/`Behavior`, `PointerMovedEventTrigger`/`Behavior`, `PointerEventsTrigger`/`Behavior` | The default `Tunnel` + `Bubble` is one bubbling subscription with handled events: it runs after controls handled the event. Arguments are `PointerRoutedEventArgs`. |
| `PointerWheelChangedEventTrigger`/`Behavior` | Same routing. Arguments are `PointerRoutedEventArgs` (the wheel delta is `GetCurrentPoint(...).Properties.MouseWheelDelta`). |
| `PointerEnteredEventTrigger`/`Behavior`, `PointerExitedEventTrigger`/`Behavior`, `PointerCaptureLostEventTrigger`/`Behavior` | The default `Direct` behaves like `Bubble`: events of descendants are also received ([#391](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391)). |
| `KeyDownEventTrigger`/`Behavior`, `KeyUpEventTrigger`/`Behavior` | Default `Tunnel` + `Bubble` includes handled events; `Tunnel` alone uses `PreviewKeyDown`/`PreviewKeyUp`. Arguments are `KeyRoutedEventArgs` (`Key` is a `VirtualKey`). |
| `TextInputEventTrigger`/`Behavior` | WinUI `CharacterReceived`: one event per character, arguments `CharacterReceivedRoutedEventArgs`. |
| `GotFocusEventTrigger`/`Behavior`, `LostFocusEventTrigger`/`Behavior` | CLR events: `RoutingStrategies` is ignored; arguments are plain `RoutedEventArgs`. |
| `TappedEventTrigger`/`Behavior`, `DoubleTappedEventTrigger`/`Behavior`, `RightTappedEventTrigger`/`Behavior` | Arguments are `TappedRoutedEventArgs`, `DoubleTappedRoutedEventArgs`, `RightTappedRoutedEventArgs`. |
| `DragEnterEventTrigger`, `DragOverEventTrigger`, `DropEventTrigger`, `DragLeaveEventTrigger` | WinUI `DragEventArgs` (also for `DragLeave`). Enter and over accept the allowed operations by default. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `ScrollGestureEventTrigger`, `ScrollGestureEventBehavior`, `ScrollGestureEndedEventTrigger`, `ScrollGestureEndedEventBehavior` | Avalonia gesture events without a WinUI routed event. |
| `TextInputMethodClientRequestedEventTrigger`, `TextInputMethodClientRequestedEventBehavior` | Avalonia IME client event without a WinUI counterpart. |

## Custom

`Xaml.Behaviors.Uno.Interactions.Custom`, namespace `Xaml.Interactions.Custom`. Properties that resolve element names
on Avalonia (`TargetControl`, `TargetObject`, `SourceControl`, `ItemsControl`, `Dialog`, `Owner`, `Flyout`, ...) need
`x:Bind` or `ElementName` on Uno Platform.

### Actions

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ChangeAvaloniaPropertyAction.TargetPropertyType` | Uno-only `Type?` property. WinUI dependency properties do not expose their type; set it to convert string values explicitly. |
| `ChangeAvaloniaPropertyAction` | Type lookup: `TargetPropertyType`, then the type of the default value, then of the current value, then `object`. An inferred type only converts strings; a string that cannot be converted is assigned as is. No read-only check. |
| `PopupAction` | No pointer placement mode. From a pointer event the popup opens at the pointer; otherwise at the sender with zero offsets. Placement is recomputed on every execution. |
| `SetEnabledAction` | Only acts on a `Control`; returns `false` for panels, borders and text blocks. |
| `SetThemeVariantAction` | `ThemeVariant` is `ElementTheme?` (`Default`, `Light`, `Dark`); `Target` is any `FrameworkElement`. Sets `RequestedTheme`; `null` means `Default`. |
| `ShowContextMenuAction` | Opens `ContextFlyout.ShowAt(control)`. |
| `LaunchUriAction` | Uses `Windows.System.Launcher`; returns `true` without a `TopLevel` launcher. |
| `RemoveElementAction` | The decorator case is a `Border`. |

### Animations

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `PlayAnimationBehavior`, `AnimateOnAttachedBehavior`, `FadeInBehavior`, `BeginAnimationAction`, `StartAnimationAction`, `StartBuiltAnimationAction`, `AnimationCompletedTrigger`, `RunAnimationTrigger` (`Animation`) | A WinUI `Storyboard` instead of an Avalonia `Animation`. Timelines without `TargetName` target the element. A storyboard is a single instance: starting it again restarts it. "Completed" is `Storyboard.Completed`. |

### AutoCompleteBox

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `AutoCompleteBoxOpenDropDownOnFocusBehavior`, `FocusAutoCompleteBoxTextBoxBehavior` | Attach to `AutoSuggestBox` (`IsSuggestionListOpen`). |
| `AutoCompleteBoxSelectionChangedTrigger` | Fires on `AutoSuggestBox.SuggestionChosen` (CLR event) with `AutoSuggestBoxSuggestionChosenEventArgs`. `EventRoutingStrategy` and `MarkAsHandled` have no effect. |
| `ClearAutoCompleteBoxSelectionAction` | No `SelectedItem`: closes the suggestion list and clears `Text`. |

### Automation

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ScreenReaderAnnounceAction` | Raises a UI Automation notification through the element's automation peer (Avalonia requests a top-level screen reader announcement). |

### Behaviors, Button, Carousel, Composition

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `AutoScrollToBottomBehavior` | Watches `ItemsControl.ItemsSource` (not items added directly to `Items`). User scrolling is detected on non-intermediate `ViewChanged`. |
| `ButtonClickEventTriggerBehavior.KeyModifiers` | `VirtualKeyModifiers`; modifiers tracked with `PreviewKeyDown`/`PreviewKeyUp`. |
| `ButtonExecuteCommandOnKeyDownBehavior` | Listens on the `XamlRoot` content; `FocusTopLevel` focuses it. |
| `CarouselNextAction`, `CarouselPreviousAction`, `CarouselKeyNavigationBehavior` | Work with `FlipView`. Next/previous stay within bounds (`FlipView` has no `WrapSelection`). |
| `CarouselSelectionChangedTrigger` | `Selector.SelectionChanged` CLR event: routing and `MarkAsHandled` do not apply. |
| `ParallaxBehavior` | Implements `IObserver<Windows.Foundation.Point>`. Finds the parent `ScrollViewer` in the visual tree and updates on every `ViewChanged`, including intermediate ones. |

### ContextDialogs, Control, Converters

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ContextDialogBehavior.Placement` | WinUI `PopupPlacementMode` (no `Pointer` value), applied as `DesiredPlacement`. The dialog is a `Popup`. |
| `BindPointerOverBehavior` | Driven by `PointerEntered`/`PointerExited`/`PointerCanceled` (WinUI has no `IsPointerOver`). `IsPointerOver` needs `Mode=TwoWay`. |
| `BoundsObserverBehavior` | `Bounds` is a `Windows.Foundation.Rect`. `Width`/`Height` need `Mode=TwoWay`. |
| `BindTagToVisualRootDataContextBehavior` | Binds to the data context of the `XamlRoot` content. |
| `InlineEditBehavior` | Focusing the editor and selecting its text are posted to the dispatcher. Keys are `VirtualKey`. |
| `PointerEventArgsConverter` | Accepts `PointerRoutedEventArgs` only (positions relative to `OriginalSource`). A wheel event returns the delta in notches (delta / 120). |

### Core

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `BindingBehavior.Binding` | `object` with value semantics: the bound value is pushed one way to `TargetProperty` of `TargetObject`, again when `Binding`, `TargetObject` or `TargetProperty` change. A `null` value clears the target. The value is cleared when the behavior leaves the visual tree. A `BindingBase` assigned in code is applied. |
| `BindingTriggerBehavior.Binding` | `object`: the bound value is compared. |
| `RoutedEventTriggerBehavior` | `RoutedEvent` is a WinUI identifier (for example `UIElement.KeyDownEvent`). The handler is always removed on unload. |
| `ObservableTriggerBehavior<T>` | Use a closed subclass in XAML. |
| `AsyncLoadBehavior` | Runs on `Loaded`. |
| `InitializedTrigger`, `InitializedBehavior`, `AttachedToLogicalTreeTrigger`, `AttachedToLogicalTreeBehavior`, `DetachedFromLogicalTreeTrigger` | Raised from `Loaded`/`Unloaded` (see [Lifecycle](#lifecycle-and-event-order)). |
| `ActualThemeVariantChangedTrigger`, `ActualThemeVariantChangedBehavior` | Raised from `FrameworkElement.ActualThemeChanged`. |

### Dialog

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `DialogOpenedTrigger`, `DialogClosedTrigger` | `SourceObject` is a `ContentDialog`; actions receive `ContentDialogOpenedEventArgs`/`ContentDialogClosedEventArgs`. |
| `ShowDialogAction.Dialog` | A `ContentDialog`, shown with `ShowAsync()` in the `XamlRoot` of `Owner.Content` or of the sender. Always modal. Returns `false` without a `XamlRoot`. |
| `ShowDialogAction.Owner` | A WinUI `Window`, only used for its `XamlRoot`. |

### ExecuteCommand

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ExecuteCommandBehaviorBase.TopLevel` | `UIElement`; `FocusTopLevel` focuses `TopLevel` or the `XamlRoot` content. |
| `ExecuteCommandOnActivatedBehavior` | Watches `Window.Current` (the main window); `SourceControl` is ignored. Deactivation is filtered out. |
| `ExecuteCommandOnKeyDownBehavior`, `ExecuteCommandOnKeyUpBehavior` | `Key` is a `VirtualKey`, `Gesture` a Uno `KeyGesture`. `Tunnel` uses the preview key events. |
| `ExecuteCommandOnPointerPressedBehavior`, `...PointerReleased...`, `...PointerMoved...`, `...PointerEntered...`, `...PointerExited...`, `...PointerCaptureLost...`, `...PointerWheelChanged...`, `...Tapped...`, `...DoubleTapped...`, `...RightTapped...`, `...Holding...`, `...TextInput...` | `Tunnel` has no preview phase; events a control handles are skipped, so the command does not run on, for example, a `Button` press. |
| `ExecuteCommandOnTextInputBehavior` | One `CharacterReceived` event per character. |
| `ExecuteCommandOnGotFocusBehavior`, `ExecuteCommandOnLostFocusBehavior` | CLR focus events: routing is ignored and `MarkAsHandled` has no effect, so behaviors on ancestors also run. |

### Focus

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `FocusBehaviorBase.NavigationMethod` (`FocusOnAttachedBehavior`, `FocusOnAttachedToVisualTreeBehavior`, `FocusOnPointerPressedBehavior`, `FocusOnPointerMovedBehavior`, `FocusOnVisibleBehavior`) | `FocusState`; the default `Unfocused` is applied as `Programmatic`. |
| `FocusBehaviorBase.KeyModifiers` | Ignored. |
| `FocusBehavior.IsFocused` | Reset to `false` only when the associated object itself raises `LostFocus` (Avalonia observes `IsFocused`). Needs `Mode=TwoWay`. |
| `FocusTrapBehavior` | Focus order from WinUI `FocusManager` (`TabIndex`, `IsTabStop`); Avalonia `KeyboardNavigation` scopes do not apply. |
| `FocusSelectedItemBehavior` | Watches `Selector.SelectedItem`. |

### Gestures, Input, InputElement

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `TappedGestureTrigger`, `DoubleTappedGestureTrigger`, `RightTappedGestureTrigger`, `HoldingGestureTrigger` | WinUI gesture events; actions receive the WinUI argument types. These are the only gesture triggers. |
| `GlobalHotkeyBehavior` | `Key`/`KeyModifiers` are WinUI types. Listens to `PreviewKeyDown` on the `XamlRoot` content (that window only). |
| `InactivityTrigger` | Watches pointer moves (with handled events) and `PreviewKeyDown` on the `XamlRoot` content. |
| `LoseFocusOnEnterBehavior` | Moves focus to the `XamlRoot` content, which must be focusable (WinUI cannot clear focus). |
| `ClickEventTrigger` | Default `Tunnel` becomes bubbling pointer events with handled events and preview key events. Actions receive an empty `RoutedEventArgs`. `ClickMode` is the WinUI enum; its `Hover` value is not handled. `KeyModifiers` is `VirtualKeyModifiers?`. `IsDefault`/`IsCancel` listen on the `XamlRoot` content. A capture lost by the release itself (a `Button` source) keeps the click. |
| `CapturePointerAction` | Captures the pointer of the event for its `OriginalSource`. |
| `ReleasePointerCaptureAction` | Releases only a capture held by the event's `OriginalSource` or one of its ancestors. |
| `TextInputTrigger.Text` | `CharacterReceived` delivers one character per event, so a `Text` longer than one character never matches. |
| `KeyTrigger`, `KeyDownTrigger`, `KeyUpTrigger` | `Key` is a `VirtualKey`, `Gesture` a Uno `KeyGesture`. The default `Direct` behaves like `Bubble`: keys of child elements also fire ([#391](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391)). |
| `KeyGestureTrigger` | Default `Tunnel` + `Bubble`: one bubbling key subscription with handled events. |
| `PointerPressedTrigger`, `PointerReleasedTrigger`, `PointerMovedTrigger`, `PointerWheelChangedTrigger` | Default `Tunnel` + `Bubble` includes handled events, and `MarkAsHandled = false` clears `Handled` of an event a control already handled. |
| `GotFocusTrigger`, `LostFocusTrigger` | CLR events: `EventRoutingStrategy` and `MarkAsHandled` have no effect. |
| Every InputElement trigger | Actions receive WinUI arguments: `PointerRoutedEventArgs`, `KeyRoutedEventArgs`, `CharacterReceivedRoutedEventArgs`, `RoutedEventArgs`. |

### ItemsControl

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `AddItemToItemsControlAction.ItemFactory`, `InsertItemToItemsControlAction.ItemFactory` | Uno-only `IItemFactory?` property. When set, `CreateItem()` creates the item on every execution and takes precedence over `Item`. |
| `IItemFactory` | Uno-only public interface (`object? CreateItem()`), the replacement for `ObjectTemplate`. Implement it in the view model layer. |
| `AddItemToItemsControlAction.Item`, `InsertItemToItemsControlAction.Item` | Only a `DataTemplate` is expanded (`LoadContent()`). |
| `ItemsControlContainerPreparedTrigger`, `ItemsControlContainerClearingTrigger`, `ItemsControlContainerIndexChangedTrigger` | Attach to an `ItemsRepeater` (`ElementPrepared`, `ElementClearing`, `ElementIndexChanged`); actions receive the `ItemsRepeaterElement*EventArgs`. They subscribe when attached. |
| `ItemsControlContainerEventsBehavior` | Attaches to an `ItemsRepeater`. `OnPreparingContainer` is never called. |
| `ScrollToItemBehavior`, `ScrollToItemIndexBehavior` | Attach to a `ListViewBase` (`ListView`, `GridView`). An index out of range is ignored. |
| `ItemNudgeDropBehavior` | Uses `TranslateTransform`. Midpoints include the scroll offset for every items control. |

```xml
<icustom:AddItemToItemsControlAction ItemsControl="{x:Bind ItemsControl}">
  <icustom:AddItemToItemsControlAction.ItemFactory>
    <vm:ItemViewModelFactory Value="Added Item" />
  </icustom:AddItemToItemsControlAction.ItemFactory>
</icustom:AddItemToItemsControlAction>
```

### ListBox and SelectingItemsControl

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ListBoxSelectAllBehavior` | Attaches to a `ListViewBase` (a WinUI `ListBox` is not accepted). Adds every item to `SelectedItems`; `Single` mode selects index 0, `None` does nothing. |
| `ListBoxUnselectAllBehavior` | Attaches to a `ListViewBase`. Clears `SelectedItems`, then sets `SelectedIndex = -1`. |
| `RemoveItemInListBoxAction` | Works with any `Selector`. |
| `SelectListBoxItemOnPointerMovedBehavior` | Selects the nearest ancestor `SelectorItem` (Avalonia needs the direct parent). |
| `SelectingItemsControlSearchBehavior` | Attaches to a `TabView` and filters and sorts its `TabItems` in place (`TabItemsSource` is not supported). Listens to `TextChanged` only. |
| `SelectingItemsControlEventsBehavior` | Attaches to a `Selector` (`TabView` is not a `Selector`). |

### Logic, Popup, ScrollViewer

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `Case` | A `DependencyObject`, not a `StyledElement` (no `Name`, `Classes` or own data context). |
| `SwitchCaseAction.Cases`, `SwitchCaseBehavior.Cases` | Typed `CaseCollection`. |
| `CaseCollection` | Uno-only public type (`DependencyObjectCollection` with `INotifyCollectionChanged`); cases inherit the data context of the switch. |
| `ConditionalBehavior.ElseActions` | Always hosted by the behavior. |
| `PopupOpenedTrigger`, `PopupClosedTrigger` | Attach to the WinUI `Popup`; actions receive an `object` argument. |
| `ScrollChangedTrigger` | WinUI `ScrollViewer.ViewChanged` on the associated `ScrollViewer` only (no bubbling from nested scroll viewers). Actions receive `ScrollViewerViewChangedEventArgs`. `EventRoutingStrategy` and `MarkAsHandled` have no effect. |
| `ViewportBehavior` | Re-evaluates on `ViewChanged`; skips evaluation while the element is not loaded. |
| `ScrollToOffsetAction`, `ScrollViewerOffsetBehavior` | `ChangeView(..., disableAnimation: true)`: the offset is applied by the scroll viewer later, not synchronously. |
| `HorizontalScrollViewerBehavior` | A line step is 16 pixels, a page step `ViewportWidth`. |

### Show, SplitView, System, TabControl

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ShowOnKeyDownBehavior.Gesture` | A WinUI `KeyboardAccelerator` (`<KeyboardAccelerator Key="..." Modifiers="..." />`), not a `KeyGesture` string. Matches the same key with exactly the same modifiers. `Key` is a `VirtualKey`. |
| `ShowOnTappedBehavior`, `ShowOnDoubleTappedBehavior`, `ShowOnKeyDownBehavior` | `Visibility` is set as a local value (replaces a binding). |
| `SplitViewPaneOpeningTrigger` | Actions receive an `object` argument; opening cannot be cancelled. |
| `SplitViewPaneClosingTrigger` | Actions receive `SplitViewPaneClosingEventArgs` (with `Cancel`). |
| `SplitViewPaneOpenedTrigger`, `SplitViewPaneClosedTrigger` | Actions receive an `object` argument. |
| `SplitViewStateBehavior.Setters` | `DependencyObjectCollection<SplitViewStateSetter>`. |
| `ClipboardMonitorBehavior` | Polls the application clipboard; requires a `XamlRoot`. Only `Text` and `Files`/`FileNames` are normalized in `Formats`; other names are WinUI format ids. No injectable clipboard. |
| `TabControlSelectionChangedTrigger` | `TabView.SelectionChanged` on the associated `TabView` only (no bubbling from nested lists). `EventRoutingStrategy` and `MarkAsHandled` have no effect. |
| `TabControlNextAction`, `TabControlPreviousAction`, `TabControlKeyNavigationBehavior` | Work with `TabView` (`TabItemsSource` or `TabItems`). |

### ThemeVariant, ToolTips, Transitions, TreeView

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ThemeVariantBehavior` | Attaches to any `FrameworkElement` (Avalonia: a `ThemeVariantScope`). `ThemeVariant` is `ElementTheme?` (no custom variants); sets `RequestedTheme`. |
| `ThemeVariantTrigger` | `ThemeVariant` is `ElementTheme?`, compared with `ActualTheme`. Evaluates once the element is loaded. |
| `ToolTipOpeningTrigger` | WinUI has no opening event: fires on `ToolTip.Opened`, after the tooltip opened, and cannot cancel it. Subscribes when the element loads to the `ToolTip` set at that time (plain content is wrapped in a new `ToolTip`); a tooltip replaced later is not followed. |
| `ToolTipClosingTrigger` | Fires on `ToolTip.Closed`, after the tooltip closed. Same subscription rules. |
| `ShowToolTipAction`, `HideToolTipAction`, `SetToolTipTipAction` | Use `ToolTipService`. Show wraps plain content in a `ToolTip`; hide only closes a `ToolTip` instance. |
| `AddTransitionAction`, `RemoveTransitionAction`, `ClearTransitionsAction`, `TransitionsBehavior`, `TransitionsChangedTrigger` | WinUI theme transitions (`Transition`, `UIElement.Transitions`), not Avalonia property transitions. |
| `TreeViewFilterBehavior`, `ApplyTreeViewFilterAction` | Filter `TreeView.RootNodes`/`TreeViewNode.Children` by node content and set `IsExpanded` (Avalonia filters logical `TreeViewItem`s by `Header`). Listens to `TextChanged` only. |
| `TreeViewFilterTextChangedTrigger` | `TextChanged` CLR event: `RoutingStrategies` is ignored. |
| `ToggleIsExpandedOnDoubleTappedBehavior` | Toggles the nearest ancestor `TreeViewItem`. |

### Validation

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `PropertyValidationBehavior<TControl, TValue>.Property` | Any `DependencyProperty`; the value is cast to `TValue`. Changes after attaching are observed (for `x:Bind`). Use a closed subclass in XAML. |
| `PropertyValidationBehavior.Rules` | `ObservableCollection<IValidationRule<TValue>>`. Rules inherit no data context: bind them with `x:Bind`. |
| `PropertyValidationBehavior` (errors) | No error template: errors are exposed through `IsValid` and `Error` only. |
| `PropertyValidationBehavior.IsValid` | Needs `Mode=TwoWay`. |
| Custom validation rules | Changing a property of a custom rule does not revalidate; only the built-in rules do. |
| `ValidationRuleBase` | Uno-only public abstract base of the generic rules (`RangeValidationRule<T>`, `MinValueValidationRule<T>`, `MaxValueValidationRule<T>`, `NotNullValidationRule<T>`). Use closed subclasses in XAML. |
| `ComboBoxValidationBehavior` | Validates `Selector.SelectedItem`. |

### Window and WriteableBitmap

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `WindowState` | Uno-only enum (`Normal`, `Minimized`, `Maximized`, `FullScreen`) mapped to the `AppWindow` presenter. Other presenters read as `Normal`. |
| `WindowStateTrigger` | `State` is the Uno enum; changes come from `AppWindow.Changed`. |
| `WindowAction` | Minimize, maximize and restore use the overlapped presenter; full screen sets the full screen presenter. |
| `ShowWindowAction` | Calls `Activate()`. The sender's data context goes to `Window.Content`. The platform chooses the size. |
| `CloseWindowAction`, `WindowAction` | The window is found through `XamlRoot`: a detached element has no window. |
| `WriteableBitmapBehavior`, `WriteableBitmapRenderBehavior` | WinUI `WriteableBitmap`: BGRA8 premultiplied, 96 DPI. The behaviors call `Invalidate()` after rendering. |
| `WriteableBitmapRenderAction`, `IWriteableBitmapRenderer` | The renderer receives the WinUI bitmap and must call `Invalidate()` itself. |
| `RenderTargetBitmapTrigger`, `RenderRenderTargetBitmapAction`, `IRenderTargetBitmapRenderHost` | Available, but no host implementation ships on Uno Platform. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `AddClassAction`, `RemoveClassAction`, `ToggleClassAction` | Avalonia style classes. |
| `ScreenshotAction` | Needs `RenderTargetBitmap` encoding and a `TopLevel` storage provider; not ported ([#397](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/397)). |
| `ResourcesChangedBehavior`, `ResourcesChangedTrigger` | WinUI raises no resources changed notification. |
| `Cursor/**`: `PointerOverCursorBehavior`, `SetCursorAction`, `SetCursorBehavior`, `SetCursorFromProviderAction`, `SetCursorFromProviderBehavior`, `ICursorProvider` | WinUI exposes the element cursor only through the protected `ProtectedCursor`. |
| `VisualDebugBehavior` | Adorner layer. |
| `ExecuteCommandOnPinch*`, `ExecuteCommandOnPullGesture*`, `ExecuteCommandOnScrollGesture*`, `ExecuteCommandOnPointerTouchPadGesture*` behaviors; `Pinch*`, `PullGesture*`, `ScrollGesture*`, `PointerTouchPadGesture*` triggers | Avalonia gesture recognizers without a WinUI routed event. |
| `TextInputMethodClientRequestedTrigger`, `ExecuteCommandOnTextInputMethodClientRequestedBehavior` | Avalonia IME client event. |
| `ItemsControlPreparingContainerTrigger` | `ItemsRepeater` raises no event before a container is prepared. |
| `NotificationManagerBehavior`, `ShowNotificationAction`, `ShowInformationNotificationAction`, `ShowSuccessNotificationAction`, `ShowWarningNotificationAction`, `ShowErrorNotificationAction`, `CloseNotificationAction` | Avalonia notification managers. |
| `RenderTargetBitmapBehavior`, `StaticRenderTargetBitmapBehavior`, `IRenderTargetBitmapRenderer`, `IRenderTargetBitmapSimpleRenderer` | Immediate mode drawing into a bitmap; WinUI `RenderTargetBitmap` only captures elements. |
| `ActiveScreenBehavior`, `RequestScreenDetailsAction`, `ScreensChangedTrigger` | Avalonia Screens; Uno Platform does not implement `DisplayArea`. |
| `CenterWindowBehavior` | Needs the screen working area, which Uno Platform Skia does not provide. |
| `WindowDragMoveBehavior` | `Window.BeginMoveDrag`; WinUI declares drag regions with `Window.SetTitleBar`. |
| `NumericUpDownValidationBehavior` | WinUI has no `NumericUpDown`. |

## Responsive

`Xaml.Behaviors.Uno.Interactions.Responsive`, namespace `Xaml.Interactions.Responsive`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `AdaptiveBehavior`, `AspectRatioBehavior` (`ClassName`, `IsPseudoClass`) | Classes become visual states (see [Style classes](#style-classes-and-visual-states)). The target needs matching visual states. |
| `AdaptiveBehavior`, `AspectRatioBehavior` (source bounds) | `ActualOffset` and the actual size, re-evaluated after layout (`SizeChanged`, `LayoutUpdated`) when they changed. |
| `AdaptiveBehavior.Setters`, `AspectRatioBehavior.Setters` | `DependencyObjectCollection`: setters inherit the data context. |

Nothing is excluded.

## Draggable

`Xaml.Behaviors.Uno.Interactions.Draggable`, namespace `Xaml.Interactions.Draggable`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| All drag behaviors (tunnel pointer handlers) | Bubbling handlers with handled events: they run after the child elements' handlers. |
| `CanvasDragBehavior`, `GridDragBehavior`, `ItemDragBehavior`, `ListReorderDragBehavior` (`:dragging`) | The `dragging`/`Dragging` visual state, then `NotDragging`, `Normal` or `Default`. The template must define the states. |
| `ItemDragBehavior`, `ListReorderDragBehavior` | Can be attached to the container or to any element of the item template: the nearest item container is dragged. |
| `ItemDragBehavior` | Moves with a `TranslateTransform`; updates `SelectedIndex` of a `Selector`. |
| `ListReorderDragBehavior.PlaceholderTemplate` | A `DataTemplate`. |
| `ListReorderDragBehavior` (placeholder) | Shown in a non-interactive `Popup` at the target container (no adorner layer); nothing is shown without a `XamlRoot`. |
| `MultiMouseDragElementBehavior.TargetControls` | `ObservableCollection<FrameworkElement>`. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `SelectionAdorner` | Drawn with `Render(DrawingContext)` for the adorner layer; WinUI has neither. |

## DragAndDrop

`Xaml.Behaviors.Uno.Interactions.DragAndDrop`, namespace `Xaml.Interactions.DragAndDrop`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ContextDragBehavior`, `ContextDragWithDirectionBehavior`, `TypedDragBehavior`, `PanelDragBehavior` | WinUI drag (`StartDragAsync`) from the pressed element (`OriginalSource`), with the WinUI default drag visual. A cancelled drag returns `None`. |
| `ContextDragBehavior`, `PanelDragBehavior` (press) | The press keeps its `Handled` flag (Avalonia resets it to `false`). |
| `IDragHandler`, `ContextDragBehaviorBase` (`BeforeDragDrop`, `AfterDragDrop`) | `e` is a `PointerRoutedEventArgs`. |
| `IDropHandler`, `DropHandlerBase`, `ContextDropBehaviorBase`, `DragAndDropEventsBehavior`, `DragDropCommandsBehavior` | `e` is the WinUI `DragEventArgs`. Use `AcceptedOperation`, `AllowedOperations` and `DataView` in your handlers (the Avalonia-shaped helpers are internal). `DragDropEffects` is `DataPackageOperation`. |
| `ContextDropBehaviorBase.ContextDataTransferFormat` | A `string` format id (same value as on Avalonia). |
| `TextDropBehavior`, `FilesDropBehavior`, `FilesPreviewBehavior`, `AddPreviewFilesAction`, `ContentControlFilesDropBehavior`, `ContextDropBehaviorBase` | Data is read synchronously: data that is not available yet (delay rendered) counts as missing. |
| `FilesDropBehavior`, `ContentControlFilesDropBehavior`, `AddPreviewFilesAction` | Files are `Windows.Storage.IStorageItem`. |
| `FilesPreviewBehavior.PreviewFiles` | `ObservableCollection<Windows.Storage.IStorageItem>`. |
| `ContentControlFilesDropBehavior.BackgroundDuringDrag` | A WinUI `Brush`. `ContentDuringDrag`/`BackgroundDuringDrag` replace bindings while dragging. |
| `TextDropBehavior`, `FilesDropBehavior`, `PanelDropBehavior`, `ContentControlFilesDropBehavior` (enabled check) | Only a `Control` can be disabled. |
| `BaseTreeViewDropHandler` | Targets the WinUI `TreeView` (`Validate(TreeView, ...)`). `DraggingUp`, `DraggingDown` and `TargetHighlight` become `TreeViewItem` visual states. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `ManagedDragDrop/**`: `ManagedContextDragBehavior`, `ManagedContextDropBehavior`, `ManagedContextDropArgs`, `ManagedDragDropService`, `DragPreviewService`, `DragPreviewWindow`, `ManagedDataTransferExtensions` | WinUI `DragEventArgs` cannot be created by user code; the drag preview needs a topmost transparent window and top-level screen coordinates. The WinUI drag and drop is already in-process with a drag visual on Uno Platform (Skia, WebAssembly). |

## DragAndDrop.DataGrid

`Xaml.Behaviors.Uno.Interactions.DragAndDrop.DataGrid`, built on the Windows Community Toolkit `DataGrid`
(`Uno.CommunityToolkit.WinUI.UI.Controls.DataGrid`).

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `BaseDataGridDropHandler<T>` | Targets the toolkit `DataGrid`, `DataGridRow` and `DataGridRowsPresenter`. `DraggingUp`/`DraggingDown` become row visual states. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| `Styles.axaml` (row drop indicators) | Adorner based. Add `DraggingUp`/`DraggingDown` visual states to the row style. |

## Animations

`Xaml.Behaviors.Uno.Animations`, namespace `Xaml.Interactions.Custom`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `AnimationFactory`, `FluidMoveAnimation.Create`, `IAnimationBuilder.Build`, `AnimationRunner` | Build and run WinUI `Storyboard`s (linear `DoubleAnimationUsingKeyFrames`). |
| `AnimationRunner.RunAsync`, `TryRun*`, `TryBuildAndRun*` | Timelines without `TargetName` are retargeted to the target. Running a storyboard again (even on another target) restarts it; pending tasks complete with the restarted run. |
| `EntranceAnimations`, `ExitAnimations`, `AttentionAnimations`, `FramerMotionAnimations`, `SlidingAnimation`, `ParallaxAnimation.Apply` (composition `Offset`) | Offsets are relative to the layout position (WinUI composes `Visual.Offset` on top of it). On Avalonia they include the element's position. |
| `TiltAnimation.Apply`/`Reset`, `OrbitAnimation.Rotate` | Rotate around an axis (`RotationAxis` + animated `RotationAngle`) instead of a quaternion `Orientation` animation; shares `RotationAngle` with the rotation animations. |
| `SelectionIndicatorAnimation.TryStart` | Starts explicit `Translation` and `Scale` key frame animations immediately, with a cubic bezier easing (no implicit animations, no spring easing). |
| `SelectingItemsControlBehavior.EnableSelectionAnimation` | Attaches to a `Selector`; runs on `SelectionChanged` when an item is both added and removed. |
| `TransitionOperations` | Work on WinUI theme transitions (`UIElement.Transitions`). |

Nothing is excluded.

## ReactiveUI

`Xaml.Behaviors.Uno.Interactions.ReactiveUI`, namespace `Xaml.Interactions.ReactiveUI`. Both platforms use the
System.Reactive flavor of ReactiveUI 25.1 (`ReactiveUI.Reactive`).

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `InteractionTriggerBehavior<TInput, TOutput>`, `NavigateToAction<TViewModel>`, `NavigateToAndResetAction<TViewModel>` | Use closed subclasses in XAML (no `x:TypeArguments`). |
| `InteractionTriggerBehavior` | The associated object is a `UIElement`. |

The Uno assembly is strong-name signed like the other Uno assemblies; the Avalonia ReactiveUI assembly is not signed.
Nothing is excluded.

## Scripting

`Xaml.Behaviors.Uno.Interactions.Scripting`, namespace `Xaml.Interactions.Scripting`.

| Behavior / action / trigger | On Uno Platform |
|-----------------------------|-----------------|
| `ExecuteScriptAction` (default imports) | `System`, `System.Collections.Generic`, `System.Linq`, `System.Collections.ObjectModel`, `Microsoft.UI.Xaml`, `Microsoft.UI.Xaml.Controls`, `.Media`, `.Input`, `.Markup` instead of the `Avalonia.*` namespaces. Scripts using Avalonia types do not compile. |
| `ExecuteScriptAction` (runtime) | Needs dynamic code: works on Skia desktop and JIT targets, not on AOT-only targets. `Sender` and `Parameter` globals are unchanged. |

Nothing is excluded.

## SourceGenerators

`Xaml.Behaviors.SourceGenerators` emits WinUI code when only `Microsoft.UI.Xaml.DependencyObject` is referenced, or when
`XamlBehaviorsSourceGeneratorPlatform` is `WinUI` (or `Uno`). See [Uno Platform support](../source-generators/uno-platform.md).

| Generated member | On Uno Platform |
|------------------|-----------------|
| Base types | `Xaml.Interactivity.StyledElementAction`/`StyledElementTrigger`: the project needs `Xaml.Behaviors.Uno.Interactivity`. |
| Properties | `DependencyProperty.Register`; changes go to `OnPropertyChanged(DependencyPropertyChangedEventArgs)`. |
| `LastError` | Registered as `object`; the CLR property stays `Exception?`. |
| `UseDispatcher` | Uses the `DispatcherQueue` of the thread that created the object (then `this.DispatcherQueue`, then the current thread). Without a queue, or when enqueueing fails, the work runs inline. |
| Change property actions (`ExecuteReversibly`, `Revert`) | Local value through the typed setter (replaces a binding); `Revert` restores the captured value. Avalonia uses an animation-priority overlay and restores the latest source value. |
| Property triggers | Observe a `DependencyProperty` (static field or static property) with `RegisterPropertyChangedCallback`, or a CLR property of an `INotifyPropertyChanged` type. The latter evaluates on every `PropertyChanged` for the name (or an empty name), even if the value did not change. |
| Property triggers (`Value`) | Typed `object?` when the generator finds no CLR accessor for the dependency property. |
| `SourceName` (property triggers, event command triggers) | `FrameworkElement.FindName` while walking up `Parent`/`VisualTreeHelper.GetParent`. |
| `IsSet` checks (event command `Parameter`, async and observable trigger overrides) | Only local values (including bindings) count, not style setters. |
| Framework event triggers | Named after the declaring WinUI type: `Button.Click` is declared on `ButtonBase`, so the trigger is `Microsoft.UI.Xaml.Controls.Primitives.ButtonBaseClickTrigger`. |
| Diagnostics | XBG036 replaces XBG011/XBG012. XBG037 (not observable) and XBG038 (owner not a `DependencyObject`) are WinUI only. XBG022 checks for a `FrameworkElement`. XBG019 is still reported for instance fields and for static members that are not a `DependencyProperty`. |

### Not available on Uno Platform

| Feature | Reason |
|---------|--------|
| Generated behaviors in XAML of the same assembly | The Uno XAML generator is a source generator and does not see other generators' output (`UXAML0001`). Generate the types in a separate library. |
| Property triggers on CLR properties of types without `INotifyPropertyChanged` | XBG037. |
| Property triggers on identifiers declared on non-`DependencyObject` types | XBG038. |
| Value priorities for reversible change property actions | WinUI has no value priorities. |

## PropertyGenerator

`Xaml.PropertyGenerator` emits WinUI dependency properties when the type derives from `Microsoft.UI.Xaml.DependencyObject`
(or when `XamlPropertyGeneratorPlatform` is `WinUI` or `Uno`; the override wins over detection). See
`src/Xaml.PropertyGenerator/README.md`.

| Attribute / option | On Uno Platform (WinUI output) |
|--------------------|-------------------------------|
| `[StyledProperty]` | `DependencyProperty.Register` with a `PropertyMetadata` default. |
| `[DirectProperty]` | A `DependencyProperty` kept in sync with the field. A non-public setter does not make the property read-only: `SetValue` and bindings still write it. |
| `[DirectProperty]` with `DefaultValueExpression` | The expression is evaluated for the metadata (shared) and per instance for the field. |
| `[DirectProperty(Lazy = true)]` | The first access creates the value and stores it with `SetValue`, which raises `On{Name}Changed` and `OnPropertyChanged`. |
| `[AttachedProperty]` | `RegisterAttached`. The default `HostType` is `DependencyObject`; a static owner class is allowed. The change hook casts the object to `HostType`. |
| `DefaultBindingMode`, `Inherits`, `ResolveByName`, `AssignBinding` | Ignored (info diagnostic `XPG0005`). |
| `Content` | `[ContentProperty(Name = ...)]` on the class. |
| Change notifications | Order: field sync, `On{Name}Changed`, `OnPropertyChanged(e)`. Attached properties do not route to `OnPropertyChanged`. |
| Trimming | Generic parameters used as property types get `[DynamicallyAccessedMembers]`; `System.Type` properties are registered through a helper with a justified IL2111 suppression. |
| Migration (`XPG1001`) | Only runs on Avalonia projects. |

## Headless testing

`Xaml.Behaviors.Uno.Headless` (session), `.Headless.XUnit` (attributes) and `.Headless.Host` (Skia host) are the Uno
counterparts of `Avalonia.Headless.XUnit`. See `src/Uno/Xaml.Behaviors.Uno.Headless/README.md` and
`tests/Uno/README.md`.

| Avalonia headless | Uno Platform headless |
|-------------------|-----------------------|
| `[AvaloniaFact]`, `[AvaloniaTheory]` | `[UnoHeadlessFact]`, `[UnoHeadlessTheory]` (xUnit v3 only). Construction, `IAsyncLifetime`, the test and disposal run on the UI thread; theory data is enumerated off it. |
| `[AvaloniaTestApplication]` | `UnoHeadlessSessionOptions` (size in raw pixels, `Scale`, `ApplicationFactory`, `StartupTimeout`), started from an xUnit assembly fixture. One session and one window per process. The default application has no `XamlControlsResources`. |
| `new Window { Content = ... }.Show()` | `session.Show(content)`/`ShowAsync`: replaces the window content and starts a new input sequence. |
| `Dispatcher.UIThread.RunJobs()` | `session.RunJobs()` (UI thread only, up to 100 000 items) or `WaitForIdleAsync()`. Work runs in FIFO order without priorities. Exceptions in queued work are logged, not rethrown. |
| `window.KeyPress(...)` | `session.Keyboard` (`VirtualKey`): `KeyDown`/`KeyUp` do not run queued work, `Press` and `TypeText` do. `TypeText` maps a–z, A–Z, 0–9, space, Enter and Tab; other characters raise `CharacterReceived` with `VirtualKey.None`. |
| `window.MouseDown/MouseUp/MouseMove/MouseWheel(...)` | `session.Mouse`: window or element relative positions, 16 ms per event, double taps at the same position; left, right and middle buttons; 120 per wheel notch. Modifiers come from the arguments and the keys held on the keyboard. |
| `CaptureRenderedFrame()` | No frame capture: headless windows produce no pixel output. The shared test compat `CaptureRenderedFrame()` runs the queued work and the layout and returns `null`. |

The Uno host pins `Uno.WinUI` 6.7.135 exactly. Test projects are executables (`OutputType=Exe`).

## See also

* [Overview](index.md)
* [Known Issues and Differences](known-issues.md)
* [Source generators on Uno Platform](../source-generators/uno-platform.md)
* `src/Uno/PORTING.md`: how the sources are shared, and the status of every project.
* `tests/Uno/README.md` and `samples/Uno/README.md`: excluded tests and sample pages, with their reasons.
