// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Sets <see cref="AutomationProperties.NameProperty"/> on the associated control when attached.
/// </summary>
public partial class AutomationNameBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Gets or sets the automation name. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial string? AutomationName { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateName();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        SetAutomationPropertiesName(null);
        base.OnDetaching();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AutomationNameProperty)
        {
            UpdateName();
        }
    }

    private void UpdateName()
    {
        SetAutomationPropertiesName(AutomationName);
    }

    private void SetAutomationPropertiesName(string? value)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.SetValue(AutomationProperties.NameProperty, value);
    }
}
