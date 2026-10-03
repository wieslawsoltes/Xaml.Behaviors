using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Data;
using Xaml.PropertyGenerator;

namespace Xaml.PropertyGenerator.UnitTests;

public enum RuntimeMode
{
    First,
    Second = 4,
}

public partial class RuntimeOwner : AvaloniaObject
{
    public List<(string? Old, string? New)> TextChanges { get; } = [];

    [StyledProperty(DefaultValue = true)]
    public partial bool IsActive { get; set; }

    [StyledProperty(DefaultValue = RuntimeMode.Second, DefaultBindingMode = PropertyBindingMode.TwoWay)]
    public partial RuntimeMode Mode { get; set; }

    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(500)")]
    public partial TimeSpan Delay { get; set; }

    [StyledProperty]
    public partial string? Text { get; set; }

    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecute { get; private set; }

    [DirectProperty]
    public partial int Counter { get; set; }

    [DirectProperty(Lazy = true)]
    public partial List<int> Items { get; }

    public void SetCanExecute(bool value) => CanExecute = value;

    partial void OnTextChanged(string? oldValue, string? newValue) => TextChanges.Add((oldValue, newValue));
}

[AttachedProperty("Tag", typeof(string), IsNullable = true, DefaultValue = "none")]
public partial class RuntimeAttachments
{
    public static int ChangeCount { get; set; }

    static partial void OnTagChanged(AvaloniaObject element, string? oldValue, string? newValue) => ChangeCount++;
}
