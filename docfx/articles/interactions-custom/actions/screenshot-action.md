# ScreenshotAction

An action that captures a screenshot of a control and saves it to a file.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| TargetControl | Control | Gets or sets the target control to capture. If null, the associated object is used. |
| FileName | string | Gets or sets the suggested file name for the screenshot. |
| StorageProvider | IStorageProvider | Gets or sets the storage provider used to pick the screenshot file. If null, the top level's storage provider is used (on Uno Platform, `SystemStorageProvider`). |

## Usage

```xml
<Button Content="Take Screenshot">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ScreenshotAction FileName="capture.png" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
