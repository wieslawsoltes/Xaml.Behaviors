using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.IntegrationTests;

public sealed class SourceGeneratorPackageFixture : IAsyncLifetime
{
    public string RepoRoot { get; }
    public string PackagesDirectory { get; private set; } = string.Empty;
    public string PackageCacheDirectory { get; private set; } = string.Empty;
    public string GeneratorVersion { get; private set; } = string.Empty;
    public string BehaviorsVersion { get; private set; } = string.Empty;
    public string AvaloniaVersion { get; }
    public string PackageVersion { get; }
    private string VersionPrefix { get; }
    public IReadOnlyDictionary<string, string> NuGetEnvironment => new Dictionary<string, string>
    {
        ["NUGET_PACKAGES"] = PackageCacheDirectory
    };

    public SourceGeneratorPackageFixture()
    {
        RepoRoot = LocateRepoRoot();
        AvaloniaVersion = PackageVersionReader.GetProperty(RepoRoot, "AvaloniaVersion");
        VersionPrefix = PackageVersionReader.GetProperty(RepoRoot, "VersionPrefix");
        PackageVersion = $"{VersionPrefix}-it.{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    public async Task InitializeAsync()
    {
        PackagesDirectory = Path.Combine(Path.GetTempPath(), "Avalonia.Xaml.Behaviors.Packages", Guid.NewGuid().ToString("N"));
        PackageCacheDirectory = Path.Combine(PackagesDirectory, "cache");
        Directory.CreateDirectory(PackagesDirectory);
        Directory.CreateDirectory(PackageCacheDirectory);

        await PackProjectAsync(Path.Combine(RepoRoot, "src", "Xaml.Behaviors.SourceGenerators", "Xaml.Behaviors.SourceGenerators.csproj"));
        GeneratorVersion = PackageVersion;

        await PackProjectAsync(Path.Combine(RepoRoot, "src", "Xaml.Behaviors", "Xaml.Behaviors.csproj"));
        BehaviorsVersion = PackageVersion;
    }

    public Task DisposeAsync()
    {
        TryDeleteDirectory(PackagesDirectory);
        return Task.CompletedTask;
    }

    private static string LocateRepoRoot()
    {
        var current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            var slnx = Path.Combine(current, "AvaloniaBehaviors.slnx");
            if (File.Exists(slnx))
            {
                return current;
            }

            var parent = Directory.GetParent(current);
            if (parent == null)
            {
                break;
            }

            current = parent.FullName;
        }

        throw new InvalidOperationException("Could not locate repository root containing AvaloniaBehaviors.slnx.");
    }

    private async Task PackProjectAsync(string projectPath)
    {
        var result = await ProcessRunner.RunAsync("dotnet", new[]
        {
            "pack",
            projectPath,
            "-c",
            "Release",
            "-o",
            PackagesDirectory,
            "/p:IncludeSymbols=false",
            "/p:IncludeSource=false",
            "/p:VersionSuffix=",
            $"/p:PackageVersion={PackageVersion}"
        }, RepoRoot, new Dictionary<string, string> { ["NUGET_PACKAGES"] = PackageCacheDirectory });

        Assert.True(result.ExitCode == 0, $"dotnet pack failed for {projectPath}:{Environment.NewLine}{result.AllOutput}");
    }

    private static void TryDeleteDirectory(string directory)
    {
        try
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup only.
        }
    }
}

public class NuGetPackageReferenceTests : IClassFixture<SourceGeneratorPackageFixture>
{
    private readonly SourceGeneratorPackageFixture _fixture;

