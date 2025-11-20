# File Drag & Drop

The package provides specialized behaviors for handling file drops from the operating system (e.g., dragging a file from Windows Explorer or Finder into your app).

## FilesDropBehavior

`FilesDropBehavior` simplifies the process of accepting files. It automatically validates that the dragged data contains files and executes a command when they are dropped.

### Properties

*   **`Command`**: The `ICommand` to execute when files are dropped. The command parameter will be an array of file path strings (`string[]`) or `IEnumerable<IStorageItem>` depending on the platform/version.

### Usage

```xml
<Border Background="Transparent" Height="200" Width="200">
    <Interaction.Behaviors>
        <FilesDropBehavior Command="{Binding DropFilesCommand}" />
    </Interaction.Behaviors>
    <TextBlock Text="Drop files here" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```

### ViewModel

```csharp
public ICommand DropFilesCommand { get; }

public MyViewModel()
{
    DropFilesCommand = ReactiveCommand.Create<IEnumerable<string>>(files =>
    {
        foreach (var file in files)
        {
            Console.WriteLine($"Dropped: {file}");
        }
    });
}
```

## FilesPreviewBehavior

`FilesPreviewBehavior` is used to provide visual feedback when files are dragged over a control. It can be used in conjunction with `AddPreviewFilesAction`.

## AddPreviewFilesAction

This action is designed to work with `FilesPreviewBehavior` to display a preview of the files being dragged.

```xml
<ListBox>
    <Interaction.Behaviors>
        <FilesPreviewBehavior>
            <AddPreviewFilesAction />
        </FilesPreviewBehavior>
    </Interaction.Behaviors>
</ListBox>
```
