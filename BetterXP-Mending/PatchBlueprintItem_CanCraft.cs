using Harmony;
using JsonModSettings;
using ModSettings;
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

            var mendingLevel = MendingHelper.GetCurrentMendingLevel();
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

            var mendingLevel = MendingHelper.GetCurrentMendingLevel();
            var requiredMendingLevel = MendingHelper.GetRequiredMendingLevel(___m_BPI);
            if (mendingLevel < requiredMendingLevel)
            {
                __instance.m_DescriptionLabel.text = "REQUIRES MENDING LEVEL " + requiredMendingLevel;
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
            return IsBandage(gearItem.name);
        }

        public static bool IsBandage(string itemName)
        {
            return itemName == "GEAR_HeavyBandage";
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
            var settings = BetterXPMendingSettings.Instance;

            if (name.StartsWith("GEAR_Rabbit"))
                return settings.RabbitLevel;

            if (name.StartsWith("GEAR_Deer"))
                return settings.DeerLevel;

            if (name.StartsWith("GEAR_Moose"))
                return settings.MooseLevel;

            if (name.StartsWith("GEAR_Wolf"))
                return settings.WolfLevel;

            if (name.StartsWith("GEAR_Bear"))
                return settings.BearLevel;

            return 0;
        }

        public static int GetXpForCrafting(string itemName)
        {
            var settings = BetterXPMendingSettings.Instance;
            if (IsBandage(itemName) && settings.CraftingBandagesGivesXP)
            {
                var roll = Random.Range(1, 3);
                if (roll == 3)
                {
                    return 1;
                }
                return 0;
            }

            if (itemName.StartsWith("GEAR_Rabbit"))
                return settings.RabbitXp;

            if (itemName.StartsWith("GEAR_Deer"))
                return settings.DeerXp;

            if (itemName.StartsWith("GEAR_Moose"))
                return settings.MooseXp;

            if (itemName.StartsWith("GEAR_Wolf"))
                return settings.WolfXp;

            if (itemName.StartsWith("GEAR_Bear"))
                return settings.BearXp;

            return 0;
        }

        public static int GetCurrentMendingLevel()
        {
            return GameManager.GetSkillClothingRepair().GetCurrentTierNumber() + 1;
        }
    }

    [HarmonyPatch(typeof(AchievementManager))]
    [HarmonyPatch("CraftedItem")]
    public class PatchCrafteditem
    {
        public static void Prefix(string itemName)
        {
            Debug.Log("[MENDING] Crafted " + itemName);
            var xp = MendingHelper.GetXpForCrafting(itemName);
            if (xp > 0)
            {
                MendingHelper.AddMendingXP(xp);
            }
        }
    }


    internal class BetterXPMendingSettings : JsonModSettingsBase<BetterXPMendingSettings>
    {
        [Name("Crafting bandages sometimes give XP")]
        [Description("Crafting bandages have a 33% chance of giving mending experience.")]
        public bool CraftingBandagesGivesXP = true;

        [Name("Level required for crafting Rabbit Skin items")]
        [Description("Mending experience level required for crafting Rabbit Skin items")]
        [Slider(1, 5)]
        public int RabbitLevel = 1;

        [Name("Level required for crafting Deer Skin items")]
        [Description("Mending experience level required for crafting Deer Skin items")]
        [Slider(1, 5)]
        public int DeerLevel = 2;

        [Name("Level required for crafting Moose Hide items")]
        [Description("Mending experience level required for crafting Moose Hide items")]
        [Slider(1, 5)]
        public int MooseLevel = 3;

        [Name("Level required for crafting Wolfskin items")]
        [Description("Mending experience level required for crafting Wolfskin items")]
        [Slider(1, 5)]
        public int WolfLevel = 4;

        [Name("Level required for crafting Bearskin items")]
        [Description("Mending experience level required for crafting Bearskin items")]
        [Slider(1, 5)]
        public int BearLevel = 5;

        [Name("XP for crafting Rabbit Skin items")]
        [Description("Mending experience points for crafting Rabbit Skin items")]
        [Slider(0, 10)]
        public int RabbitXp = 3;

        [Name("XP for crafting Deer Hide items")]
        [Description("Mending experience points for crafting Deer Hide items")]
        [Slider(0, 10)]
        public int DeerXp = 4;

        [Name("XP for crafting Moose Hide items")]
        [Description("Mending experience points for crafting Moose Hide items")]
        [Slider(0, 10)]
        public int MooseXp = 5;

        [Name("XP for crafting Wolfskin items")]
        [Description("Mending experience points for crafting Wolfskin items")]
        [Slider(0, 10)]
        public int WolfXp = 7;

        [Name("XP for crafting Bearskin items")]
        [Description("Mending experience points for crafting Bearskin items")]
        [Slider(0, 10)]
        public int BearXp = 8;


        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<BetterXPMendingSettings>();
        }
    }
}