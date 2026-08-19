using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml;
using Xunit;

namespace Avalonia.Xaml.Interactions.UnitTests.Custom;

public class XmlnsDefinitionRuntimeXamlTests
{
    [AvaloniaFact]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "This test intentionally exercises Avalonia's runtime XAML compiler.")]
    public void Runtime_Loader_Resolves_Explicit_Behavior_Assemblies_Alongside_Application_XmlnsDefinitions()
    {
        const string xaml = """
            <features:PreviewFeatureControl
                xmlns="https://github.com/avaloniaui"
                xmlns:features="https://unit.test/features"
                xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Xaml.Behaviors.Interactivity"
                xmlns:core="clr-namespace:Avalonia.Xaml.Interactions.Core;assembly=Xaml.Behaviors.Interactions">
              <i:Interaction.Behaviors>
                <core:EventTriggerBehavior EventName="Loaded" />
              </i:Interaction.Behaviors>
            </features:PreviewFeatureControl>
            """;

        var result = AvaloniaRuntimeXamlLoader.Load(
            xaml,
            typeof(XmlnsDefinitionRuntimeXamlTests).Assembly,
            designMode: true);

        Assert.IsType<PreviewFeatureControl>(result);
    }
}

public sealed class PreviewFeatureControl : Border;
