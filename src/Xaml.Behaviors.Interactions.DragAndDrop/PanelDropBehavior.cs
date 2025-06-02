// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that allows dropping controls dragged with <see cref="PanelDragBehavior"/>.
/// </summary>
public sealed class PanelDropBehavior : DropBehaviorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PanelDropBehavior"/> class.
    /// </summary>
    public PanelDropBehavior()
    {
        PassEventArgsToCommand = true;
        Handler = new PanelDropHandler(ExecuteCommand);
    }

    private sealed class PanelDropHandler(System.Action<object?> execute) : DropHandlerBase
    {
        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return e.Data.Contains(ContextDropBehavior.DataFormat) && e.Data.Get(ContextDropBehavior.DataFormat) is Control && sender is Panel;
        }

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (sender is not Panel target || e.Data.Get(ContextDropBehavior.DataFormat) is not Control control)
            {
                return false;
            }

            if (control.Parent is Panel source)
            {
                source.Children.Remove(control);
            }

            target.Children.Add(control);

            if (target is Canvas canvas)
            {
                var p = e.GetPosition(canvas);
                Canvas.SetLeft(control, p.X - control.Bounds.Width / 2);
                Canvas.SetTop(control, p.Y - control.Bounds.Height / 2);
            }

            execute(e);
            return true;
        }
    }
}
