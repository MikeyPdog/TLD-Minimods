using Harmony;
using JsonModSettings;
using ModSettings;

namespace ReadWhenHungry
{
    [HarmonyPatch(typeof(Panel_Inventory_Examine))]
    [HarmonyPatch("MaybeAbortReadingWithHUDMessage")]
    class ReadWhenHungry
    {
        // true result means abort reading. False means you're allowed to read
        static bool Prefix(ref bool __result)
        {
            __result = ShouldPreventReading();
            return false; // Don't execute original method afterwards
        }

        static bool ShouldPreventReading()
        {
            var settings = ReadWhenHungrySettings.Instance;
            if (GameManager.GetWeatherComponent().IsTooDarkForAction(ActionsToBlock.Reading))
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooDarkToRead"), false);
                return true;
            }
            if (!settings.allowReadingWhenTired && GameManager.GetFatigueComponent().IsExhausted())
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooTiredToRead"), false);
                return true;
            }
            if (!settings.allowReadingWhenFreezing && GameManager.GetFreezingComponent().IsFreezing())
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooColdToRead"), false);
                return true;
            }
            if (!settings.allowReadingWhenHungry && GameManager.GetHungerComponent().IsStarving())
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooHungryToRead"), false);
                return true;
            }
            if (!settings.allowReadingWhenThirsty && GameManager.GetThirstComponent().IsDehydrated())
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooThirstyToRead"), false);
                return true;
            }
            if (!settings.allowReadingWhenWounded && GameManager.GetConditionComponent().GetNormalizedCondition() < 0.1f)
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TooWoundedToRead"), false);
                return true;
            }
            if (GameManager.GetConditionComponent().HasNonRiskAffliction())
            {
                if (   (!settings.allowReadingWithCabinFever && HasAffliction(AfflictionType.CabinFever))
                    || (!settings.allowReadingWithFoodPoisoning && HasAffliction(AfflictionType.FoodPoisioning))
                    || (!settings.allowReadingWithDysentery && HasAffliction(AfflictionType.Dysentery))
                    || (!settings.allowReadingWithInfection && HasAffliction(AfflictionType.Infection))
                    || (!settings.allowReadingWithHypothermia && HasAffliction(AfflictionType.Hypothermia))
                    || (!settings.allowReadingWithIntestinalParasites && HasAffliction(AfflictionType.IntestinalParasites))

                    || (!settings.allowReadingWithBloodLoss && HasAffliction(AfflictionType.BloodLoss))
                    || (!settings.allowReadingWithBrokenRib && HasAffliction(AfflictionType.BrokenRib))
                    || (!settings.allowReadingWithBurns && HasAffliction(AfflictionType.Burns))
                    || (!settings.allowReadingWithSprainedAnkle && HasAffliction(AfflictionType.SprainedAnkle))
                    || (!settings.allowReadingWithSprainedWrist && HasAffliction(AfflictionType.SprainedWrist)))
                {
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_CannotReadWithAfflictions"), false);
                    return true;
                }
                return false;
            }
            return false;
        }

        private static bool HasAffliction(AfflictionType afflictionType)
        {
            return GameManager.GetConditionComponent().HasSpecificAffliction(afflictionType);
        }
    }

    internal class ReadWhenHungrySettings : JsonModSettingsBase<ReadWhenHungrySettings>
    {
        [Section("Basics")]
        [Name("Allow reading when hungry")]
        [Description("Allow reading when starving hungry (0 calories stored).")]
        public bool allowReadingWhenHungry = true;

        [Name("Allow reading when thirsty")]
        [Description("Allow reading when you are thirsty (0 water drank)")]
        public bool allowReadingWhenThirsty = false;

        [Name("Allow reading when freezing")]
        [Description("Allow reading when you are freezing (empty/red temperature bar)")]
        public bool allowReadingWhenFreezing = false;

        [Name("Allow reading when wounded")]
        [Description("Allow reading when you are below 10% health")]
        public bool allowReadingWhenWounded = false;

        [Name("Allow reading when exhausted")]
        [Description("Allow reading when you are completely exhausted / tired")]
        public bool allowReadingWhenTired = false;

        [Section("Afflictions")]
        [Name("Allow reading with cabin fever")]
        public bool allowReadingWithCabinFever = true;

        [Name("Allow reading with food poisoning")]
        public bool allowReadingWithFoodPoisoning = false;

        [Name("Allow reading with dysentery")]
        public bool allowReadingWithDysentery = false;

        [Name("Allow reading with infection")]
        public bool allowReadingWithInfection = false;

        [Name("Allow reading with hypothermia")]
        public bool allowReadingWithHypothermia = false;

        [Name("Allow reading with intestinal parasites")]
        public bool allowReadingWithIntestinalParasites = true;

        [Name("Allow reading when losing blood")]
        public bool allowReadingWithBloodLoss = false;

        [Name("Allow reading with broken ribs ")]
        public bool allowReadingWithBrokenRib = true;

        [Name("Allow reading with burns")]
        public bool allowReadingWithBurns = false;

        [Name("Allow reading with a sprained ankle")]
        public bool allowReadingWithSprainedAnkle = true;

        [Name("Allow reading with a sprained wrist")]
        public bool allowReadingWithSprainedWrist = true;

        
        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<ReadWhenHungrySettings>();
        }
    }
}