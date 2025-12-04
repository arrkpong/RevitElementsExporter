# Repository Guidelines

## Project Structure & Module Organization
- Solution: `RevitElementsExporter.sln`.
- Add-in project: `RevitElementsExporter/` contains `RevitElementsExporter.csproj` and the main command `ExportCoordinates.cs`.
- Build outputs: `RevitElementsExporter/bin/` and `obj/` (generated).
- Revit add-in manifest: `RevitElementsExporter/.addin` (copy/deploy into Revit add-ins folder when distributing).

## Build, Test, and Development Commands
- `dotnet build`: restores packages and compiles the add-in for `net8.0-windows` with WPF support. Run from repo root.
- `dotnet restore`: fetches dependencies if you need to prime the cache.
- Tests: none currently defined; add under a sibling test project if needed.

## Coding Style & Naming Conventions
- Language: C# 12, nullable enabled, implicit usings on.
- Indentation: 4 spaces; braces on new lines (C# default style).
- Naming: PascalCase for classes/methods/properties, camelCase for locals/parameters, `_camelCase` for private fields if added.
- CSV handling: use `EscapeCsv` helper patterns already in `ExportCoordinates.cs`; prefer `StringBuilder` for aggregation.
- Units: store Revit feet; convert to meters with the existing `FeetToMeters` constant for outputs.

## Testing Guidelines
- Add unit tests with xUnit or NUnit in a separate test project (e.g., `RevitElementsExporter.Tests`) and target the same framework.
- Mock Revit API calls; avoid loading real `RevitAPI.dll` in unit tests. Extract logic to pure functions where possible.
- Name tests with clear intent (e.g., `Exports_PointLocation_WithMeters`).

## Commit & Pull Request Guidelines
- Commits: write concise, imperative summaries (e.g., "Add XYZ export for curve elements"). Group related changes together.
- PRs: include what changed, why, and how to validate (commands run, expected outputs). Link related issue/task. Add screenshots only if UI changes occur (unlikely here).

## Build/Runtime Environment Tips
- Target framework: `net8.0-windows`; platform target: `x64` to match Revit 2026 API binaries.
- External references: ensure local paths to `RevitAPI.dll` and `RevitAPIUI.dll` match your Revit installation (default `C:\Program Files\Autodesk\Revit 2026\`).
- Deployment: copy the built assembly and `.addin` manifest into `%AppData%\Autodesk\Revit\Addins\2026\` (or the version you target). Restart Revit to load changes.
