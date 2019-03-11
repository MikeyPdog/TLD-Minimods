using Harmony;
using JsonModSettings;
using ModSettings;

namespace QuickerWoodCutting
{
    [HarmonyPatch(typeof(BreakDown))]
    [HarmonyPatch("Start")]
    class QuickerWoodCutting
    {
        static void Postfix(BreakDown __instance)
        {
            // Branches take 5 mins instead of 10
            if (__instance.m_DisplayName == "Branch")
            {
                __instance.m_TimeCostHours /= 2f;
            }

            // Limbs take 30 mins base (15 mins with hatchet) instead of 90 mins base (45 mins with hatchet)
            if (__instance.m_DisplayName.EndsWith("Limb"))
            {
                __instance.m_TimeCostHours /= 3f;
            }
        }
    }

    internal class QuickerWoodCuttingSettings : JsonModSettingsBase<QuickerWoodCuttingSettings>
    {
        [Name("Speed multiplier when breaking branches")]
        [Description("Speed multiplier when chopping or breaking branches")]
        [Slider(1f, 5f, 9)]
        public float BreakBranchSpeedMultiplier = 2f;

        [Name("Speed multiplier when chopping limbs")]
        [Description("Speed multiplier when chopping limbs")]
        [Slider(1f, 5f, 9)]
        public float BreakLimbSpeedMultiplier = 3f;

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<QuickerWoodCuttingSettings>();
        }
    }
}