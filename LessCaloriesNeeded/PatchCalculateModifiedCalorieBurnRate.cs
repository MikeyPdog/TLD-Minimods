using Harmony;

namespace LessCaloriesNeeded
{
    [HarmonyPatch(typeof(PlayerManager))]
    [HarmonyPatch("CalculateModifiedCalorieBurnRate")]
    class PatchCalculateModifiedCalorieBurnRate
    {
        static void Postfix(ref float __result)
        {
            __result *= 0.5f;
        }
    }
}