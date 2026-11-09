# LTO ↔ HoD Water Compatibility (unofficial)

An **experimental, server-side Harmony compatibility mod** for Vintage Story that helps **LTO Farming Plus 5.0.0** and **Hydrate or Diedrate (HoD) 2.5.8** work together when irrigating crops from a barrel containing rainwater.

The workaround was developed using ChatGPT assistance, built and tested by a player in a singleplayer Creative world. It is **not an official LTO or HoD release**, and is **not yet confirmed on a dedicated server**. It changes neither original mod DLL.

## The issue

With HoD rainwater in a barrel connected to LTO wooden gutters and a copper sprinkler, the gutter reported **`Water: no source`** despite the barrel having water. The sprinkler therefore could not run.

LTO Lib Plus 5.0.0 centralizes plain-water validation in:

```csharp
LTOLibPlus.Water.LtoPlainWater.IsPlainWater(ItemStack)
```

The standalone mod attaches a Harmony postfix to that exact method. It keeps the original `true` result; if LTO returns `false`, it examines the item's code *path* and accepts `waterportion` or anything beginning with `waterportion-` (regardless of namespace). Other values are unchanged.

**Scope warning:** This is intentionally a broad compatibility workaround, *not* a HoD-specific allowlist. Because `IsPlainWater` is shared, it could also affect other LTO systems such as barrel washing or future LTO Stamina Plus water logic. A native integration should use a narrower, behavior-based validation rule after review.

## Supported test configuration

| Component | Version |
| --- | --- |
| Vintage Story | 1.22.7 Stable |
| LTO Farming Plus | 5.0.0 |
| LTO Lib Plus | 5.0.0 |
| Hydrate or Diedrate | 2.5.8 |
| VSImGui | 1.2.7 (LTO dependency) |
| This compatibility mod | 0.2.0-TEST |

The compiled test package is [**`dist/LtoHodWaterCompat_0.2.0-TEST.zip`**](dist/LtoHodWaterCompat_0.2.0-TEST.zip). This is the ZIP to install; **do not install the source-code repository ZIP**.

## Installation

1. **Back up your world and mod configuration** before applying anything to a permanent server.
2. Install the versions above and keep the original LTO / HoD mod ZIPs unmodified.
3. Remove any older `LtoHodWaterCompat_0.1.x` ZIPs and leave only `LtoHodWaterCompat_0.2.0-TEST.zip` in the active Mods directory. Vintage Story looks in both its installation `Mods` folder and `%AppData%\VintagestoryData\Mods`; check both for duplicates.
4. Completely close Vintage Story (not merely leave the world) before replacing the DLL and then restart. On a dedicated server, stop it before installing and restart it afterward.
5. In `server-main.txt`, look for:

   ```text
   [LTO HoD Water Compat V5] ACTIVE: Postfix applied to LTOLibPlus.Water.LtoPlainWater.IsPlainWater(ItemStack).
   ```

6. In a disposable Creative test world, check that the gutter says `Water: connected` and the copper sprinkler runs with HoD rainwater. The sprinkler also needs its farmland cross pattern and height configured correctly.
7. **For full validation**, confirm farmland moisture increases *and* water quantity in the barrel decreases. Spraying particles alone does not prove both game mechanics.

**Current test status:** The player reported the v0.2.0 workaround **worked** after the 5.0.0 upgrade. Dedicated-server behavior and the two quantitative checks above remain unverified in this project.

## Build from source

Requires a Windows installation of Vintage Story 1.22.7 and .NET SDK 10 (the project references the local `VintagestoryAPI.dll` and `Lib/0Harmony.dll`; neither DLL is redistributed here).

```powershell
$env:VINTAGE_STORY = 'E:\Vintagestory'  # adjust to your install location
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

The build script creates `LtoHodWaterCompat_0.2.0-TEST.zip` in the project directory. It is independent of the checked-in build artifact in `dist/`. For more detail, see [technical notes](docs/TECHNICAL_NOTES.md) and [testing checklist](docs/TESTING.md).

## Reverting

Remove the compatibility ZIP and fully restart the game or dedicated server. There are no edits to LTO Farming Plus, LTO Lib Plus, or HoD original packages to undo.

## For LTO / HoD maintainers

This project is being shared to document a reproducible interoperability problem and a small example fix, not to demand upstream support. You are welcome to inspect or adapt the implementation under the [MIT license](LICENSE). A more selective native solution would be preferable, particularly to avoid interactions with other hydration systems.