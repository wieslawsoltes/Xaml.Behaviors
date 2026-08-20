# Animations sample application

This application demonstrates `Xaml.Behaviors.Animations` without referencing `Xaml.Behaviors.Interactivity` or any interactions package.

The left-hand `SingleSelectionTabControl` and sidebar theme mirror the main behaviors sample. Each page focuses on one standalone feature family:

- key-frame factory, builder, and synchronous/asynchronous runners;
- fluid movement with automatic transform preparation;
- fade, sliding, scale, and rotation primitives;
- attention, entrance, exit, Framer Motion, and special catalogs;
- selection indicator animation;
- orbit, tilt, and parallax composition effects;
- transition collection mutation and observation operations.

Run it from the repository root:

```bash
dotnet run --project samples/AnimationsTestApplication/AnimationsTestApplication.csproj
```
