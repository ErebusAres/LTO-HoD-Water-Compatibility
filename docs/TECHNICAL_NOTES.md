# Technical notes for maintainers

## Patch target

`LTOLibPlus.Water.LtoPlainWater.IsPlainWater(Vintagestory.API.Common.ItemStack)` — a static method returning `bool` in the **LTO Lib Plus 5.0.0** assembly.

`WaterCompatModSystem.StartServerSide` locates the loaded `LTOLibPlus` assembly by name, looks up the exact containing type and method signature via reflection, then applies a Harmony **postfix**. A failure is caught and logged as `NOT ACTIVE`; the patch doesn't silently claim success.

The postfix is effectively:

```csharp
if (!__result && __0?.Collectible?.Code?.Path is string path &&
    (path.Equals("waterportion", StringComparison.OrdinalIgnoreCase) ||
     path.StartsWith("waterportion-", StringComparison.OrdinalIgnoreCase)))
{
    __result = true;
}
```

`__0` refers to the original method's first argument (`ItemStack`). No original file is changed. The mod unpatches its own Harmony ID when disposed.

## Behavior and constraints

- Existing LTO `true` results are preserved.
- `false` results are changed only for item code **paths** matching `waterportion` or `waterportion-*`.
- The check deliberately ignores the namespace, so it is **not strictly HoD-only**. Any other mod using that item-code pattern may be accepted.
- This patch affects **all callers** of the shared validator, not only LTO gutters and sprinklers.
- The patch does **not** provide HoD pipes, pumps, liquid conversion, or automatic refilling.
- The current mod metadata pins LTO Farming Plus 5.0.0, LTO Lib Plus 5.0.0, and HoD 2.5.8. Compatibility with future versions has not been established.
- There is no runtime feature detection for LTO Stamina Plus; interaction with its hydration system is **unverified**.

## Reproduction and observed outcome

**Before the patch:** Open barrel with HoD rainwater -> LTO wooden gutter -> copper sprinkler; gutter says `Water: no source` despite barrel water being present.

**After the patch:** In a singleplayer Creative test, user reported `Water: connected` and the sprinkler operating; game screenshots supported the visual result. Quantitative verification of water consumption and increasing farmland moisture was not recorded.

The small source implementation is in [`LtoHodWaterCompatV5.cs`](../LtoHodWaterCompatV5.cs) and is buildable with [`build.ps1`](../build.ps1). No copyrighted DLLs from the upstream mods are committed to the repository.

**Suggested upstream improvement:** Consider a narrower item/liquid provenance check that validates the actual liquid's intended category rather than accepting every namespace carrying `waterportion-*`. Evaluate other callers (including LTO Stamina Plus) when deciding scope.
