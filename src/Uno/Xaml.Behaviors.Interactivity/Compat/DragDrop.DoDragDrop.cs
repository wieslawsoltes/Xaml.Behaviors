// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace Xaml.Interactivity;

/// <content>
/// Drag operations (Avalonia <c>DragDrop.DoDragDropAsync</c>, WinUI <see cref="UIElement.StartDragAsync"/>).
/// </content>
internal static partial class DragDrop
{
    /// <summary>
    /// Starts a drag and drop operation from the element that raised <paramref name="triggerEvent"/>.
    /// </summary>
    /// <remarks>
    /// The data is written to the <see cref="DataPackage"/> in <see cref="UIElement.DragStarting"/>. WinUI shows the
    /// default drag visual (a snapshot of the source element).
    /// </remarks>
    /// <param name="triggerEvent">The pointer event that initiated the drag (usually the pressed event).</param>
    /// <param name="data">The dragged data.</param>
    /// <param name="allowedEffects">The allowed operations.</param>
    /// <returns>The operation accepted by the drop target, or <see cref="DataPackageOperation.None"/>.</returns>
    public static Task<DataPackageOperation> DoDragDropAsync(PointerRoutedEventArgs triggerEvent, DataTransfer data, DataPackageOperation allowedEffects)
        => triggerEvent.OriginalSource is UIElement source
            ? DoDragDropAsync(source, triggerEvent, data, allowedEffects)
            : Task.FromResult(DataPackageOperation.None);

    /// <summary>
    /// Starts a drag and drop operation from <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The drag source.</param>
    /// <param name="triggerEvent">The pointer event that initiated the drag.</param>
    /// <param name="data">The dragged data.</param>
    /// <param name="allowedEffects">The allowed operations.</param>
    /// <returns>The operation accepted by the drop target, or <see cref="DataPackageOperation.None"/>.</returns>
    public static async Task<DataPackageOperation> DoDragDropAsync(UIElement source, PointerRoutedEventArgs triggerEvent, DataTransfer data, DataPackageOperation allowedEffects)
    {
        void OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            data.ApplyTo(args.Data);
            args.Data.RequestedOperation = allowedEffects;
            args.AllowedOperations = allowedEffects;
        }

        source.DragStarting += OnDragStarting;
        try
        {
            return await source.StartDragAsync(triggerEvent.GetCurrentPoint(source)).AsTask().ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return DataPackageOperation.None;
        }
        finally
        {
            source.DragStarting -= OnDragStarting;
        }
    }
}
