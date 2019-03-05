using Harmony;
using JsonModSettings;
using ModSettings;

namespace ReadWhenHungry
{
    [HarmonyPatch(typeof(Panel_Inventory_Examine))]
    [HarmonyPatch("MaybeAbortReadingWithHUDMessage")]
    class ReadWhenHungry
    {
        static bool Prefix(ref bool __result)
        {
            __result = CanRead();
            return false; // Don't execute original method afterwards
        }

        static bool CanRead()
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
            if (!settings.allowReadingWhenIll && GameManager.GetConditionComponent().HasNonRiskAffliction())
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_CannotReadWithAfflictions"), false);
                return true;
            }
            return false;
        }
    }

    internal class ReadWhenHungrySettings : JsonModSettingsBase<ReadWhenHungrySettings>
    {
        [Name("Allow reading when hungry")]
        [Description("Allow reading when starving hungry (0 calories stored).")]
        public bool allowReadingWhenHungry = true;

        [Name("Allow reading when ill")]
        [Description("Allow reading when you have an affliction (e.g. intestinal parasites).")]
        public bool allowReadingWhenIll = true;

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

        public static void OnLoad()
        {
            Instance = JsonModSettingsLoader.Load<ReadWhenHungrySettings>();
        }
    }
}