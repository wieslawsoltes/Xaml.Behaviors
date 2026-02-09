# ClickEventTrigger

`ClickEventTrigger` emulates Avalonia `Button` click semantics on any `Control`.

It supports:

* Pointer click handling with `ClickMode` (`Release` / `Press`)
* Keyboard activation (`Enter`, `Space`)
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
