// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
#else
using Avalonia.Controls;
using Avalonia.Threading;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// An action that requests extended screen information from <see cref="Screens"/>.
/// </summary>
public partial class RequestScreenDetailsAction : Avalonia.Xaml.Interactivity.StyledElementAction
{

    /// <summary>
    /// Gets or sets the <see cref="Screens"/> instance used to request screen details. This is an avalonia property.
    /// If not set, the screens of the associated <see cref="TopLevel"/> will be used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Screens? Screens { get; set; }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (sender is not Visual visual)
        {
            return false;
        }

        Dispatcher.UIThread.InvokeAsync(async () => await RequestAsync(visual));

        return true;
    }

    private async Task RequestAsync(Visual visual)
    {
        if (IsEnabled != true)
        {
            return;
        }

        try
        {
            var screens = Screens ?? TopLevel.GetTopLevel(visual)?.Screens;
            if (screens is not null)
            {
                await screens.RequestScreenDetails();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
