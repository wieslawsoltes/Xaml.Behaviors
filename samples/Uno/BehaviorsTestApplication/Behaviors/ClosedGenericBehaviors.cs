// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using BehaviorsTestApplication.ViewModels;
using Xaml.Interactions.Custom;
using Xaml.Interactions.ReactiveUI;

namespace BehaviorsTestApplication.Behaviors;

// WinUI XAML has no x:TypeArguments: the generic behaviors and actions used by the Avalonia sample with
// x:TypeArguments are closed here so the Uno views can use them.

/// <summary>
/// <see cref="ObservableTriggerBehavior{T}"/> for <see cref="int"/> values (Avalonia: <c>x:TypeArguments="x:Int32"</c>).
/// </summary>
public partial class Int32ObservableTriggerBehavior : ObservableTriggerBehavior<int>
{
}

/// <summary>
/// <see cref="NavigateToAction{TViewModel}"/> for <see cref="DetailPageViewModel"/>
/// (Avalonia: <c>x:TypeArguments="vm:DetailPageViewModel"</c>).
/// </summary>
public partial class NavigateToDetailPageAction : NavigateToAction<DetailPageViewModel>
{
}

/// <summary>
/// <see cref="NavigateToAndResetAction{TViewModel}"/> for <see cref="HomePageViewModel"/>
/// (Avalonia: <c>x:TypeArguments="vm:HomePageViewModel"</c>).
/// </summary>
public partial class NavigateToHomePageAndResetAction : NavigateToAndResetAction<HomePageViewModel>
{
}

/// <summary>
/// <see cref="InteractionTriggerBehavior{TInput, TOutput}"/> for interactions without input and output (Avalonia:
/// <c>x:TypeArguments="reactive:Unit, reactive:Unit"</c>).
/// </summary>
public partial class UnitInteractionTriggerBehavior : InteractionTriggerBehavior<System.Reactive.Unit, System.Reactive.Unit>
{
}

/// <summary>
/// <see cref="NotNullValidationRule{T}"/> for <see cref="object"/> values (Avalonia: <c>x:TypeArguments="system:Object"</c>).
/// </summary>
public partial class ObjectNotNullValidationRule : NotNullValidationRule<object>
{
}

/// <summary>
/// <see cref="PropertyValidationBehavior{TControl, TValue}"/> for the <see cref="Microsoft.UI.Xaml.Controls.TextBox.Text"/>
/// property (Avalonia: <c>x:TypeArguments="TextBox, sys:String"</c> and <c>Property="{x:Static TextBox.TextProperty}"</c>).
/// </summary>
/// <remarks>
/// The behavior subscribes to <see cref="PropertyValidationBehavior{TControl, TValue}.Property"/> when it is attached,
/// which happens while the XAML is parsed, before <c>x:Bind</c> assigns its values (WinUI XAML has no
/// <c>x:Static</c>): the validated property is therefore set here.
/// </remarks>
public partial class TextBoxTextPropertyValidationBehavior : PropertyValidationBehavior<Microsoft.UI.Xaml.Controls.TextBox, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxTextPropertyValidationBehavior"/> class.
    /// </summary>
    public TextBoxTextPropertyValidationBehavior()
    {
        Property = Microsoft.UI.Xaml.Controls.TextBox.TextProperty;
    }
}

/// <summary>
/// <see cref="RangeValidationRule{T}"/> for nullable <see cref="System.DateTimeOffset"/> values
/// (Avalonia: <c>x:TypeArguments="system:Nullable(system:DateTimeOffset)"</c>, DatePickerValidationBehaviorView).
/// </summary>
public partial class NullableDateTimeOffsetRangeValidationRule : RangeValidationRule<System.DateTimeOffset?>
{
}
