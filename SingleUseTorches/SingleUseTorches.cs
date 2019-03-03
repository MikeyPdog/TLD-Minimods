using Harmony;

namespace SingleUseTorches
{
    [HarmonyPatch(typeof(FireStarterItem))]
    [HarmonyPatch("Start")]
    class SingleUseTorches
    {
        static void Postfix(FireStarterItem __instance)
        {
            if (__instance.name == "GEAR_Torch")
            {
                __instance.m_ConditionDegradeOnUse = 100;
            }
        }
    }
}