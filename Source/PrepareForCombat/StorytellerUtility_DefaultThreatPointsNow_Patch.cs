using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PrepareForCombat;

[HarmonyPatch(typeof(StorytellerUtility), "DefaultThreatPointsNow")]
internal static class StorytellerUtility_DefaultThreatPointsNow_Patch
{
    private const float GlobalMinPoints = 35f;

    private const float GlobalMaxPoints = 10000f;

    private static readonly SimpleCurve PointsPerWealthCurve;

    private static readonly SimpleCurve PointsPerColonistByWealthCurve;

    static StorytellerUtility_DefaultThreatPointsNow_Patch()
    {
        PointsPerWealthCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0f),
            new CurvePoint(14000f, 0f),
            new CurvePoint(400000f, 2400f),
            new CurvePoint(700000f, 3600f),
            new CurvePoint(1000000f, 4200f)
        };
        PointsPerColonistByWealthCurve = new SimpleCurve
        {
            new CurvePoint(0f, 15f),
            new CurvePoint(10000f, 15f),
            new CurvePoint(400000f, 140f),
            new CurvePoint(1000000f, 200f)
        };
    }

    [HarmonyPrefix]
    public static bool Patch_StorytellerUtility_DefaultThreatPointsNow(IIncidentTarget target, ref float __result)
    {
        var playerWealthForStoryteller = target.PlayerWealthForStoryteller;
        var num = PointsPerWealthCurve.Evaluate(playerWealthForStoryteller);
        var num2 = 0f;
        foreach (var item in target.PlayerPawnsForStoryteller)
        {
            if (PrepareForCombatMod.instance.Settings.combatCapable &&
                item.ParentHolder is Building_CryptosleepCasket ||
                item.IsQuestLodger())
            {
                continue;
            }

            var num3 = 0f;
            if (item.IsFreeColonist && !PrepareForCombatMod.instance.Settings.combatCapable || item.IsFreeColonist &&
                PrepareForCombatMod.instance.Settings.combatCapable && !item.WorkTagIsDisabled(WorkTags.Violent) &&
                item.health.capacities.GetLevel(PawnCapacityDefOf.Moving) >= 0.15)
            {
                num3 = PointsPerColonistByWealthCurve.Evaluate(playerWealthForStoryteller);
            }
            else if (item.RaceProps.Animal && item.Faction == Faction.OfPlayer && !item.Downed &&
                     item.training.CanAssignToTrain(TrainableDefOf.Release).Accepted &&
                     !PrepareForCombatMod.instance.Settings.combatCapable || item.RaceProps.Animal &&
                     item.Faction == Faction.OfPlayer && !item.Downed &&
                     item.training.HasLearned(TrainableDefOf.Release) &&
                     PrepareForCombatMod.instance.Settings.combatCapable &&
                     item.health.capacities.GetLevel(PawnCapacityDefOf.Moving) >= 0.15)
            {
                num3 = 0.08f * item.kindDef.combatPower * PrepareForCombatMod.instance.Settings.percentPowerAnimals;
                if (target is Caravan)
                {
                    num3 *= 0.7f;
                }
            }

            if (!(num3 > 0f))
            {
                continue;
            }

            if (item.ParentHolder is Building_CryptosleepCasket)
            {
                num3 *= 0.3f;
            }

            num3 = Mathf.Lerp(num3, num3 * item.health.summaryHealth.SummaryHealthPercent, 0.65f);
            num2 += num3;
        }

        num += num2;
        var totalThreatPointsFactor = Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor;
        num *= Mathf.Lerp(1f, totalThreatPointsFactor, Find.Storyteller.difficulty.adaptationEffectFactor);
        num *= target.IncidentPointsRandomFactorRange.RandomInRange;
        num *= Find.Storyteller.difficulty.threatScale;
        num *= Find.Storyteller.def.pointsFactorFromDaysPassed.Evaluate(GenDate.DaysPassed);
        __result = Mathf.Clamp(num, GlobalMinPoints, GlobalMaxPoints);
        return false;
    }
}