using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// 
/// </summary>
public sealed class FilesDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    public FilesDropBehavior()
    {
        Handler = new FilesDropHandler(ExecuteCommand);
    }

    private sealed class FilesDropHandler(System.Action<object?> execute) : DropHandlerBase
    {
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains(DataFormats.FileNames);
        }

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (!e.Data.Contains(DataFormats.FileNames))
            {
                return false;
            }

            var files = e.Data.GetFiles();
            if (files is null)
            {
                return false;
            }

            execute(files);

            return true;
        }
    }
}
