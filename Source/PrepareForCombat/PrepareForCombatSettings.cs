using Verse;

namespace PrepareForCombat;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class PrepareForCombatSettings : ModSettings
{
    public bool combatCapable;
    public float percentPowerAnimals = 1f;
    public float percentWealthBuildings = 1f;
    public float percentWealthItems = 1f;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref combatCapable, "combatCapable");
        Scribe_Values.Look(ref percentWealthBuildings, "percentWealthBuildings", 1f);
        Scribe_Values.Look(ref percentWealthItems, "percentWealthItems", 1f);
        Scribe_Values.Look(ref percentPowerAnimals, "percentPowerAnimals", 1f);
    }
}