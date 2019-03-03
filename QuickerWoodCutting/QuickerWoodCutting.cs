using Harmony;

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
                __instance.m_TimeCostHours = 0.0833f;
            }

            // Limbs take 30 mins base (15 mins with hatchet) instead of 90 mins base (45 mins with hatchet)
            if (__instance.m_DisplayName.EndsWith("Limb"))
            {
                __instance.m_TimeCostHours = 0.5f;
            }
        }
    }
}