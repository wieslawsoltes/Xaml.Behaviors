using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

public class FilesDropBehaviorTests
{
    [AvaloniaFact]
    public void FilesDropBehavior_Drops_Files()
    {
        var tmp1 = Path.GetTempFileName();
        var tmp2 = Path.GetTempFileName();
        File.WriteAllText(tmp1, "1");
        File.WriteAllText(tmp2, "2");

        var provider = new MockStorageProvider();
        var file1 = provider.GetFile(tmp1);
        var file2 = provider.GetFile(tmp2);

        var window = new FilesDropBehavior001();
        window.Show();

        var data = new DataObject();
        data.Set(DataFormats.Files, new[] { file1, file2 });

        var args = new DragEventArgs(DragDrop.DropEvent, data, window.TargetBorder, new Point(0, 0), KeyModifiers.None);
        window.TargetBorder.RaiseEvent(args);

        Assert.NotNull(window.DroppedFiles);
        var list = new List<IStorageItem>(window.DroppedFiles!);
        Assert.Equal(2, list.Count);
        Assert.Equal(file1.Path, ((IStorageFile)list[0]).Path);
        Assert.Equal(file2.Path, ((IStorageFile)list[1]).Path);
    }
}
