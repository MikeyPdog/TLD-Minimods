using Harmony;

namespace SprintCostsFood
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
                __instance.m_HPDecreasePerDayFromStarving *= 5;
            }
        }
    }
}