using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using MissionTypes;
using ModSettings;
using UnityEngine;
using JsonModSettings;

namespace CustomWhiteout
{
    [HarmonyPatch(typeof(Action_WhiteoutGearRequirements))]
    [HarmonyPatch("OnExecute")]
    class WhiteoutGear
    {
        private static int iteration = 0;
        private static Dictionary<WhiteoutItem, string> gearItemNamesByWhiteoutItem; 

        private static void Postfix(Action_WhiteoutGearRequirements __instance)
        {
            PopulateDictionary();
            var settings = WhiteoutGearSettings.Instance;
            __instance.daysOfFoodRequired = settings.daysOfFoodRequired;
            __instance.numLitersPotableWater = settings.litersWaterRequired;
            __instance.numLitersKerosene = settings.litersKeroseneRequired;

//            Debug.Log("[WHITEOUT] Required items: " + __instance.requiredItemsList.value.Count);
//            SetRequirement("GEAR_Softwood,GEAR_Hardwood", settings.hardSoftWoodRequired, __instance);
//            SetRequirement("GEAR_BearSkinBedRoll", settings.bearSkillBedrollRequired, __instance, "Bearskin Bedroll");
//            Debug.Log("[WHITEOUT] Required items now: " + __instance.requiredItemsList.value.Count);

            // CAN ONLY SHOW 2 MORE ITEMS - 15 total
            // How do I do this?
            // Item1....6 for just the items
            // Item1 amount

            /* Normal stuff
             * days of food     - Find/hunt             Keep
             * Water            - Make on fire          Maybe
             * Lamp oil         - Fishing / find        Keep
             * Soft/hardwood    - Find/chop             Keep
             * Reclaimed wood   - Break down            Keep
             * Sticks           - Find in woods         Maybe not
             * Tinder           - Pointless             No
             * Bandages         - Easy, get cloth       Maybe
             * Matches          - Find in houses        Maybe
             * Rifle            - Find in houses        Maybe
             * Rifle cartidges  - Find in houses        Maybe
             * Hatchet          - Find/create           Maybe
             * Lantern          - Find in houses        Maybe
             */

            /* SINGLE ITEMS
             * Rifle
             * Hatchet
             * Lantern
             */

            /* Items single or stacked
             * 50x Sticks
             * 30x reclaimed wood
             * 20x soft/hard wood
             * 25x tinder
             * 25x Matches
             * 10x Bandages
             * 10x rifle cartridges
             * 1x Rifle
             * 1x Hatchet
             * 1x Lantern
             * 
             */

            /* Ideas
             * Clothing (bearskin etc)
             * Rose hip / mushroom tea
             * Birch bark
             * Climbing ropes
             * 
             */

            // __instance.requiredItemsHeaderList;
            ClearItemRequirements(__instance);
            AddRequirement(settings.item1, settings.item1amount,  __instance);
            AddRequirement(settings.item2, settings.item2amount,  __instance);
            AddRequirement(settings.item3, settings.item3amount,  __instance);
            AddRequirement(settings.item4, settings.item4amount,  __instance);
            AddRequirement(settings.item5, settings.item5amount,  __instance);
            AddRequirement(settings.item6, settings.item6amount,  __instance);
            AddRequirement(settings.item7, settings.item7amount,  __instance);
            AddRequirement(settings.item8, settings.item8amount,  __instance);
            AddRequirement(settings.item9, settings.item9amount,  __instance);
            AddRequirement(settings.item10, settings.item10amount,  __instance);
            AddRequirement(settings.item11, settings.item11amount,  __instance);
            AddRequirement(settings.item12, settings.item11amount,  __instance);

//            SetRequirement("GEAR_WolfSkinCape", true, __instance, "Wolfskin Coat");
//            SetRequirement("GEAR_DeerSkinBoots", true, __instance, "Deerskin Boots");
//            SetRequirement("GEAR_DeerSkinPants", true, __instance, "Deerskin Pants");
//
//
//            SetRequirement("GEAR_RabbitSkinMittens", true, __instance, "Rabbitskin Mittens");
//            SetRequirement("GEAR_RabbitskinHat", true, __instance, "Rabbitskin Hat");
//            SetRequirement("GEAR_MooseHideBag", true, __instance, "Moosehide bag");
//            SetRequirement("GEAR_Bow", true, __instance, "Bow");
//            SetRequirement("GEAR_Arrow", 30, __instance, "Arrows");

        }

