using Harmony;

namespace QuickerMapping
{
    [HarmonyPatch(typeof(CharcoalItem))]
    [HarmonyPatch("Awake")]
    class PatchCharcoalMappingTime
    {
        static void Postfix(CharcoalItem __instance)
        {
            __instance.m_SurveyGameMinutes = 1;
        }
    }
}