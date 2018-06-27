using System.Collections.Generic;
using Harmony;

namespace FrostbiteNotPermanent
{
    [HarmonyPatch(typeof (Frostbite))]
    [HarmonyPatch("FrostbiteStart")]
    internal class PatchFrostbiteNotPermanent
    {
        public static bool Prefix(Frostbite __instance, int location, bool displayIcon, bool nofx = false)
        {
            if (GameManager.GetPlayerManagerComponent().PlayerIsDead())
            {
                return false;
            }
            if (InterfaceManager.m_Panel_ChallengeComplete.IsEnabled())
            {
                return false;
            }
            GameManager.GetConditionComponent().AddHealth(-20f, DamageSource.FrostBite);

            var m_LocationsCurrentFrostbiteDamage = Traverse.Create(__instance).Field("m_LocationsCurrentFrostbiteDamage").GetValue<List<float>>();
            List<float> locationsCurrentFrostbiteDamage = m_LocationsCurrentFrostbiteDamage;
            locationsCurrentFrostbiteDamage[location] *= 0.5f;
            if (!__instance.LocationIsIgnored(location))
            {
                Utils.SendAnalyticsEvent("Affliction", "Frostbite");
                StatsManager.IncrementValue(StatID.Frostbite);
                GameManager.GetLogComponent().AddAffliction(AfflictionType.Frostbite);
            }
            if (displayIcon && !nofx && !__instance.LocationIsIgnored(location))
            {
                PlayerDamageEvent.SpawnDamageEvent(__instance.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_frostbite",
                    InterfaceManager.m_Panel_ActionsRadial.m_FirstAidRedColor, true, InterfaceManager.m_Panel_HUD.m_DamageEventDisplaySeconds,
                    InterfaceManager.m_Panel_HUD.m_DamageEventFadeOutSeconds);
            }
            if (!nofx)
            {
                GameManager.GetPlayerVoiceComponent().PlayCritical(__instance.m_FrostbiteVO);
                GameManager.TriggerSurvivalSaveAndDisplayHUDMessage();
            }

            return false;
        }
    }
}