using System;
using Avalonia.Controls;
using Avalonia.Styling;
using Xunit;

namespace Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

public class StoryboardTargetResolverTests
{
    [Fact]
    public void ResolveTarget_UsesExplicitTarget()
    {
        var animation = new Border();
        var explicitTarget = new Button();
        StoryboardService.SetTarget(animation, explicitTarget);

        var resolver = new StoryboardTargetResolver(associatedObject: null);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var result = resolver.ResolveTarget(metadata);

        Assert.Same(explicitTarget, result);
    }

    [Fact]
    public void ResolveTarget_UsesNameScope()
    {
        var host = new ContentControl();
        var target = new Border();
        var nameScope = new NameScope();
        nameScope.Register("TargetBorder", target);
        NameScope.SetNameScope(host, nameScope);

        var animation = new Border();
        StoryboardService.SetTargetName(animation, "TargetBorder");

        var resolver = new StoryboardTargetResolver(host);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var result = resolver.ResolveTarget(metadata);

        Assert.Same(target, result);
    }

    [Fact]
    public void ResolveTarget_UsesTemplateNameScopeOverride()
    {
        var templateScope = new NameScope();
        var templateTarget = new Border();
        templateScope.Register("PART_ContentHost", templateTarget);

        var control = new ContentControl();
        var animation = new Border();
        StoryboardService.SetTargetName(animation, "PART_ContentHost");

        var resolver = new StoryboardTargetResolver(control, nameScope: templateScope);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var result = resolver.ResolveTarget(metadata);

        Assert.Same(templateTarget, result);
    }

    [Fact]
    public void ResolveTarget_SearchesLogicalAncestors()
    {
        var root = new StackPanel();
        var associated = new Border();
        root.Children.Add(associated);

        var target = new Button();
        var scope = new NameScope();
        scope.Register("NestedButton", target);
        NameScope.SetNameScope(root, scope);

        var animation = new Border();
        StoryboardService.SetTargetName(animation, "NestedButton");

        var resolver = new StoryboardTargetResolver(associated);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var result = resolver.ResolveTarget(metadata);

        Assert.Same(target, result);
    }

    [Fact]
    public void ResolveTarget_FallsBackToAssociatedObject()
    {
        var associated = new Border();
        var animation = new Border();

        var resolver = new StoryboardTargetResolver(associated);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var result = resolver.ResolveTarget(metadata);

        Assert.Same(associated, result);
    }

    [Fact]
    public void ResolveTarget_Throws_WhenNameMissing()
    {
        var host = new ContentControl();
        var animation = new Border();
        StoryboardService.SetTargetName(animation, "Missing");

        var resolver = new StoryboardTargetResolver(host);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var exception = Assert.Throws<InvalidOperationException>(() => resolver.ResolveTarget(metadata));
        Assert.Contains("Missing", exception.Message);
    }

    [Fact]
    public void ResolveTarget_Throws_WhenNoFallback()
    {
        var animation = new Border();
        var resolver = new StoryboardTargetResolver(associatedObject: null);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        Assert.Throws<InvalidOperationException>(() => resolver.ResolveTarget(metadata));
    }

    [Fact]
    public void ResolveTarget_Throws_WhenTargetNameUsedInStyle()
    {
        var animation = new Border();
        StoryboardService.SetTargetName(animation, "Target");

        var resolver = new StoryboardTargetResolver(new Style(), isStyleContext: true);
        var metadata = StoryboardService.GetTargetMetadata(animation);

        var exception = Assert.Throws<InvalidOperationException>(() => resolver.ResolveTarget(metadata));
        Assert.Contains("Style", exception.Message);
    }
}
