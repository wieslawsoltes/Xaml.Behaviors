using Avalonia.Controls;
using Xunit;

namespace Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

public class StoryboardRegistryServiceTests
{
    [Fact]
    public void Register_StoresInstanceAndTryGetSucceeds()
    {
        var service = new StoryboardRegistryService();
        var host = new Border();
        var instance = CreateInstance(host, "Intro");

        service.Register(host, "Intro", instance);

        var found = service.TryGet(host, "Intro", out var resolved);

        Assert.True(found);
        Assert.Same(instance, resolved);

        service.CleanupHost(host);
    }

    [Fact]
    public void Register_SnapshotDisposesPreviousInstance()
    {
        var service = new StoryboardRegistryService();
        var host = new Border();
        var first = CreateInstance(host, "Highlight");
        var second = CreateInstance(host, "Highlight");

        service.Register(host, "Highlight", first);
        service.Register(host, "Highlight", second);

        Assert.True(first.IsDisposed);
        Assert.False(second.IsDisposed);

        service.CleanupHost(host);
    }

    [Fact]
    public void Register_ComposeKeepsExistingInstance()
    {
        var service = new StoryboardRegistryService();
        var host = new Border();
        var first = CreateInstance(host, "Highlight", StoryboardHandoffBehavior.Compose);
        var second = CreateInstance(host, "Highlight", StoryboardHandoffBehavior.Compose);

        service.Register(host, "Highlight", first);
        service.Register(host, "Highlight", second);

        Assert.False(first.IsDisposed);
        Assert.True(second.Layer > first.Layer);

        service.CleanupHost(host);
    }

    [Fact]
    public void Remove_SpecificInstance_DisposesOnlyThatInstance()
    {
        var service = new StoryboardRegistryService();
        var host = new Border();
        var first = CreateInstance(host, "Highlight", StoryboardHandoffBehavior.Compose);
        var second = CreateInstance(host, "Highlight", StoryboardHandoffBehavior.Compose);

        service.Register(host, "Highlight", first);
        service.Register(host, "Highlight", second);

        var removed = service.Remove(host, "Highlight", second);

        Assert.True(removed);
        Assert.True(second.IsDisposed);
        Assert.False(first.IsDisposed);
        Assert.True(service.TryGet(host, "Highlight", out var resolved));
        Assert.Same(first, resolved);

        service.CleanupHost(host);
    }

    [Fact]
    public void CleanupHost_DisposesAllInstances()
    {
        var service = new StoryboardRegistryService();
        var host = new Border();
        var first = CreateInstance(host, "Alpha");
        var second = CreateInstance(host, "Beta");

        service.Register(host, "Alpha", first);
        service.Register(host, "Beta", second);

        service.CleanupHost(host);

        Assert.True(first.IsDisposed);
        Assert.True(second.IsDisposed);
        Assert.False(service.TryGet(host, "Alpha", out _));
        Assert.False(service.TryGet(host, "Beta", out _));
    }

    private static StoryboardInstance CreateInstance(
        StyledElement host,
        string key,
        StoryboardHandoffBehavior behavior = StoryboardHandoffBehavior.SnapshotAndReplace) =>
        new(host, key, behavior);
}
