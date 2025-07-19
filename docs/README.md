# Documentation

Welcome to the project documentation. Below is the table of contents that mirrors the structure of the main README.

## Table of Contents

- [Building Avalonia XAML Behaviors](../README.md#building-avalonia-xaml-behaviors)
  - [Build on Windows using script](../README.md#build-on-windows-using-script)
  - [Build on Linux using script](../README.md#build-on-linux-using-script)
  - [Build on OSX using script](../README.md#build-on-osx-using-script)
- [NuGet](../README.md#nuget)
  - [Package Sources](../README.md#package-sources)
  - [Available Packages](../README.md#available-packages)
- [Getting Started](../README.md#getting-started)
  - [Adding Behaviors in XAML](../README.md#adding-behaviors-in-xaml)
  - [Basic Examples](../README.md#basic-examples)
  - [How It Works](../README.md#how-it-works)
  - [Advanced Usage](../README.md#advanced-usage)
- [Docs](../README.md#docs)
- [Interactions](../README.md#interactions)
- [Interactivity (Infrastructure)](../README.md#interactivity-infrastructure)
- [Resources](../README.md#resources)
- [License](../README.md#license)

For detailed information about each section, refer to the [project README](../README.md).

## Generating API Docs

After building the solution you can create markdown stubs for behaviors,
actions and triggers. Run the following command:

```bash
./build.sh --target Docs
```

New files will appear under `docs/Behaviors`, `docs/Actions` and `docs/Triggers`
for any components that are missing documentation.
