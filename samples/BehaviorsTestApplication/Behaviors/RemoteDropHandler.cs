using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BehaviorsTestApplication.ViewModels;

namespace BehaviorsTestApplication.Behaviors;

public sealed class RemoteDropHandler : DropHandlerBase
{
    public string? Url { get; set; }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return e.Data.Contains(DataFormats.Files) && targetContext is MainWindowViewModel && !string.IsNullOrEmpty(Url);
    }

    public override async Task<bool> ExecuteAsync(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (!e.Data.Contains(DataFormats.Files) || targetContext is not MainWindowViewModel vm || string.IsNullOrEmpty(Url))
        {
            return false;
        }

        var files = e.Data.GetFiles();
        if (files is null)
        {
            return false;
        }

        using var client = new HttpClient();
        foreach (var file in files)
        {
#if NET6_0_OR_GREATER
            await using var stream = File.OpenRead(file.Path);
#else
            using var stream = File.OpenRead(file.Path);
#endif
            using var content = new StreamContent(stream);
            var response = await client.PostAsync(Url, content);
            vm.UploadCompletedCommand?.Execute(response);
        }

        return true;
    }
}
