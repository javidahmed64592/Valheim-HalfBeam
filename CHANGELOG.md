# Changelog

## 1.0.2

- Updated Jotunn to 2.30.3.

## 1.0.1

- The mod's name is now shown as "Half Beam", with a space like the other mods, for example in BetterHammer's Modded build menu category. Its settings and saved pieces are unchanged.

## 1.0.0

- Added shorter wood iron, timber, decorated timber, log, worn log, flametal, dvergr and grausten pieces, with half-length diagonals where vanilla has them: 42 new pieces, 59 in total.
- Added a buildable full-length dvergr pole, which vanilla can't build.
- Darkwood, wood iron and decorated timber pieces are shortened by cutting out a section instead of squashing, so trims, iron bands and patterns keep their size. This fixes the stretched darkwood trim.
- Shorter dvergr poles keep their copper band at full size.
- Log beams and the grausten medium pieces keep all their snap points.
- Every piece is on the Architecture tab, in the same order as its vanilla original.
- New icons that match the shortened pieces.
- Moved the source files into folders. No behaviour change.

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
