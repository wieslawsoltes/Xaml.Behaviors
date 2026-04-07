# ClickEventTrigger

`ClickEventTrigger` emulates Avalonia `Button` click semantics on any `Control`.

It supports:

* Pointer click handling with `ClickMode` (`Release` / `Press`)
* Keyboard activation (`Enter`, `Space`)
* Configurable input subscription routing via `RoutingStrategies`
* Optional `IsDefault` and `IsCancel` root key handling
* Optional flyout toggle behavior
* Optional exact `KeyModifiers` filtering
* Optional handled-event subscription with `HandledEventsToo`

> Access-key internals are intentionally not emulated. This trigger uses only public Avalonia APIs.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `SourceControl` | `Control?` | Optional external source control (resolved by name). If set, click handling and execution use that control instead of the associated object. |
| `ClickMode` | `ClickMode` | Determines whether the click fires on press or release. Default is `Release`. |
| `RoutingStrategies` | `RoutingStrategies` | Routed-event strategies used for pointer/keyboard subscriptions. Default is `Tunnel`. |
| `KeyModifiers` | `KeyModifiers?` | Optional exact key-modifier filter. `null` means no filter. |
| `IsDefault` | `bool` | If `true`, Enter on the visual root can trigger click when the control is visible and enabled. |
| `IsCancel` | `bool` | If `true`, Escape on the visual root can trigger click when the control is visible and enabled. |
| `Flyout` | `FlyoutBase?` | Optional explicit flyout to toggle on click. |
| `UseAttachedFlyout` | `bool` | If `true`, uses `FlyoutBase.AttachedFlyout` when `Flyout` is not set. Default is `true`. |
| `HandleEvent` | `bool` | If `true`, marks handled at button-like decision points. Default is `true`. |
| `HandledEventsToo` | `bool` | If `true`, the trigger listens to routed events even when they were already marked handled by earlier handlers. Default is `false`. |

If `SourceControl` is not set, `ClickEventTrigger` uses the associated object.

## Event Handling Strategy (`HandleEvent` vs `HandledEventsToo`)

Use these two properties together depending on the behavior you need:

| Goal | `HandleEvent` | `HandledEventsToo` | Why |
| --- | --- | --- | --- |
| Button-like behavior on generic control | `true` (default) | `false` (default) | Trigger consumes the click semantics and avoids duplicate input handling. |
| Keep native control behavior (Button/TextBox/etc.) and still run actions | `false` | Usually `true` | Native control logic is not suppressed, and trigger can still react if another handler marks input as handled first. |
| Only react to unhandled routed events | any | `false` | Trigger ignores already-handled events and participates in normal bubbling only. |

`SourceControl` and `HandledEventsToo` are safe to change at runtime. The trigger reattaches its handlers automatically when either value changes.

`RoutingStrategies` is also safe to change at runtime. The trigger reattaches pointer/keyboard handlers when the value changes.

## RoutingStrategies Deep Dive

`ClickEventTrigger` listens to pointer/key routed events and then executes its click semantics.
`RoutingStrategies` controls *where in the routed-event pipeline* those handlers run.

### Why the default is `Tunnel`

Defaulting to `Tunnel` makes `ClickEventTrigger` deterministic for command/picker workflows on controls such as `Button`:

* Tunnel handlers run before bubble handlers.
* With `HandleEvent="True"` (default), the trigger can consume the event first.
* This avoids native control command paths running before picker cancel checks.

### Choosing a strategy

| Goal | `RoutingStrategies` | Typical companion settings | Notes |
| --- | --- | --- | --- |
| Intercept first and keep trigger authoritative | `Tunnel` (default) | `HandleEvent="True"` | Best for picker/action-driven click behavior where native command side-effects should not run first. |
| Let native control behavior run first, then observe | `Bubble` | `HandleEvent="False"` + `HandledEventsToo="True"` | Useful for telemetry, mirrored actions, or secondary command paths. |
| Observe both routed phases | `Tunnel,Bubble` | Depends on scenario | Useful for complex trees/source controls. Keep trigger actions idempotent, because routes can pass through both phases. |
| Subscribe only to direct stage | `Direct` | Advanced usage | Only use when you specifically need direct routing semantics. |

### Example: Default `Tunnel` for picker/command safety

```xml
<Button Content="Save"
        Command="{Binding NativeButtonCommand}">
    <Interaction.Behaviors>
        <ClickEventTrigger RoutingStrategies="Tunnel">
            <SaveFilePickerAction Command="{Binding SaveFileCommand}"
                                  Title="Save Document" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Button>
```

Use this when the trigger action should own click processing and native command behavior should not run first.

### Example: `Bubble` to preserve native behavior

```xml
<Button Content="Run native + analytics"
        Command="{Binding NativeButtonCommand}">
    <Interaction.Behaviors>
        <ClickEventTrigger RoutingStrategies="Bubble"
                           HandleEvent="False"
                           HandledEventsToo="True">
            <InvokeCommandAction Command="{Binding AnalyticsCommand}" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Button>
```

Use this when native control behavior is primary and trigger actions are additive.

### Example: Combined flags (`Tunnel,Bubble`) with external source

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBox Name="SearchBox" Width="260" Text="{Binding Query}" />

    <Border Width="260"
            Height="64"
            Background="LightSteelBlue"
            Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger SourceControl="SearchBox"
                               RoutingStrategies="Tunnel,Bubble"
                               HandleEvent="False"
                               HandledEventsToo="True">
                <InvokeCommandAction Command="{Binding SearchBoxClickCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
    </Border>
