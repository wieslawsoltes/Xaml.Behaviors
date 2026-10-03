// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A trigger that executes its actions when <see cref="Trigger"/> is called, passing a <see cref="WriteableBitmap"/> as parameter.
/// </summary>
public partial class WriteableBitmapTrigger : StyledElementTrigger
{

    /// <summary>
    /// Gets or sets the bitmap passed to actions. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial WriteableBitmap? Bitmap { get; set; }

    /// <summary>
    /// Manually invokes the trigger.
    /// </summary>
    public void Trigger()
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, Bitmap);
    }
}
