using Harmony;

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
                baseBurnRate *= 5;
                var calorieBurnPerHour = __instance.CalculateModifiedCalorieBurnRate(baseBurnRate);
                GameManager.GetHungerComponent().SetCalorieBurnPerHour(calorieBurnPerHour);
            }
        }
    }
}