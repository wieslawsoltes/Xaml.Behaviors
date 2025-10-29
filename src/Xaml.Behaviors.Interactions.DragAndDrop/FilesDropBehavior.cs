// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that handles file drop operations.
/// </summary>
public sealed class FilesDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilesDropBehavior"/> class.
    /// </summary>
    public FilesDropBehavior()
    {
        PassEventArgsToCommand = true;
        Handler = new FilesDropHandler(ExecuteCommand);
    }

    /// <summary>
    /// Internal handler used to validate and execute file drop operations.
    /// </summary>
    /// <param name="execute">Callback invoked when files are successfully dropped.</param>
    private sealed class FilesDropHandler(System.Action<object?> execute) : DropHandlerBase
    {
        /// <inheritdoc />
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.DataTransfer.Contains(DataFormat.File);
        }

        /// <inheritdoc />
        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (!e.DataTransfer.Contains(DataFormat.File))
            {
                return false;
            }

            var files = e.DataTransfer.TryGetFiles();
            if (files is null)
            {
                return false;
            }

            execute(files);

            return true;
        }
    }
}
