// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Microsoft.UI.Dispatching;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Threading;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Behavior that calls an asynchronous method when the associated control is loaded.
/// </summary>
public partial class AsyncLoadBehavior : Behavior<Control>
{

    /// <summary>
    /// Gets or sets the name of the method to invoke on load.
    /// </summary>
    [StyledProperty]
    public partial string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the object that exposes the method of interest. If null the DataContext is used.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial object? TargetObject { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null)
        {
#if UNO
            // WinUI raises Loaded when the element joins the live (visual) tree.
            AssociatedObject.Loaded += OnLoaded;
#else
            AssociatedObject.AttachedToVisualTree += OnLoaded;
#endif
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
#if UNO
            AssociatedObject.Loaded -= OnLoaded;
#else
            AssociatedObject.AttachedToVisualTree -= OnLoaded;
#endif
        }
        base.OnDetaching();
    }

#if UNO
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.Loaded -= OnLoaded;
        }
#else
    private void OnLoaded(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AttachedToVisualTree -= OnLoaded;
        }
#endif

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
