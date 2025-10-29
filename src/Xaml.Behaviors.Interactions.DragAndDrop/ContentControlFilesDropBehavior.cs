// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that handles file drop operations for <see cref="ContentControl"/>.
/// </summary>
public sealed class ContentControlFilesDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// Identifies the <seealso cref="ContentDuringDrag"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ContentDuringDragProperty =
        AvaloniaProperty.Register<ContentControlFilesDropBehavior, object?>(nameof(ContentDuringDrag));

    /// <summary>
    /// Identifies the <seealso cref="BackgroundDuringDrag"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundDuringDragProperty =
        AvaloniaProperty.Register<ContentControlFilesDropBehavior, IBrush?>(nameof(BackgroundDuringDrag));

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentControlFilesDropBehavior"/> class.
    /// </summary>
    public ContentControlFilesDropBehavior()
    {
        PassEventArgsToCommand = true; // we need this to correctly pass the parameter on drop
        Handler = new FilesDropHandler(ExecuteCommand,
            () => ContentDuringDrag,
            () => BackgroundDuringDrag);
    }

    /// <summary>
    /// If sender is ContentControl - this content will be set during drag over
    /// </summary>
    public object? ContentDuringDrag
    {
        get => GetValue(ContentDuringDragProperty);
        set => SetValue(ContentDuringDragProperty, value);
    }

    /// <summary>
    /// If sender is ContentControl - this background will be set during drag over
    /// </summary>
    public IBrush? BackgroundDuringDrag
    {
        get => GetValue(BackgroundDuringDragProperty);
        set => SetValue(BackgroundDuringDragProperty, value);
    }

    private sealed class FilesDropHandler(
        Action<object?> execute,
        Func<object?> getContentDuringDrag,
        Func<IBrush?> getBackgroundDuringDrag) : DropHandlerBase
    {
        public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
        {
            base.Enter(sender, e, sourceContext, targetContext);

            if (e.DragEffects == DragDropEffects.None || sender is not ContentControl contentControl)
            {
                return;
            }

            if (getContentDuringDrag() is { } contentDuringDrag)
            {
                contentControl.SetCurrentValue(ContentControl.ContentProperty, contentDuringDrag);
            }

            if (getBackgroundDuringDrag() is { } backgroundDuringDrag)
            {
                contentControl.SetCurrentValue(TemplatedControl.BackgroundProperty, backgroundDuringDrag);
            }
        }

        public override void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
        {
            base.Drop(sender, e, sourceContext, targetContext);

            ClearDragValues(sender);
        }

        public override bool Validate(object? sender,
            DragEventArgs e,
            object? sourceContext,
            object? targetContext,
            object? state)
        {
            return e.DataTransfer.Contains(DataFormat.File);
        }

        public override bool Execute(object? sender,
            DragEventArgs e,
            object? sourceContext,
            object? targetContext,
            object? state)
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

        public override void Cancel(object? sender, RoutedEventArgs e)
        {
            ClearDragValues(sender);
        }

        private static void ClearDragValues(object? sender)
        {
            if (sender is not ContentControl contentControl)
            {
                return;
            }

            contentControl.ClearValue(ContentControl.ContentProperty);
            contentControl.ClearValue(TemplatedControl.BackgroundProperty);
        }
    }
}
