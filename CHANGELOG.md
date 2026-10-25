# Changelog

## 0.2.0

- Added an `.editorconfig`, `Directory.Build.props` and a CI workflow that runs `dotnet format`, a Release build and a NuGet vulnerability check.
- Reformatted the code to match the new code style rules (no behaviour change).
- Renamed the namespace from `HalfBeams` to `HalfBeam` to match the project.
- Tidied up packaging scripts and workflow files.
- Added a `.gitattributes` to keep line endings consistent.
- Moved the local Valheim install paths into `Directory.Build.props`; set `VALHEIM_DIR` and `BEPINEX_PROFILE_DIR` once to build every mod without editing the `.csproj`.
- Updated README and icon.

## 0.1.1

- Removed debug logging for snap distances and baked node scaling.

## 0.1.0

- Initial release.
- Add 0.5m beams and poles for wood and ashwood, and 1m and 0.5m beams and poles for darkwood.
- Add half-length 26°, 45° and 67° diagonal beams for wood, ashwood and darkwood.
- Add custom icons for all 17 new pieces.
- Known issue: the decorative trim on darkwood beams is stretched.
