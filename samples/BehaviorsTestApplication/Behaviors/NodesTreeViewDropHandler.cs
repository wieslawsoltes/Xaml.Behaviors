using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public class NodesTreeViewDropHandler : BaseTreeViewDropHandler
{
    protected override (bool Valid, bool WillSourceItemBeMovedToDifferentParent) Validate(TreeView treeView, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        if (sourceContext is not DragNodeViewModel sourceNode
            || targetContext is not DragAndDropSampleViewModel vm
            || treeView.GetVisualAt(e.GetPosition(treeView)) is not Control targetControl
            || targetControl.DataContext is not DragNodeViewModel targetNode
            || sourceNode == targetNode
            || targetNode.IsDescendantOf(sourceNode) // block moving parent to inside child
            || vm.HasMultipleTreeNodesSelected)
        {
            // moving multiple items is disabled because 
            // when an item is clicked to be dragged (whilst pressing Ctrl),
            // it becomes unselected and won't be considered for movement.
            // TODO: find how to fix that.
            return (false, false);
        }

        var sourceParent = sourceNode.Parent;
        var targetParent = targetNode.Parent;
        var sourceNodes = sourceParent is not null ? sourceParent.Nodes : vm.Nodes;
        var targetNodes = targetParent is not null ? targetParent.Nodes : vm.Nodes;
        var areSourceNodesDifferentThanTargetNodes = sourceNodes != targetNodes;

        if (sourceNodes is not null && targetNodes is not null)
        {
            var sourceIndex = sourceNodes.IndexOf(sourceNode);
            var targetIndex = targetNodes.IndexOf(targetNode);

            if (sourceIndex < 0 || targetIndex < 0)
            {
                return (false, false);
            }

            var insertIndex = targetIndex;

            if (e.Source is Control c)
            {
                var treeViewItem = c.FindLogicalAncestorOfType<TreeViewItem>();
                if (treeViewItem is not null && e.GetPosition(treeViewItem).Y > treeViewItem.Bounds.Height / 2)
                {
                    insertIndex = targetIndex + 1;
                }
            }

            var adjustedTargetIndex = insertIndex;
            if (sourceParent == targetParent && adjustedTargetIndex > sourceIndex)
            {
                adjustedTargetIndex--;
            }

            switch (e.DragEffects)
            {
                case DragDropEffects.Copy:
                    {
                        if (execute)
                        {
                            var clone = new DragNodeViewModel() { Title = sourceNode.Title + "_copy" };
                            InsertItem(targetNodes, clone, insertIndex);
                        }

                        return (true, areSourceNodesDifferentThanTargetNodes);
                    }
                case DragDropEffects.Move:
                    {
                        if (execute)
                        {
                            if (sourceNodes == targetNodes)
                            {
                                MoveItem(sourceNodes, sourceIndex, adjustedTargetIndex);
                            }
                            else
                            {
                                sourceNode.Parent = targetParent;
                                sourceNodes.RemoveAt(sourceIndex);
                                var insertionIndex = insertIndex;
                                if (insertionIndex > targetNodes.Count)
                                {
                                    insertionIndex = targetNodes.Count;
                                }
                                targetNodes.Insert(insertionIndex, sourceNode);
                            }
                        }

                        return (true, areSourceNodesDifferentThanTargetNodes);
                    }
                case DragDropEffects.Link:
                    {
                        if (execute)
                        {
                            if (sourceNodes == targetNodes)
                            {
                                SwapItem(sourceNodes, sourceIndex, adjustedTargetIndex);
                            }
                            else
                            {
                                sourceNode.Parent = targetParent;
                                targetNode.Parent = sourceParent;

                                SwapItem(sourceNodes, targetNodes, sourceIndex, targetIndex);
                            }
                        }

                        return (true, areSourceNodesDifferentThanTargetNodes);
                    }
            }
        }

        return (false, false);
    }
}
