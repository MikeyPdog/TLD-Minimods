using Harmony;
using JsonModSettings;
using ModSettings;

namespace StarvingHurts
{
    [HarmonyPatch(typeof(Condition))]
    [HarmonyPatch("Start")]
    class PatchStarvation
    {
        static void Postfix(Condition __instance)
        {
            var m_StartHasBeenCalled = Traverse.Create(__instance).Field("m_StartHasBeenCalled").GetValue<bool>();
            if (m_StartHasBeenCalled)
            {
                __instance.m_HPDecreasePerDayFromStarving *= StarvingHurtsSettings.Instance.StarvationDamageMultiplier;
            }
        }
    }

    internal class StarvingHurtsSettings : JsonModSettingsBase<StarvingHurtsSettings>
    {
        [Name("Damage multiplier when starving")]
        [Description("Damage multiplier when starving")]
        [Slider(1f, 5f, 9)]
        public float StarvationDamageMultiplier = 5f;

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<StarvingHurtsSettings>();
        }
    }
}