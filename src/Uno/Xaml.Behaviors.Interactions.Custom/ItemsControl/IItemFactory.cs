// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
namespace Xaml.Interactions.Custom;

/// <summary>
/// Creates the items added by <see cref="AddItemToItemsControlAction"/> and <see cref="InsertItemToItemsControlAction"/>.
/// </summary>
/// <remarks>
/// Avalonia builds a new item on every execution from an <c>ObjectTemplate</c>. WinUI templates
/// (<c>DataTemplate</c>) only create UI elements and WinUI XAML has no <c>x:Arguments</c>, so on Uno Platform the
/// actions call a factory instead (<c>ItemFactory</c>). Implement it in the view model layer, for example as an object
/// declared in XAML or a view model property bound with <c>x:Bind</c>.
/// </remarks>
public interface IItemFactory
{
    /// <summary>
    /// Creates a new item.
    /// </summary>
    /// <returns>The new item, or <see langword="null"/> to add nothing.</returns>
    object? CreateItem();
}
