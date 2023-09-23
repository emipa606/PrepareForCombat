using System.Reflection;
using HarmonyLib;
using Mlie;
using UnityEngine;
using Verse;

namespace PrepareForCombat;

[StaticConstructorOnStartup]
internal class PrepareForCombatMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static PrepareForCombatMod instance;

    private static string currentVersion;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public PrepareForCombatMod(ModContentPack content) : base(content)
    {
        instance = this;
        Settings = GetSettings<PrepareForCombatSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        new Harmony("Mlie.PrepareForCombat").PatchAll(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal PrepareForCombatSettings Settings { get; }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Prepare For Combat";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.Label("pfc_percentWealthBuildings_title".Translate(), -1f,
            "pfc_percentWealthBuildings_desc".Translate());
        Settings.percentWealthBuildings =
            listing_Standard.SliderLabeled(Settings.percentWealthBuildings.ToStringPercent(),
                Settings.percentWealthBuildings, 0f, 2f, tooltip: "pfc_percentWealthBuildings_desc".Translate());

        listing_Standard.Gap();
        listing_Standard.Label("pfc_percentWealthItems_title".Translate(), -1f,
            "pfc_percentWealthItems_desc".Translate());
        Settings.percentWealthItems =
            listing_Standard.SliderLabeled(Settings.percentWealthItems.ToStringPercent(),
                Settings.percentWealthItems, 0f, 2f, tooltip: "pfc_percentWealthItems_desc".Translate());

        listing_Standard.Gap();
        listing_Standard.Label("pfc_percentPowerAnimals_title".Translate(), -1f,
            "pfc_percentPowerAnimals_desc".Translate());
        Settings.percentPowerAnimals =
            listing_Standard.SliderLabeled(Settings.percentPowerAnimals.ToStringPercent(),
                Settings.percentPowerAnimals, 0f, 2f, tooltip: "pfc_percentPowerAnimals_desc".Translate());

        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("pfc_combatCapable_title".Translate(), ref Settings.combatCapable,
            "pfc_combatCapable_desc".Translate());

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("pfc_modversion_title".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }
}