</StackPanel>
```

Use this when source events may be observed in different routed phases and you want maximum compatibility.

### Pointer Capture Guidance

`ClickEventTrigger` now avoids pointer capture for `TextBox` sources and for non-invasive mode (`HandleEvent="False"`).

Use this guidance:

* If your source is editable (for example `TextBox`) and you need native drag-selection/caret behavior, set `HandleEvent="False"`.
* If upstream handlers may mark events handled and you still want trigger actions, add `HandledEventsToo="True"`.
* For non-editable controls, keep defaults unless you explicitly need native behavior to win.

This prevents the trigger from stealing pointer ownership in text-editing scenarios while preserving action execution.

## Example

```xml
<Border Background="LightSteelBlue" Width="260" Height="70" Focusable="True">
    <Interaction.Behaviors>
        <ClickEventTrigger ClickMode="Release"
                           IsDefault="True"
                           KeyModifiers="{x:Null}">
            <InvokeCommandAction Command="{Binding SubmitCommand}" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Border>
```

## SourceControl Example

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <Border Name="SourceControlSourceTarget"
            Width="300"
            Height="72"
            Background="LightCyan"
            Focusable="True" />

    <Border Width="300"
            Height="72"
            Background="PowderBlue"
            Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger SourceControl="SourceControlSourceTarget">
                <InvokeCommandAction Command="{Binding SourceControlClickCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Text="Behavior host (source is external)" />
    </Border>
</StackPanel>
```

## SourceControl + `HandledEventsToo` with a Button source

Use this when a `Button` should keep its native click behavior while a separate host still runs trigger actions.

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <Button Name="ActionButton"
            Width="280"
            Content="Native Button source">
        <Interaction.Behaviors>
            <ClickEventTrigger HandleEvent="False" HandledEventsToo="True">
                <InvokeCommandAction Command="{Binding SourceButtonCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
    </Button>

    <Border Width="280"
            Height="64"
            Background="LightSkyBlue"
            Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger SourceControl="ActionButton" HandledEventsToo="True">
                <InvokeCommandAction Command="{Binding MirroredActionCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Text="Host target (Button source)" />
    </Border>
</StackPanel>
```

## `TextBox` Example (preserve text input behavior)

Use `HandleEvent="False"` so editing behavior remains intact, and `HandledEventsToo="True"` when you still want the trigger to observe input that may already be marked handled.

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBox Name="SearchBox"
             Width="280"
             Text="{Binding Query, Mode=TwoWay}">
        <Interaction.Behaviors>
            <ClickEventTrigger HandleEvent="False" HandledEventsToo="True">
                <InvokeCommandAction Command="{Binding TextBoxTriggerCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
    </TextBox>

    <Border Width="280"
            Height="64"
            Background="CadetBlue"
            Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger SourceControl="SearchBox" HandledEventsToo="True">
                <InvokeCommandAction Command="{Binding SourceTextBoxCommand}" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Text="Host target (TextBox source)" />
    </Border>
</StackPanel>
```

## Open/Save Picker + MVVM Commands

`ClickEventTrigger` can execute storage-provider picker actions on non-button controls while keeping command handling in the ViewModel.

```xml
<StackPanel Spacing="8">
    <Border Width="300" Height="72" Background="LightGreen" Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger>
                <OpenFilePickerAction Command="{Binding OpenFilesCommand}"
                                      Title="Open Files from ClickEventTrigger"
                                      SuggestedFileName="TriggerSample"
                                      AllowMultiple="True"
                                      FileTypeFilter="Text Files|*.txt|Markdown Files|*.md|All Files|*.*" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Text="Open Files (ClickEventTrigger)" />
    </Border>

    <Border Width="300" Height="72" Background="LightSalmon" Focusable="True">
        <Interaction.Behaviors>
            <ClickEventTrigger>
                <SaveFilePickerAction Command="{Binding SaveFileCommand}"
                                      InputConverter="{x:Static StorageItemToPathConverter.Instance}"
                                      Title="Save File from ClickEventTrigger"
                                      SuggestedFileName="TriggerSample"
                                      DefaultExtension="txt"
                                      FileTypeChoices="Text Files (*.txt)|*.txt|Markdown Files (*.md)|*.md|All Files (*.*)|*.*" />
            </ClickEventTrigger>
        </Interaction.Behaviors>
        <TextBlock HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Text="Save File (ClickEventTrigger)" />
    </Border>

    <ListBox Height="140" ItemsSource="{Binding FileItems}" />
</StackPanel>
```

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using ReactiveUI;

public ICommand OpenFilesCommand { get; }
public ICommand SaveFileCommand { get; }
public ObservableCollection<Uri> FileItems { get; } = new();

public MainWindowViewModel()
{
    OpenFilesCommand = ReactiveCommand.Create<IEnumerable<IStorageItem>>(files =>
    {
        foreach (IStorageItem file in files)
        {
            if (file.Path is { } path)
            {
                FileItems.Add(path);
            }
        }
    });

    SaveFileCommand = ReactiveCommand.Create<Uri>(file =>
    {
        FileItems.Add(file);
    });
}
```
