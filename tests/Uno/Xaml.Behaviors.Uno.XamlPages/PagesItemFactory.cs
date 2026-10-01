// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xaml.Interactions.Custom;

namespace Xaml.Behaviors.Uno.XamlPages;

/// <summary>
/// An item created by <see cref="PagesItemFactory"/>.
/// </summary>
/// <param name="Text">The item text.</param>
public sealed record PagesItem(string Text);

/// <summary>
/// Creates a new <see cref="PagesItem"/> on every call; declared in XAML as the item factory of the item actions.
/// </summary>
public sealed class PagesItemFactory : IItemFactory
{
    /// <summary>
    /// Gets or sets the text of the created items.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc />
    public object? CreateItem() => new PagesItem(Text);
}
