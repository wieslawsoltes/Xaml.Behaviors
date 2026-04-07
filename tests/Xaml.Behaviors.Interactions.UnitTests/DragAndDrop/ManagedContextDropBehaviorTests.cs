using System.Linq;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.DragAndDrop;

public class ManagedContextDropBehaviorTests
{
    [Fact]
    public void ManagedDataTransferExtensions_Return_Original_Payload()
    {
        var payload = new ManagedPayload("payload");
        IDataTransfer dataTransfer = ManagedPayloadDataTransfer.Create("Managed.Payload", payload);

        Assert.Same(payload, dataTransfer.TryGetManagedValue<ManagedPayload>("Managed.Payload"));
        Assert.Same(payload, dataTransfer.Items.Single().TryGetManagedValue<ManagedPayload>("Managed.Payload"));
    }

    [Fact]
    public void ManagedDataTransferExtensions_Keep_Text_Payload_Compatible_With_Avalonia_Text_APIs()
    {
        IDataTransfer dataTransfer = ManagedPayloadDataTransfer.Create("Managed.Payload", "payload");

        Assert.Equal("payload", dataTransfer.TryGetText());
        Assert.Equal("payload", dataTransfer.TryGetValue(DataFormat.CreateStringApplicationFormat("Managed.Payload")));
    }

    [Fact]
    public void ManagedContextDropBehavior_CreateDataTransfer_Preserves_Payload_In_DragEventArgs_DataTransfer()
    {
        const string format = "Managed.Payload";
        var payload = new ManagedPayload("payload");

        IDataTransfer dataTransfer = ManagedContextDropBehavior.CreateDataTransfer(payload, format);

        Assert.Same(payload, dataTransfer.TryGetManagedValue<ManagedPayload>(format));
    }

    [Fact]
    public void ManagedDataTransferExtensions_Keep_ByteArray_Payload_Compatible_With_Avalonia_Byte_APIs()
    {
        const string format = "Managed.Bytes";
        byte[] payload = [0x01, 0x02, 0x03];

        IDataTransfer dataTransfer = ManagedPayloadDataTransfer.Create(format, payload);

        Assert.Same(payload, dataTransfer.TryGetManagedValue(format));
        Assert.Same(payload, dataTransfer.TryGetValue(DataFormat.CreateBytesApplicationFormat(format)));
    }

    private sealed record ManagedPayload(string Value);
}
