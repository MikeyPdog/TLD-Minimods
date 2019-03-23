using Harmony;
using ModSettings;
using UnityEngine;
using JsonModSettings;

namespace CustomWhiteout
{
    [HarmonyPatch(typeof(Action_WhiteoutGearRequirements))]
    [HarmonyPatch("OnExecute")]
    class WhiteoutGear
    {
        private static int iteration = 0;
        static void Postfix(Action_WhiteoutGearRequirements __instance)
        {
            __instance.daysOfFoodRequired = WhiteoutGearSettings.Instance.daysOfFoodRequired;
            __instance.numLitersPotableWater = WhiteoutGearSettings.Instance.litersWaterRequired;
            __instance.numLitersKerosene = WhiteoutGearSettings.Instance.litersKeroseneRequired;
            iteration++;
            if (iteration < 100)
            {
                Debug.Log("[WHITEOUT] OnExecute called: " + iteration);
            }
        }
    }

    internal class WhiteoutGearSettings : JsonModSettingsBase<WhiteoutGearSettings>
    {
        [Name("Days of food required")]
        [Description("Days of food required (default 15)")]
        [Slider(1, 60)]
        public int daysOfFoodRequired = 15;

        [Name("Liters of water required")]
        [Description("Liters of water required (default 25)")]
        [Slider(1, 100)]
        public int litersWaterRequired = 25;

        [Name("Liters of kerosene required")]
        [Description("Liters of kerosene (lamp oil) required (default 5)")]
        [Slider(1, 20)]
        public int litersKeroseneRequired = 5;

        // How does hardwood / softwood work?
        // Hatchet works with impr. hatchet (good!)

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<WhiteoutGearSettings>();
        }
    }

    // First:
    // TODO: Change list of gear to collect

    // - List gear 
    // - Allow loading of the gear
    // Action_WhiteoutGearRequirements

    // - There are many different ways of tracking the requirements
    // - this.daysOfFoodRequired
    // - this.numLitersPotableWater
    // - this.numLitersKerosene
    // - this.requiredItemsList -> List<InventoryItemRequirement> (name, amount, GearItem

    // Second:
    // TODO: Change difficulty
    // Difficulty on a per challenge basis?
}