    public NuGetPackageReferenceTests(SourceGeneratorPackageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PackageReference_GeneratesAndExecutesAction()
    {
        var sampleDirectory = Path.Combine(_fixture.PackagesDirectory, "SampleProject");
        Directory.CreateDirectory(sampleDirectory);

        CreateNuGetConfig(sampleDirectory, _fixture.PackagesDirectory);
        var projectFile = CreateSampleProject(sampleDirectory);
        var programFile = Path.Combine(sampleDirectory, "Program.cs");

        File.WriteAllText(programFile, """
using Xaml.Behaviors.SourceGenerators;

namespace IntegrationSample;

public partial class TestViewModel
{
    public int Calls { get; private set; }

    [GenerateTypedAction]
    public void Submit()
    {
        Calls++;
    }
}

public static class Program
{
    public static int Main()
    {
        var vm = new TestViewModel();
        var action = new SubmitAction { TargetObject = vm };
        action.Execute(null, null);
        return vm.Calls == 1 ? 0 : -1;
    }
}
""");

        var buildResult = await ProcessRunner.RunAsync("dotnet", new[]
        {
            "build",
            projectFile,
            "-c",
            "Release",
            "--nologo"
        }, sampleDirectory, _fixture.NuGetEnvironment);

        Assert.True(buildResult.ExitCode == 0, $"dotnet build failed:{Environment.NewLine}{buildResult.AllOutput}");

        var runResult = await ProcessRunner.RunAsync("dotnet", new[]
        {
            "run",
            "--project",
            projectFile,
            "-c",
            "Release",
            "--no-build"
        }, sampleDirectory, _fixture.NuGetEnvironment);

        Assert.True(runResult.ExitCode == 0, $"dotnet run failed:{Environment.NewLine}{runResult.AllOutput}");
    }

    [Fact]
    public async Task PackageReference_GeneratesAndExecutesMultipleGenerators()
    {
        var sampleDirectory = Path.Combine(_fixture.PackagesDirectory, "SampleProject.Multi");
        Directory.CreateDirectory(sampleDirectory);

        CreateNuGetConfig(sampleDirectory, _fixture.PackagesDirectory);
        var projectFile = CreateSampleProject(sampleDirectory, "SampleProject.Multi");
        var programFile = Path.Combine(sampleDirectory, "Program.cs");

        File.WriteAllText(programFile, """
using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using Xaml.Behaviors.Generated;

[assembly: GenerateTypedDataTrigger(typeof(int))]

namespace IntegrationSample.Multi;

public partial class SampleControl : Control
{
    public int CallCount { get; private set; }
    public string? Status { get; set; }

    [GenerateTypedAction]
    public void Increment() => CallCount++;

    [GenerateTypedTrigger]
    public event EventHandler? Clicked;

    [GenerateTypedChangePropertyAction]
    public string Tag { get; set; } = string.Empty;

    public void Raise() => Clicked?.Invoke(this, EventArgs.Empty);
}

public static class Program
{
    public static int Main()
    {
        var control = new SampleControl();

        var setTag = new SetTagAction { TargetObject = control, Value = "Hello" };
        setTag.Execute(null, null);
        if (control.Tag != "Hello") return -1;

        var action = new IncrementAction { TargetObject = control };
        action.Execute(null, null);
        if (control.CallCount != 1) return -2;

        var trigger = new ClickedTrigger();
        trigger.Actions!.Add(new IncrementAction { TargetObject = control });
        trigger.Attach(control);
        control.Raise();
        if (control.CallCount != 2) return -3;

        var dataTrigger = new Int32DataTrigger
        {
            Binding = 5,
            Value = 5,
            ComparisonCondition = ComparisonConditionType.Equal
        };
        dataTrigger.Actions!.Add(new IncrementAction { TargetObject = control });
        dataTrigger.Attach(control);
        if (control.CallCount != 3) return -4;

        return 0;
    }
}
""");

        var buildResult = await ProcessRunner.RunAsync("dotnet", new[]
        {
            "build",
            projectFile,
            "-c",
            "Release",
            "--nologo"
        }, sampleDirectory, _fixture.NuGetEnvironment);

        Assert.True(buildResult.ExitCode == 0, $"dotnet build failed:{Environment.NewLine}{buildResult.AllOutput}");

        var runResult = await ProcessRunner.RunAsync("dotnet", new[]
        {
            "run",
            "--project",
            projectFile,
            "-c",
            "Release",
            "--no-build"
        }, sampleDirectory, _fixture.NuGetEnvironment);

        Assert.True(runResult.ExitCode == 0, $"dotnet run failed:{Environment.NewLine}{runResult.AllOutput}");
    }

    private string CreateSampleProject(string sampleDirectory, string projectName = "SampleProject")
    {
        var projectPath = Path.Combine(sampleDirectory, $"{projectName}.csproj");
        File.WriteAllText(projectPath, $"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Avalonia" Version="{_fixture.AvaloniaVersion}" />
    <PackageReference Include="Xaml.Behaviors" Version="{_fixture.BehaviorsVersion}" />
    <PackageReference Include="Xaml.Behaviors.SourceGenerators" Version="{_fixture.GeneratorVersion}" PrivateAssets="all" OutputItemType="Analyzer" ReferenceOutputAssembly="true" />
  </ItemGroup>
</Project>
""");
        return projectPath;
    }

    private static void CreateNuGetConfig(string sampleDirectory, string packagesDirectory)
    {
        var configPath = Path.Combine(sampleDirectory, "NuGet.Config");
        File.WriteAllText(configPath, $"""
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="{packagesDirectory}" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
""");
    }

}

internal static class PackageVersionReader
{
    public static string GetProperty(string repoRoot, string propertyName)
    {
        var candidates = new[]
        {
            Path.Combine(repoRoot, "Directory.Packages.props"),
            Path.Combine(repoRoot, "Directory.Build.props")
        };

        foreach (var filePath in candidates)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            var document = XDocument.Load(filePath);
            var property = document.Descendants().FirstOrDefault(x => x.Name.LocalName == propertyName)?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(property))
            {
                return property!;
            }
        }

        throw new InvalidOperationException($"Could not read the {propertyName} property from Directory.Packages.props or Directory.Build.props.");
    }
}

internal sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string AllOutput => $"{StandardOutput}{StandardError}";
}

internal static class ProcessRunner
{
    public static async Task<ProcessResult> RunAsync(string fileName, string[] arguments, string workingDirectory, IReadOnlyDictionary<string, string>? environment = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        if (environment != null)
        {
            foreach (var pair in environment)
            {
                startInfo.Environment[pair.Key] = pair.Value;
            }
        }

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        var output = new StringBuilder();
        var error = new StringBuilder();

        using var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (_, data) =>
        {
            if (data.Data is not null)
            {
                output.AppendLine(data.Data);
            }
        };
        process.ErrorDataReceived += (_, data) =>
        {
            if (data.Data is not null)
            {
                error.AppendLine(data.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        return new ProcessResult(process.ExitCode, output.ToString(), error.ToString());
    }
}
