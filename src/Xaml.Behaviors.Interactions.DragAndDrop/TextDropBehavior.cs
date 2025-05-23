using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that handles text drop operations.
/// </summary>
public sealed class TextDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextDropBehavior"/> class.
    /// </summary>
    public TextDropBehavior()
    {
        PassEventArgsToCommand = true;
        Handler = new TextDropHandler(ExecuteCommand);
    }

    /// <summary>
    /// Internal handler used to validate and execute text drop operations.
    /// </summary>
    /// <param name="execute">Callback invoked when text is successfully dropped.</param>
    private sealed class TextDropHandler(System.Action<object?> execute) : DropHandlerBase
    {
        /// <inheritdoc />
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains(DataFormats.Text);
        }

        /// <inheritdoc />
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
