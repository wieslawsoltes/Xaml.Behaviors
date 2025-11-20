# UploadFileBehaviorBase

`UploadFileBehaviorBase` is an abstract base class for behaviors that upload a file to a URL. It handles the file upload process and executes a command upon completion.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| FilePath | `string` | Gets or sets the path of the file to upload. |
| Url | `string` | Gets or sets the destination URL. |
| Command | `ICommand` | Gets or sets the command to execute after the upload completes. The command parameter will be the `HttpResponseMessage`. |

## Usage

This is a base class. To use it, create a derived class that triggers the `Execute` method (e.g., in response to an event).

```csharp
public class MyUploadBehavior : UploadFileBehaviorBase
{
    // Implement logic to trigger the upload, e.g., on a button click
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is Button button)
        {
            button.Click += Button_Click;
        }
    }

    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is Button button)
        {
            button.Click -= Button_Click;
        }
        base.OnDetachedFromVisualTree();
    }

    private async void Button_Click(object? sender, RoutedEventArgs e)
    {
        await Execute(sender, e);
    }
}
```
