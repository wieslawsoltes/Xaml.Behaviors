using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// 
/// </summary>
public sealed class TextDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    public TextDropBehavior()
    {
        Handler = new TextDropHandler(ExecuteCommand);
    }

    private sealed class TextDropHandler(System.Action<object?> execute) : DropHandlerBase
    {
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains(DataFormats.Text);
        }

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (!e.Data.Contains(DataFormats.Text))
            {
                return false;
            }

            var text = e.Data.GetText();
            if (text is null)
            {
                return false;
            }

            execute(text);

            return true;
        }
    }
}
