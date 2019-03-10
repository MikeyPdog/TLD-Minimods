using Harmony;
using JsonModSettings;
using ModSettings;

namespace SprintCostsFood
{
    [HarmonyPatch(typeof(PlayerManager))]
    [HarmonyPatch("UpdateCalorieBurnRate")]
    class PatchSprintingCalorieBurnRate
    {
        static void Postfix(PlayerManager __instance)
        {
            if (__instance.PlayerIsSprinting())
            {
                var baseBurnRate = GameManager.GetHungerComponent().m_CalorieBurnPerHourSprinting * GameManager.GetFeatFreeRunner().GetSprintCalorieBurnScale();
                baseBurnRate *= SprintCostsFoodSettings.Instance.CalorieBurnRateMultiplier;
                var calorieBurnPerHour = __instance.CalculateModifiedCalorieBurnRate(baseBurnRate);
                GameManager.GetHungerComponent().SetCalorieBurnPerHour(calorieBurnPerHour);
            }
        }
    }

    internal class SprintCostsFoodSettings : JsonModSettingsBase<SprintCostsFoodSettings>
    {
        [Name("Food usage multiplier when sprinting")]
        [Description("Food usage multiplier when sprinting")]
        [Slider(1f, 5f, 9)]
        public float CalorieBurnRateMultiplier = 5f;

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<SprintCostsFoodSettings>();
        }
    }
}