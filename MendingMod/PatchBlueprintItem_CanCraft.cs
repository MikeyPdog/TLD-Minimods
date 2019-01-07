using Harmony;
using UnityEngine;

namespace MendingMod
{
    [HarmonyPatch(typeof(BlueprintItem))]
    [HarmonyPatch("CanCraft")]
    class PatchBlueprintItem_CanCraft
    {
        static bool Postfix(bool __result, ref BlueprintItem __instance)
        {
            if (__result == false)
            {
                return false;
            }

            var mendingLevel = (float)GameManager.GetSkillClothingRepair().GetCurrentTierNumber();
            var gearItem = __instance.m_CraftedResult;

//            Debug.Log("[MendingMod] mendingLevel:" + mendingLevel);
//            Debug.Log("[MendingMod] Tostring:" + gearItem.ToString());
//            Debug.Log("[MendingMod] Display name:" + gearItem.m_DisplayName);
//            Debug.Log("[MendingMod] type:" + gearItem.m_Type);
//            Debug.Log("[MendingMod] name:" + gearItem.name + " type:" + gearItem.m_Type);


            // type:Clothing for most, not the bedroll
            // name:GEAR_BearSkinBedRoll
            // name:GEAR_WolfSkinCape
            // name:GEAR_BearSkinCoat
            // name:GEAR_MooseHideCloak
            // name:GEAR_MooseHideBag
            // name:GEAR_RabbitSkinMittens
            // name:GEAR_RabbitskinHat
            // name:GEAR_DeerSkinBoots
            // name:GEAR_DeerSkinPants

            var isClothing = gearItem.m_Type == GearTypeEnum.Clothing || gearItem.name == "GEAR_BearSkinBedRoll";
            // Mendinglevel is 0 in code when its 1 in game
            if (mendingLevel < 2 && isClothing) 
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof (CraftingPage))]
    [HarmonyPatch("RefreshDescriptionText")]
    public class Something
    {
        private static void Postfix(ref CraftingPage __instance, BlueprintItem ___m_BPI)
        {
            if (!___m_BPI)
            {
                return;
            }
            if (!___m_BPI.m_CraftedResult)
            {
                return;
            }

            if (!MendingHelper.IsClothing(___m_BPI))
            {
                return;
            }

            var mendingLevel = (float)GameManager.GetSkillClothingRepair().GetCurrentTierNumber();
            if (mendingLevel < 3)
            {
                __instance.m_DescriptionLabel.text = "UNLOCKED AT MENDING LEVEL 3";
            }
        }
    }

    public class MendingHelper
    {
        public static bool IsClothing(GearItem gearItem)
        {
            return gearItem.m_Type == GearTypeEnum.Clothing || gearItem.name == "GEAR_BearSkinBedRoll";
        }

        public static bool IsClothing(BlueprintItem blueprintItem)
        {
            var gearItem = blueprintItem.m_CraftedResult;
            return IsClothing(gearItem);
        }
    }
}