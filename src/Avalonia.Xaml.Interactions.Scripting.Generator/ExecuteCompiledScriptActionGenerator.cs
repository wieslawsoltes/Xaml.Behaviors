using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Avalonia.Xaml.Interactions.Scripting.Generator;

[Generator]
public class ExecuteCompiledScriptActionGenerator : IIncrementalGenerator
{
    private static readonly Regex s_scriptRegex = new(
        "<[^:]+:ExecuteCompiledScriptAction[^>]*Script=\"(?<script>[^\"]*)\"",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var xamlFiles = context.AdditionalTextsProvider.Where(static file =>
            file.Path.EndsWith(".xaml") || file.Path.EndsWith(".axaml"));

        var scriptEntries = xamlFiles.SelectMany((file, cancellation) =>
        {
            var text = file.GetText(cancellation);
            if (text is null)
            {
                return ImmutableArray<(string Path, string Script)>.Empty;
            }

            var matches = s_scriptRegex.Matches(text.ToString());
            var builder = ImmutableArray.CreateBuilder<(string, string)>();
            foreach (Match match in matches.Cast<Match>())
            {
                if (match.Groups["script"].Success)
                {
                    builder.Add((file.Path, match.Groups["script"].Value));
                }
            }
            return builder.ToImmutable();
        });

        context.RegisterSourceOutput(scriptEntries, static (spc, entry) =>
        {
            var (path, script) = entry;
            var className = $"ExecuteCompiledScriptAction_{path.GetHashCode():X}";
            var source = $@"namespace Avalonia.Xaml.Interactions.Scripting;
public partial class {className} : ExecuteCompiledScriptAction
{{
    partial void ExecuteCompiledCode(object? sender, object? parameter)
    {{
        {script}
    }}
}}";
            spc.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }
}
