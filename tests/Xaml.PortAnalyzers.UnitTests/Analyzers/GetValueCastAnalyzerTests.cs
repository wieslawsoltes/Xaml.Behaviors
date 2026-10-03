// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xaml.PortAnalyzers.Analyzers;
using Xaml.PortAnalyzers.CodeFixes;
using Xaml.PortAnalyzers.UnitTests.Infrastructure;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Analyzers;

public class GetValueCastAnalyzerTests
{
    private const string Header = """
        using Avalonia;

        namespace Test;

        public partial class Sample : AvaloniaObject
        {
            public static readonly StyledProperty<bool> FlagProperty = AvaloniaProperty.Register<Sample, bool>("Flag");
            public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<Sample, string?>("Text");
            public static readonly StyledProperty<object?> ValueProperty = AvaloniaProperty.Register<Sample, object?>("Value");

        """;

    [Fact]
    public async Task Reports_GettersReturningUncastGetValue()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource(Header + """
                    public bool Accessor { get => GetValue(FlagProperty); set => SetValue(FlagProperty, value); }
                    public bool Expression => GetValue(FlagProperty);
                    public string? Block
                    {
                        get
                        {
                            if (Accessor)
                            {
                                return this.GetValue(TextProperty);
                            }

                            return (GetValue(TextProperty));
                        }
                    }
                    public string NullForgiving => this.GetValue(TextProperty)!;
                    public bool Cast => (bool)GetValue(FlagProperty);
                    public object? Object => GetValue(ValueProperty);
                    public object Object2 => GetValue(ValueProperty)!;
                    public bool NotGetValue => Equals(null);
                    public bool Setter { set => SetValue(FlagProperty, value); }
                }
                """)
            .GetDiagnosticsAsync(new GetValueCastAnalyzer());

        Assert.Equal(
            [
                "XPORT004:10:'GetValue(FlagProperty)'",
                "XPORT004:11:'GetValue(FlagProperty)'",
                "XPORT004:18:'this.GetValue(TextProperty)'",
                "XPORT004:21:'(GetValue(TextProperty))'",
                "XPORT004:24:'this.GetValue(TextProperty)!'",
            ],
            diagnostics.Describe());
        Assert.Equal(
            "Getter of property 'Accessor' returns the result of 'GetValue' without a cast; cast it to 'bool' because 'GetValue' returns 'object' on the port target",
            diagnostics[0].GetMessage());
    }

    [Fact]
    public async Task Reports_GenericPropertyTypes()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithSource("""
                using Avalonia;

                public partial class Box<T> : AvaloniaObject
                {
                    public static readonly StyledProperty<T> ItemProperty = AvaloniaProperty.Register<Box<T>, T>("Item");
                    public T Item => GetValue(ItemProperty);
                }
                """)
            .GetDiagnosticsAsync(new GetValueCastAnalyzer());

        Assert.Equal(["XPORT004:6:'GetValue(ItemProperty)'"], diagnostics.Describe());
    }

    [Fact]
    public async Task Ignores_PortInactiveGettersAndExcludedFiles()
    {
        var diagnostics = await new PortAnalyzerTest()
            .WithGlobalOption("build_property.XamlPortSharedSourceExcludes", "Excluded.cs")
            .WithSource(Header + """
                    public bool Guarded
                    {
                #if UNO
                        get => (bool)GetValue(FlagProperty);
                #else
                        get => GetValue(FlagProperty);
                #endif
                    }
                }
                """)
            .WithSource("Excluded.cs", """
                public partial class Other : Avalonia.AvaloniaObject
                {
                    public static readonly Avalonia.StyledProperty<bool> FlagProperty = Avalonia.AvaloniaProperty.Register<Other, bool>("Flag");
                    public bool Flag => GetValue(FlagProperty);
                }
                """)
            .GetDiagnosticsAsync(new GetValueCastAnalyzer());

        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task CodeFix_CastsToDeclaredPropertyType()
    {
        var fixedSource = await new PortAnalyzerTest()
            .WithSource(Header + """
                    public bool Accessor
                    {
                        get => GetValue(FlagProperty);
                        set => SetValue(FlagProperty, value);
                    }

                    public string? Text => this.GetValue(TextProperty);

                    public string Block
                    {
                        get { return GetValue(TextProperty)!; }
                    }
                }
                """)
            .ApplyCodeFixesAsync(new GetValueCastAnalyzer(), new AddGetValueCastCodeFixProvider());

        Assert.Equal(Header + """
                public bool Accessor
                {
                    get => (bool)GetValue(FlagProperty);
                    set => SetValue(FlagProperty, value);
                }

                public string? Text => (string?)this.GetValue(TextProperty);

                public string Block
                {
                    get { return (string)GetValue(TextProperty)!; }
                }
            }
            """, fixedSource);
    }

    [Theory]
    [InlineData("GetValue(P)", true)]
    [InlineData("this.GetValue(P)", true)]
    [InlineData("((GetValue(P)))", true)]
    [InlineData("GetValue(P)!", true)]
    [InlineData("GetValue<int>(P)", true)]
    [InlineData("(int)GetValue(P)", false)]
    [InlineData("GetValue(P) as string", false)]
    [InlineData("GetValues(P)", false)]
    [InlineData("x?.GetValue(P)", false)]
    [InlineData("GetValue", false)]
    public void IsUncastGetValueInvocation_MatchesOnlyPlainCalls(string expression, bool expected)
    {
        Assert.Equal(expected, GetValueCastAnalyzer.IsUncastGetValueInvocation(SyntaxFactory.ParseExpression(expression)));
    }

    [Fact]
    public void CreateCast_WrapsExpressionAndKeepsTrivia()
    {
        var expression = SyntaxFactory.ParseExpression(" GetValue(P) ");

        var cast = AddGetValueCastCodeFixProvider.CreateCast(expression, SyntaxFactory.ParseTypeName("int?"));

        Assert.Equal(" (int?)GetValue(P) ", cast.ToFullString());
        Assert.IsType<CastExpressionSyntax>(cast);
    }
}
