# Codex Agent Guide

This repository hosts the Avalonia port of **XAML Behaviors**. The project is a .NET solution targeting **.NET 9** and uses [Nuke](https://nuke.build) for its build scripts.

## Building the solution

- The main solution file is `AvaloniaBehaviors.sln`.
- You can build using the cross platform scripts:
  ```bash
  ./build.sh            # Linux/macOS
  .\build.ps1           # Windows PowerShell
  ```
- These scripts invoke Nuke and will restore, compile and run any requested target.
- For a manual build, run:
  ```bash
  dotnet build AvaloniaBehaviors.sln --configuration Release
  ```

## Running tests

- Unit tests live under the `tests` directory and are included in the solution.
- To execute them use either:
  ```bash
  dotnet test AvaloniaBehaviors.sln --configuration Release
  ```
  or run the Nuke target:
  ```bash
  ./build.sh --target Test
  ```
- Ensure tests pass before committing any changes.

## Coding style

- The repository follows the rules defined in `.editorconfig`.
- C# files use **4 spaces** for indentation. XAML and XML files use **2 spaces**.
- End files with a newline and avoid trailing whitespace.

## Notes for Codex

- Run the full test suite with `dotnet test` after modifications.
- Use the existing build scripts when possible so the correct .NET SDK version (from `global.json`) is used.
- No additional instructions or AGENTS files exist in the repo.
