using Harmony;

namespace LessEffectiveRepairs
{
    [HarmonyPatch(typeof(Skill_ClothingRepair))]
    [HarmonyPatch("GetItemConditionScale")]
    class PatchClothingRepairMultiplier
    {
        static void Postfix(Skill_ClothingRepair __instance, ref float __result)
        {
            __result *= 0.5f;
        }
    }
}