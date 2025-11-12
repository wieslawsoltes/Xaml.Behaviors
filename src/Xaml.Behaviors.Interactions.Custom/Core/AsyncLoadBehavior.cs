// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that calls an asynchronous method when the associated control is loaded.
/// </summary>
public class AsyncLoadBehavior : Behavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="MethodName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MethodNameProperty =
        AvaloniaProperty.Register<AsyncLoadBehavior, string?>(nameof(MethodName));

    /// <summary>
    /// Identifies the <see cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetObjectProperty =
        AvaloniaProperty.Register<AsyncLoadBehavior, object?>(nameof(TargetObject));

    /// <summary>
    /// Gets or sets the name of the method to invoke on load.
    /// </summary>
    public string? MethodName
    {
        get => GetValue(MethodNameProperty);
        set => SetValue(MethodNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the object that exposes the method of interest. If null the DataContext is used.
    /// </summary>
    [ResolveByName]
    public object? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null)
        {
            AssociatedObject.AttachedToVisualTree += OnLoaded;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AttachedToVisualTree -= OnLoaded;
        }
        base.OnDetaching();
    }

    private void OnLoaded(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AttachedToVisualTree -= OnLoaded;
        }

        _ = Dispatcher.UIThread.InvokeAsync(async () => await InvokeAsync());
    }

    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Reflection is used to invoke view-model members provided by the application.")]
    private async Task InvokeAsync()
    {
        if (AssociatedObject is null || string.IsNullOrEmpty(MethodName))
        {
            return;
        }

        var target = TargetObject ?? AssociatedObject.DataContext;
        if (target is null)
        {
            return;
        }

        var methodInfo = target.GetType().GetRuntimeMethod(MethodName, System.Type.EmptyTypes);
        if (methodInfo is null)
        {
            return;
        }

        var result = methodInfo.Invoke(target, null);
        if (result is Task task)
        {
            await task;
        }
    }
}
