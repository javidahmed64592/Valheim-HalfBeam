# Changelog

## Unreleased

- Added 0.5m grausten small and medium beams and pillars (the medium ones keep all ten snap points).
- Added half and quarter wood iron, timber and decorated timber beams and poles, and their half-length 26°, 45° and 67° diagonals (21 new pieces).
- Darkwood pieces are now shortened by cutting a section out of the middle instead of being squashed, so the carved pattern keeps its size. This fixes the stretched darkwood trim from 0.1.0.
- Timber pieces are scaled rather than cut, because their slightly barrel-shaped planks showed a seam.
- New icons for the new pieces and the darkwood pieces, cut the same way.
- Added 1m and 0.5m log poles and log beams (log beams keep all four snap points), 2m, 1m and 0.5m worn log poles, 1m and 0.5m flametal beams, and half-length ashwood 26° and 45° alternative diagonals.
- Added dvergr poles: a buildable full-length 2m pole (vanilla can't build it) and 1m and 0.5m poles whose copper band keeps its real size.
- Pieces with more than two snap points can now be shortened.
- Added 1m and 0.5m flametal pillars and half-length log beam 26°, 45° and 67° diagonals.
- All pieces now appear on the Building and Architecture tabs, including the dvergr poles and ashwood alternative diagonals, whose vanilla pieces have no tab.
- Moved the source files into folders (`Core`, `Pieces`), leaving only `Plugin.cs` at the root. No behaviour change.

## 0.3.0

- Piece names and descriptions can now be translated (`Translations/English.json`). The English names are unchanged.
- Added a CI check that the version matches in every file.
- Moved `package.sh` into `.github/scripts` and made it the same for every mod: the name, version and DLL now come from `manifest.json` and the `.csproj`.

## 0.2.1

- Updated the BepInExPack_Valheim dependency to 5.4.2360.

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
