using System;
using System.IO;
using System.Reflection;
using Avalonia.Platform.Storage;

namespace Avalonia.Xaml.Interactions.UnitTests.Core;

internal static class TestStorage
{
    public static IStorageFile CreateFile(string path)
    {
        var assembly = typeof(IStorageFile).Assembly;
        var type = assembly.GetType("Avalonia.Platform.Storage.FileIO.BclStorageFile")!;
        return (IStorageFile)Activator.CreateInstance(type, new FileInfo(path))!;
    }
}

internal sealed class MockStorageProvider
{
    public IStorageFile GetFile(string path) => TestStorage.CreateFile(path);
}