        private static void PopulateDictionary()
        {
            gearItemNamesByWhiteoutItem = new Dictionary<WhiteoutItem, string>();
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Arrows, "GEAR_Arrow");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Bandages, "GEAR_HeavyBandage,GEAR_OldMansBeardDressing");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Bearskin_Bedroll, "GEAR_BearSkinBedRoll");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Birch_Bark, "GEAR_BarkTinder");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Bow, "GEAR_Bow");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Deerskin_Boots, "GEAR_DeerskinBoots");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Deerskin_Pants, "GEAR_DeerskinPants");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Hatchet, "GEAR_Hatchet,GEAR_HatchetImprovised");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Lantern, "GEAR_KeroseneLampB");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Matches, "GEAR_PackMatches,GEAR_WoodMatches");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Moose_Hide_Bag, "GEAR_MooseHideBag");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Moose_Hide_Cloak, "GEAR_MooseHideCloak");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Rabbitskin_Hat, "GEAR_RabbitskinHat");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Rabbitskin_Mittens, "GEAR_RabbitSkinMittens");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Reclaimed_Wood, "GEAR_ReclaimedWoodB");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Reishi_Tea, "GEAR_ReishiTea");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Rifle, "GEAR_Rifle");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Rifle_Cartridges, "GEAR_RifleAmmoBox,GEAR_RifleAmmoSingle");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Rosehip_Tea, "GEAR_RoseHipTea");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Soft_or_Hard_Wood, "GEAR_Softwood,GEAR_Hardwood");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Stick, "GEAR_Stick");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Tinder, "GEAR_NewsprintRoll,GEAR_PaperStack,GEAR_Newsprint,GEAR_CashBundle,GEAR_BarkTinder,GEAR_Tinder,GEAR_CattailTinder");
            gearItemNamesByWhiteoutItem.Add(WhiteoutItem.Wolfskin_Coat, "GEAR_WolfSkinCape");
        }

        private static void AddRequirement(WhiteoutItem item, int numberRequired, Action_WhiteoutGearRequirements instance)
        {
            if (item == WhiteoutItem.None)
            {
                return;
            }

            var displayName = item.ToString().Replace('_', ' ');
            var gearName = gearItemNamesByWhiteoutItem[item];
            SetRequirement(gearName, numberRequired, instance, displayName);
        }

        private static void ClearItemRequirements(Action_WhiteoutGearRequirements instance)
        {

            for (int i = 0; i < instance.requiredItemsList.value.Count; i++)
            {
                Debug.Log("[WHITEOUT] Required items: " + instance.requiredItemsList.value[i].name + " (" + instance.requiredItemsList.value[i].amount + ")");
                Debug.Log("[WHITEOUT] Header: " + instance.requiredItemsHeaderList[i]);
            }

            instance.requiredItemsList.value.Clear();
            instance.requiredItemsHeaderList.Clear();
        }

        private static void SetRequirement(string gearName, bool required, Action_WhiteoutGearRequirements instance, string journalDisplayName = null)
        {
            SetRequirement(gearName, required ? 1 : 0, instance, journalDisplayName);
        }

        private static void SetRequirement(string gearName, int numberRequired, Action_WhiteoutGearRequirements instance, string journalDisplayName = null)
        {
            var itemRequirements = instance.requiredItemsList.value;
            var displayNames = instance.requiredItemsHeaderList;
            var itemRequirement = itemRequirements.FirstOrDefault(i => i.name == gearName);
            if (itemRequirement != null)
            {
                if (numberRequired == 0)
                {
                    var index = itemRequirements.IndexOf(itemRequirement);
                    itemRequirements.RemoveAt(index);
                    displayNames.RemoveAt(index);
                }
                else
                {
                    itemRequirement.amount = numberRequired;
                }
            }
            else
            {
                if (numberRequired == 0)
                {
                    return;
                }

                // TODO: Condition required?
                var newItemRequirement = new InventoryItemRequirement(gearName, numberRequired, false, null, null);
                itemRequirements.Add(newItemRequirement);
                displayNames.Add(journalDisplayName);
            }
        }
    }

    internal class WhiteoutGearSettings : JsonModSettingsBase<WhiteoutGearSettings>
    {
        [Name("Days of food required")]
        [Description("Days of food required (default 15)")]
        [Slider(5, 60, 12)]
        public int daysOfFoodRequired = 15;

        [Name("Liters of water required")]
        [Description("Liters of water required (default 25)")]
        [Slider(10, 100, 19)]
        public int litersWaterRequired = 25;

        [Name("Liters of kerosene required")]
        [Description("Liters of kerosene (lamp oil) required (default 5)")]
        [Slider(1, 20)]
        public int litersKeroseneRequired = 5;

//        [Name("Require Bearskin Bedroll")]
//        [Description("Require Bearskin Bedroll")]
//        public bool bearSkillBedrollRequired = false;
//
//        [Name("Hard/soft wood required")]
//        [Description("Pieces of hard/soft wood (fir / cedar) required (default 20)")]
//        [Slider(5, 100, 20)]
//        public int hardSoftWoodRequired = 20;


        [Name("Item 1")]
        public WhiteoutItem item1 = WhiteoutItem.Reclaimed_Wood;

        [Name("Item 1 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item1amount = 30;


        [Name("Item 2")]
        public WhiteoutItem item2 = WhiteoutItem.Stick;

        [Name("Item 2 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item2amount = 50;


        [Name("Item 3")]
        public WhiteoutItem item3 = WhiteoutItem.Tinder;

        [Name("Item 3 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item3amount = 25;


        [Name("Item 4")]
        public WhiteoutItem item4 = WhiteoutItem.Bandages;

        [Name("Item 4 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item4amount = 10;


        [Name("Item 5")]
        public WhiteoutItem item5 = WhiteoutItem.Matches;

        [Name("Item 5 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item5amount = 25;


        [Name("Item 6")]
        public WhiteoutItem item6 = WhiteoutItem.Rifle;

        [Name("Item 6 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item6amount = 1;


        [Name("Item 7")]
        public WhiteoutItem item7 = WhiteoutItem.Rifle_Cartridges;

        [Name("Item 7 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item7amount = 10;


        [Name("Item 8")]
        public WhiteoutItem item8 = WhiteoutItem.Hatchet;

        [Name("Item 8 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item8amount = 1;


        [Name("Item 9")]
        public WhiteoutItem item9 = WhiteoutItem.Lantern;

        [Name("Item 9 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item9amount = 1;


        [Name("Item 10")]
        public WhiteoutItem item10 = WhiteoutItem.None;

        [Name("Item 10 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item10amount = 1;


        [Name("Item 11")]
        public WhiteoutItem item11 = WhiteoutItem.None;

        [Name("Item 11 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item11amount = 1;

        [Name("Item 12")]
        public WhiteoutItem item12 = WhiteoutItem.None;

        [Name("Item 12 Number required")]
        [Description("Number of this item required")]
        [Slider(1, 100)]
        public int item12amount = 1;

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<WhiteoutGearSettings>();
        }
    }

    internal enum WhiteoutItem
    {
        None,
        Arrows,
        Bandages,
        Bow,
        Bearskin_Bedroll,
        Birch_Bark,
        Deerskin_Pants,
        Deerskin_Boots,
        Hatchet,
        Lantern,
        Matches,
        Moose_Hide_Bag,
        Moose_Hide_Cloak,
        Rabbitskin_Mittens,
        Rabbitskin_Hat,
        Reclaimed_Wood,
        Reishi_Tea,
        Rifle,
        Rifle_Cartridges,
        Rosehip_Tea,
        Soft_or_Hard_Wood,
        Stick,
        Tinder,
        Wolfskin_Coat,
    }
        /*              * 50x Sticks
             * 30x reclaimed wood
             * 20x soft/hard wood
             * 25x tinder
             * 25x Matches
             * 10x Bandages
             * 10x rifle cartridges
             * 1x Rifle
             * 1x Hatchet
             * 1x Lantern
         */

    // First:
    // TODO: Change list of gear to collect

    // ALSO: Fix bugs
    // - DONE Missing tinder items (paper stack, cash bundle)
    // - missing firelog t

    // ALSO: Change target location - random or preset
    // ALSO: Change start location, random or preset

    // Second:
    // TODO: Change difficulty
    // Difficulty on a per challenge basis?
}