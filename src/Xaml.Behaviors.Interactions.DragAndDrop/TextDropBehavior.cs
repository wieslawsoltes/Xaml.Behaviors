// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
            return e.DataTransfer.Contains(DataFormat.Text);
        }

        /// <inheritdoc />
        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (!e.DataTransfer.Contains(DataFormat.Text))
            {
                return false;
            }

            var text = e.DataTransfer.TryGetText();
            if (text is null)
            {
                return false;
            }

            execute(text);

            return true;
        }
    }
}
