// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Root element of a <see cref="Interaction.BehaviorsTemplateProperty"/> template.
/// </summary>
/// <remarks>
/// WinUI templates create elements, so the behaviors of a template are declared as the content of this element; the
/// collection is moved to the templated object and the host is discarded.
/// </remarks>
public sealed partial class BehaviorCollectionHost : FrameworkElement
{
    /// <summary>
    /// Gets the behaviors created by the template.
    /// </summary>
    [DirectProperty(Lazy = true, Content = true)]
    public partial BehaviorCollection Behaviors { get; }
}
