using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class DragServiceTests
{
    [Fact]
    public void CreateDataObject_Sets_Context_Value()
    {
        var dataObject = DragService.CreateDataObject("value");

        Assert.True(dataObject.Contains(ContextDropBehaviorBase.DataFormat));
        Assert.Equal("value", dataObject.Get(ContextDropBehaviorBase.DataFormat));
    }

    [Fact]
    public void CreateDataObject_Adds_Extras()
    {
        var dataObject = DragService.CreateDataObject("value", ("direction", "down"));

        Assert.Equal("value", dataObject.Get(ContextDropBehaviorBase.DataFormat));
        Assert.Equal("down", dataObject.Get("direction"));
    }
}

