using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LtoHodWaterCompatV5;

/// <summary>
/// Experimental server-side bridge for LTOLibPlus 5.0.0 and HoD 2.5.8.
/// It does not modify either original mod file.
/// </summary>
public sealed class WaterCompatModSystem : ModSystem
{
    private const string HarmonyId = "ltohodwatercompat.v5";
    private Harmony? harmony;

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        try
        {
            Assembly? libAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a =>
                string.Equals(a.GetName().Name, "LTOLibPlus", StringComparison.OrdinalIgnoreCase));
            if (libAssembly is null)
                throw new InvalidOperationException("LTOLibPlus assembly was not loaded.");

            Type? validator = libAssembly.GetType("LTOLibPlus.Water.LtoPlainWater", throwOnError: false);
            if (validator is null)
                throw new InvalidOperationException("LTOLibPlus.Water.LtoPlainWater was not found.");

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo[] candidates = validator.GetMethods(flags).Where(m =>
                m.Name == "IsPlainWater" &&
                m.ReturnType == typeof(bool) &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(ItemStack)).ToArray();

            if (candidates.Length != 1)
                throw new InvalidOperationException($"Expected one bool IsPlainWater(ItemStack), found {candidates.Length}. " +
                    "This build only targets LTOLibPlus 5.0.0.");

            var postfixMethod = typeof(PlainWaterPostfix).GetMethod(
                nameof(PlainWaterPostfix.Postfix), BindingFlags.Public | BindingFlags.Static);
            if (postfixMethod is null)
                throw new InvalidOperationException("Could not locate our Harmony postfix method.");

            harmony = new Harmony(HarmonyId);
            harmony.Patch(candidates[0], postfix: new HarmonyMethod(postfixMethod));
            api.Logger.Notification("[LTO HoD Water Compat V5] ACTIVE: Postfix applied to LTOLibPlus.Water.LtoPlainWater.IsPlainWater(ItemStack).");
        }
        catch (Exception ex)
        {
            harmony?.UnpatchAll(HarmonyId);
            harmony = null;
            api.Logger.Error("[LTO HoD Water Compat V5] NOT ACTIVE: " + ex);
        }
    }

    public override void Dispose()
    {
        harmony?.UnpatchAll(HarmonyId);
        harmony = null;
        base.Dispose();
    }
}

/// <summary>
/// Leaves LTO's native 'true' results alone. Only upgrades a 'false' result
/// for item stacks whose item codes denote water portions. No juice or milk.
/// </summary>
public static class PlainWaterPostfix
{
    // Harmony's __0 binds to the first argument regardless of its source name.
    public static void Postfix(ItemStack? __0, ref bool __result)
    {
        if (__result) return;
        string? path = __0?.Collectible?.Code?.Path;
        if (path is null) return;

        if (path.Equals("waterportion", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("waterportion-", StringComparison.OrdinalIgnoreCase))
        {
            __result = true;
        }
    }
}
