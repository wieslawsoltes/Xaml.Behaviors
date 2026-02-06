# ClickEventTrigger

`ClickEventTrigger` emulates Avalonia `Button` click semantics on any `Control`.

It supports:

* Pointer click handling with `ClickMode` (`Release` / `Press`)
* Keyboard activation (`Enter`, `Space`)
* Optional `IsDefault` and `IsCancel` root key handling
* Optional flyout toggle behavior
* Optional exact `KeyModifiers` filtering

> Access-key internals are intentionally not emulated. This trigger uses only public Avalonia APIs.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `ClickMode` | `ClickMode` | Determines whether the click fires on press or release. Default is `Release`. |
| `KeyModifiers` | `KeyModifiers?` | Optional exact key-modifier filter. `null` means no filter. |
| `IsDefault` | `bool` | If `true`, Enter on the visual root can trigger click when the control is visible and enabled. |
| `IsCancel` | `bool` | If `true`, Escape on the visual root can trigger click when the control is visible and enabled. |
| `Flyout` | `FlyoutBase?` | Optional explicit flyout to toggle on click. |
| `UseAttachedFlyout` | `bool` | If `true`, uses `FlyoutBase.AttachedFlyout` when `Flyout` is not set. Default is `true`. |
| `HandleEvent` | `bool` | If `true`, marks handled at button-like decision points. Default is `true`. |

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
