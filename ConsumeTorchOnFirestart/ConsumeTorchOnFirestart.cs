using Harmony;

namespace ConsumeTorchOnFirestart
{
    [HarmonyPatch(typeof(FireStarterItem))]
    [HarmonyPatch("Start")]
    class ConsumeTorchOnFirestart
    {
        static void Postfix(FireStarterItem __instance)
        {
            if (__instance.name == "GEAR_Torch")
            {
                __instance.m_ConditionDegradeOnUse = 100;
                __instance.m_ConsumeOnUse = true;
            }
        }
    }
}