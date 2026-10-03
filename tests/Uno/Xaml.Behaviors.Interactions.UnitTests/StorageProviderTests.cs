// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Xaml.Behaviors.Uno.Headless;
using Xaml.Behaviors.Uno.Headless.XUnit;
using Xaml.Interactions.Core;
using Xaml.Interactivity;
using Xunit;

namespace Xaml.Interactions.UnitTests;

public sealed class FakeStorageProvider(IStorageFile file, IStorageFolder folder) : IStorageProvider
{
    public FilePickerOpenOptions? OpenOptions { get; private set; }

    public FilePickerSaveOptions? SaveOptions { get; private set; }

    public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
    {
        OpenOptions = options;
        return Task.FromResult<IReadOnlyList<IStorageFile>>([file]);
    }

    public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
    {
        SaveOptions = options;
        return Task.FromResult<IStorageFile?>(file);
    }

    public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        => Task.FromResult<IReadOnlyList<IStorageFolder>>([folder]);

    public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
        => Task.FromResult<IStorageFolder?>(folderPath.LocalPath.TrimEnd('/') == folder.Path.TrimEnd('/') ? folder : null);
}

public class StorageProviderTests
{
    private static UnoHeadlessSession Session => UnoHeadlessSession.Current;

    private static async Task<(FakeStorageProvider Provider, IStorageFile File, IStorageFolder Folder)> CreateProviderAsync()
    {
        var directory = Directory.CreateTempSubdirectory("xaml-behaviors-uno-");
        var path = Path.Combine(directory.FullName, "data.txt");
        await File.WriteAllTextAsync(path, "content");
        var file = await StorageFile.GetFileFromPathAsync(path);
        var folder = await StorageFolder.GetFolderFromPathAsync(directory.FullName);
        return (new FakeStorageProvider(file, folder), file, folder);
    }

    [UnoHeadlessFact]
    public async Task OpenFilePickerAction_Passes_Files_And_Filters()
    {
        var (provider, file, _) = await CreateProviderAsync();
        var command = new RecordingCommand();
        var action = new OpenFilePickerAction
        {
            StorageProvider = provider,
            Command = command,
            Title = "Open",
            AllowMultiple = true,
            FileTypeFilter = "Text files (*.txt)|*.txt;*.md|All files|*.*",
        };

        await (Task)action.Execute(new Border(), null);

        var files = Assert.IsAssignableFrom<IReadOnlyList<IStorageFile>>(Assert.Single(command.Parameters));
        Assert.Same(file, Assert.Single(files));
        Assert.Equal("Open", provider.OpenOptions!.Title);
        Assert.True(provider.OpenOptions.AllowMultiple);
        Assert.Equal(["Text files", "All files"], provider.OpenOptions.FileTypeFilter!.Select(static t => t.Name));
        Assert.Equal(["*.txt", "*.md"], provider.OpenOptions.FileTypeFilter![0].Patterns!);
    }

    [UnoHeadlessFact]
    public async Task SaveFilePickerAction_Raises_Pick_And_Resolves_Start_Location()
    {
        var (provider, file, folder) = await CreateProviderAsync();
        IStorageFile? picked = null;
        var action = new SaveFilePickerAction
        {
            StorageProvider = provider,
            SuggestedFileName = "out",
            DefaultExtension = "txt",
            SuggestedStartLocationPath = folder.Path,
        };
        action.Pick += (_, e) => picked = e.File;

        await (Task)action.Execute(new Border(), null);

        Assert.Same(file, picked);
        Assert.Equal("out", provider.SaveOptions!.SuggestedFileName);
        Assert.Same(folder, provider.SaveOptions.SuggestedStartLocation);
    }

    [UnoHeadlessFact]
    public async Task OpenFolderPickerAction_Passes_Folders()
    {
        var (provider, _, folder) = await CreateProviderAsync();
        var command = new RecordingCommand();

        await (Task)new OpenFolderPickerAction { StorageProvider = provider, Command = command }.Execute(new Border(), null);

        Assert.Same(folder, Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<IStorageFolder>>(Assert.Single(command.Parameters))));
    }

    [UnoHeadlessFact]
    public async Task ButtonOpenFilePickerBehavior_Opens_Picker_On_Click()
    {
        var (provider, file, _) = await CreateProviderAsync();
        var command = new RecordingCommand();
        var button = new Button();
        Interaction.GetBehaviors(button).Add(new ButtonOpenFilePickerBehavior { StorageProvider = provider, Command = command });
        await Session.ShowAsync(button);

        new ButtonAutomationPeer(button).Invoke();
        for (var i = 0; i < 20 && command.Parameters.Count == 0; i++)
        {
            await Task.Delay(10);
            await Session.WaitForIdleAsync();
        }

        Assert.Same(file, Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<IStorageFile>>(Assert.Single(command.Parameters))));
    }

    [UnoHeadlessFact]
    public async Task Storage_Converters_Open_Streams_And_Paths()
    {
        var (_, file, _) = await CreateProviderAsync();

        using (var read = await (Task<Stream>)StorageFileToReadStreamConverter.Instance.Convert(file, typeof(object), null, string.Empty))
        using (var reader = new StreamReader(read))
        {
            Assert.Equal("content", await reader.ReadToEndAsync());
        }

        Assert.Equal(file.Path, StorageItemToPathConverter.Instance.Convert(file, typeof(string), null, string.Empty));
    }
}
