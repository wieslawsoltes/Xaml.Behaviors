// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// An action that plays a system beep.
/// </summary>
public class ConsoleBeepAction : StyledElementAction
{
    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        try
        {
            // Basic beep as a fallback since Avalonia has no audio API yet.
            Console.Beep();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
