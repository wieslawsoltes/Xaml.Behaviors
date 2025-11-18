using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Xaml.Behaviors.Storyboards.UnitTests;

public class StoryboardPropertyPathTests
{
    [Fact]
    public void Parse_SimpleProperty()
    {
        var path = StoryboardPropertyPath.Parse("Opacity");

        Assert.Equal("Opacity", path.OriginalPath);
        Assert.Single(path.Segments);
        Assert.Equal("Opacity", path.Segments[0].PropertyName);
        Assert.False(path.Segments[0].IsAttached);
    }

    [Fact]
    public void Parse_AttachedProperty()
    {
        var path = StoryboardPropertyPath.Parse("(Canvas.Left)");

        Assert.Single(path.Segments);
        var segment = path.Segments[0];
        Assert.True(segment.IsAttached);
        Assert.Equal("Canvas", segment.OwnerTypeName);
        Assert.Equal("Left", segment.PropertyName);
    }

    [Fact]
    public void Parse_NestedAttachedProperty()
    {
        var path = StoryboardPropertyPath.Parse("(Border.Background).(SolidColorBrush.Color)");

        Assert.Equal(2, path.Segments.Count);
        Assert.Equal("Border", path.Segments[0].OwnerTypeName);
        Assert.Equal("Background", path.Segments[0].PropertyName);
        Assert.Equal("SolidColorBrush", path.Segments[1].OwnerTypeName);
        Assert.Equal("Color", path.Segments[1].PropertyName);
    }

    [Fact]
    public void Parse_IndexerSegments()
    {
        var path = StoryboardPropertyPath.Parse("RenderTransform.Children[0].Angle");

        Assert.Equal(3, path.Segments.Count);
        Assert.True(path.Segments[1].HasIndexers);
        Assert.Equal("0", Assert.Single(path.Segments[1].IndexerArguments));
    }

    [AvaloniaFact]
    public void StoryboardService_Parses_Target_PropertyPath()
    {
        var animation = new Border();
        StoryboardService.SetTargetProperty(animation, "(Canvas.Left)");

        var metadata = StoryboardService.GetTargetMetadata(animation);

        Assert.True(metadata.HasTargetProperty);
        Assert.NotNull(metadata.ParsedTargetProperty);
        Assert.Equal("Canvas", metadata.ParsedTargetProperty!.Segments[0].OwnerTypeName);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests rely on storyboard property path reflection.")]
    public void Resolver_Handles_SimpleProperty()
    {
        var border = new Border();
        var path = StoryboardPropertyPath.Parse("Opacity");

        var resolved = StoryboardPropertyPathResolver.Resolve(border, path);

        Assert.Same(border, resolved.Target);
        Assert.Same(Border.OpacityProperty, resolved.Property);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests rely on storyboard property path reflection.")]
    public void Resolver_Handles_AttachedProperty()
    {
        var border = new Border();
        var path = StoryboardPropertyPath.Parse("(Canvas.Left)");

        var resolved = StoryboardPropertyPathResolver.Resolve(border, path);

        Assert.Same(border, resolved.Target);
        Assert.Same(Canvas.LeftProperty, resolved.Property);
    }

    [AvaloniaFact]
    [RequiresUnreferencedCode("Tests rely on storyboard property path reflection.")]
    public void Resolver_Handles_NestedProperty()
    {
        var brush = new SolidColorBrush(Colors.Red);
        var border = new Border { Background = brush };
        var path = StoryboardPropertyPath.Parse("(Border.Background).(SolidColorBrush.Color)");

        var resolved = StoryboardPropertyPathResolver.Resolve(border, path);

        Assert.Same(brush, resolved.Target);
        Assert.Same(SolidColorBrush.ColorProperty, resolved.Property);
    }
}
