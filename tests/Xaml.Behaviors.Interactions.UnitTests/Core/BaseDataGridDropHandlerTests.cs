using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.UnitTests.Core;
using Avalonia.Headless.XUnit;
using Xunit;
using BehaviorsTestApplication.Behaviors;
using BehaviorsTestApplication.ViewModels;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class BaseDataGridDropHandlerTests
{
    [AvaloniaFact]
    public void FindDataGridRowFromChildView_Returns_Null_For_Unrelated_Control()
    {
        // Arrange: create an element hierarchy simulating a disabled row
        var container = new Border { IsEnabled = false };
        var child = new Button();
        container.Child = child;

        // Use reflection to access the private method
        var handlerType = typeof(BaseDataGridDropHandler<DragItemViewModel>);
        var method = handlerType.GetMethod("FindDataGridRowFromChildView", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Act
        var result = method!.Invoke(null, new object[] { child });

        // Assert
        Assert.Null(result);
    }
}
