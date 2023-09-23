using HarmonyLib;
using RimWorld;
using Verse;

namespace PrepareForCombat;

[HarmonyPatch(typeof(Map), "PlayerWealthForStoryteller", MethodType.Getter)]
internal static class Map_PlayerWealthForStoryteller_Patch
{
    [HarmonyPrefix]
    public static bool Patch_Map_PlayerWealthForStoryteller(Map __instance, ref float __result)
    {
        if (!__instance.IsPlayerHome)
        {
            var num = 0f;
            foreach (var item in __instance.mapPawns.PawnsInFaction(Faction.OfPlayer))
            {
                if (item.IsFreeColonist)
                {
                    num += WealthWatcher.GetEquipmentApparelAndInventoryWealth(item);
                }

                if (item.RaceProps.Animal)
                {
                    num += item.MarketValue;
                }
            }

            __result = num;
            return false;
        }

        if (Find.Storyteller.difficulty.fixedWealthMode)
        {
            __result = StorytellerUtility.FixedWealthModeMapWealthFromTimeCurve.Evaluate(__instance.AgeInDays *
                Find.Storyteller.difficulty.fixedWealthTimeFactor);
            return false;
        }

        __result = (PrepareForCombatMod.instance.Settings.percentWealthItems * __instance.wealthWatcher.WealthItems) +
                   (PrepareForCombatMod.instance.Settings.percentWealthBuildings *
                    __instance.wealthWatcher.WealthBuildings *
                    0.5f) + __instance.wealthWatcher.WealthPawns;

        return false;
    }
}