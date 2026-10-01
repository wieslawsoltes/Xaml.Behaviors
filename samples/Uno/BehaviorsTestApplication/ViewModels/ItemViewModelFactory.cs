// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Xaml.Interactions.Custom;

namespace BehaviorsTestApplication.ViewModels;

/// <summary>
/// Creates a new <see cref="ItemViewModel"/> for every execution of the add/insert item actions.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia <c>&lt;ObjectTemplate&gt;&lt;vm:ItemViewModel&gt;&lt;x:Arguments&gt;…</c>
/// of the samples: WinUI templates only create UI elements and WinUI XAML has no <c>x:Arguments</c>, so the views
/// declare this factory as the <c>ItemFactory</c> of the actions.
/// </remarks>
public sealed class ItemViewModelFactory : IItemFactory
{
    /// <summary>
    /// Gets or sets the value of the created items.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color of the created items.
    /// </summary>
    public string Color { get; set; } = "Black";

    /// <inheritdoc />
    public object? CreateItem() => new ItemViewModel(Value, Color);
}
