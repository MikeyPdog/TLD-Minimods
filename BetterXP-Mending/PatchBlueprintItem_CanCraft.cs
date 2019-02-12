using Harmony;
using UnityEngine;

namespace BetterXPMending
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

            var mendingLevel = GameManager.GetSkillClothingRepair().GetCurrentTierNumber();
            var requiredMendingLevel = MendingHelper.GetRequiredMendingLevel(__instance);
            if (mendingLevel < requiredMendingLevel)
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof (CraftingPage))]
    [HarmonyPatch("RefreshDescriptionText")]
    public class PatchCraftingPageDescriptionText
    {
        const float WhiteColorValue = 0.7843137f;
        private static readonly Color WhiteColor = new Color(WhiteColorValue, WhiteColorValue, WhiteColorValue);
        private static readonly Color RedColor = new Color(0.7f, 0f, 0f);

        private static void Postfix(ref CraftingPage __instance, BlueprintItem ___m_BPI)
        {
            __instance.m_DescriptionLabel.color = WhiteColor;
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

            // Mending levels in code start at 0 rather than 1!
            var mendingLevel = (float)GameManager.GetSkillClothingRepair().GetCurrentTierNumber();
            var requiredMendingLevel = MendingHelper.GetRequiredMendingLevel(___m_BPI);
            var displayMendingLevel = requiredMendingLevel + 1;
            if (mendingLevel < requiredMendingLevel)
            {
                __instance.m_DescriptionLabel.text = "REQUIRES MENDING LEVEL " + displayMendingLevel;
                __instance.m_DescriptionLabel.color = RedColor;
            }
        }
    }

    public class MendingHelper
    {
        public static bool IsClothing(GearItem gearItem)
        {
            if (gearItem == null) return false;
            return gearItem.m_Type == GearTypeEnum.Clothing || gearItem.name == "GEAR_BearSkinBedRoll";
        }

        public static bool IsClothing(BlueprintItem blueprintItem)
        {
            return IsClothing(blueprintItem.m_CraftedResult);
        }

        public static bool IsBandage(GearItem gearItem)
        {
            return gearItem.name == "GEAR_HeavyBandage";
        }
        
        public static void AddMendingXP(int xp)
        {
            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.ClothingRepair, xp, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
        }

        public static int GetRequiredMendingLevel(BlueprintItem blueprintItem)
        {
            var gearItem = blueprintItem.m_CraftedResult;
            if (!IsClothing(gearItem))
            {
                return 0;
            }

            var name = gearItem.name;
            switch (name)
            {
                // These are code levels not ingame levels. So add 1 for ingame levels.
                case "GEAR_RabbitSkinMittens": return 0;
                case "GEAR_RabbitskinHat": return 0;
                case "GEAR_DeerSkinBoots": return 1;
                case "GEAR_DeerSkinPants": return 1;
                case "GEAR_MooseHideBag": return 2;
                case "GEAR_MooseHideCloak": return 2;
                case "GEAR_WolfSkinCape": return 3;
                case "GEAR_BearSkinBedRoll": return 4;
                case "GEAR_BearSkinCoat": return 4;
            }

            return 0;
        }

        public static int GetXpForCrafting(BlueprintItem blueprintItem)
        {
            var gearItem = blueprintItem.m_CraftedResult;

            if (IsBandage(gearItem))
            {
                var roll = Random.Range(1,3);
                if (roll == 3)
                {
                    return 1;
                }
                return 0;
            }

            if (!IsClothing(gearItem))
            {
                return 0;
            }

            var name = gearItem.name;
            switch (name)
            {
                case "GEAR_RabbitSkinMittens": return 3;
                case "GEAR_RabbitskinHat": return 3;
                case "GEAR_DeerSkinBoots": return 4;
                case "GEAR_DeerSkinPants": return 4;
                case "GEAR_MooseHideBag": return 5;
                case "GEAR_MooseHideCloak": return 5;
                case "GEAR_WolfSkinCape": return 7;
                case "GEAR_BearSkinBedRoll": return 8;
                case "GEAR_BearSkinCoat": return 8;
            }

            return 3;
        }
    }

    [HarmonyPatch(typeof(Panel_Crafting))]
    [HarmonyPatch("UpdateSkillAfterCrafting")]
    public class PatchPanel_Crafting
    {
        public static bool Prefix(ref Panel_Crafting __instance, bool success, BlueprintItem ___m_OverrideBPI)
        {
            if (success)
            {
                var selectedBlueprint = ___m_OverrideBPI;
                var xp = MendingHelper.GetXpForCrafting(selectedBlueprint);
                MendingHelper.AddMendingXP(xp);
            }

            return false;
        }
    }
}