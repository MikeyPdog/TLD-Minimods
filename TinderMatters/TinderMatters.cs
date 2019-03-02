using Harmony;
using UnityEngine;

namespace TinderMatters
{
    [HarmonyPatch(typeof(Panel_FireStart))]
    [HarmonyPatch("OnStartFire")]
    static class TinderMatters
    {
        static bool Prefix(Panel_FireStart __instance, Campfire ___m_CampFireInstance, FireStarterItem ___m_KeroseneAccelerant, FireManager ___m_FireManager, bool skipResearchItemCheck = false)
        {
            var traverseInstance = Traverse.Create(__instance);
            traverseInstance.Field("m_FireManager").SetValue(GameManager.GetFireManagerComponent());

            var selectedFireStarter = traverseInstance.Method("GetSelectedFireStarter").GetValue<FireStarterItem>();
            var selectedTinder = traverseInstance.Method("GetSelectedTinder").GetValue<FuelSourceItem>();
            var selectedFuelSource = traverseInstance.Method("GetSelectedFuelSource").GetValue<FuelSourceItem>();
            var selectedAccelerant = traverseInstance.Method("GetSelectedAccelerant").GetValue<FireStarterItem>();

            if (___m_CampFireInstance && ___m_CampFireInstance.TooWindyToStart())
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Itstoowindytostartafirethere"), false);
                return false;
            }
            if (selectedFireStarter == null || (GameManager.GetSkillFireStarting().TinderRequired() && selectedTinder == null) || selectedFuelSource == null)
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Insufficientmaterialstocreatefire"), false);
                return false;
            }

            if (selectedFireStarter.m_RequiresSunLight && !traverseInstance.Method("HasDirectSunlight").GetValue<bool>())
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Magnifyinglensrequiresdirectsunlight"), false);
                return false;
            }
            if (!skipResearchItemCheck)
            {
                ResearchItem component = selectedFuelSource.GetComponent<ResearchItem>();
                if (component != null && !component.IsResearchComplete())
                {
                    Panel_Confirmation.CallbackDelegate forceBurnResearchItem = () => traverseInstance.Method("ForceBurnResearchItem").GetValue();
                    InterfaceManager.m_Panel_Confirmation.ShowBurnResearchNotification(forceBurnResearchItem);
                    return false;
                }
            }
            if (selectedAccelerant != null && selectedAccelerant == ___m_KeroseneAccelerant)
            {
                GameManager.GetPlayerManagerComponent().DeductLiquidFromInventory(GameManager.GetFireManagerComponent().m_KeroseneLitersAccelerant, GearLiquidTypeEnum.Kerosene);
            }
            float percentChanceSuccess = TinderHelper.CalculateFireStartSuccess(selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant);
            Debug.LogWarning("[TinderMatters] Panel_FireStart OnStartFire called: " + percentChanceSuccess);
            ___m_FireManager.PlayerStartFire(___m_CampFireInstance, selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant, percentChanceSuccess);
            __instance.Enable(false);
            GameAudioManager.PlayGuiConfirm();
            return false;
        }
    }

    [HarmonyPatch(typeof(Panel_FireStart))]
    [HarmonyPatch("RefreshChanceOfSuccessLabel")]
    static class Something
    {
        static void Postfix()
        {
            Debug.LogWarning("[TinderMatters] Panel_FireStart RefreshChanceOfSuccessLabel postfix");
        }

        [HarmonyPatch(typeof(Panel_FireStart))]
        [HarmonyPatch("RefreshChanceOfSuccessLabel")]
        static bool Prefix(Panel_FireStart __instance)
        {
            Debug.LogWarning("[TinderMatters] Panel_FireStart RefreshChanceOfSuccessLabel prefxissx: ");
            var traverseInstance = Traverse.Create(__instance);
            traverseInstance.Field("m_FireManager").SetValue(GameManager.GetFireManagerComponent());

            var selectedFireStarter = traverseInstance.Method("GetSelectedFireStarter").GetValue<FireStarterItem>();
            var selectedTinder = traverseInstance.Method("GetSelectedTinder").GetValue<FuelSourceItem>();
            var selectedFuelSource = traverseInstance.Method("GetSelectedFuelSource").GetValue<FuelSourceItem>();
            var selectedAccelerant = traverseInstance.Method("GetSelectedAccelerant").GetValue<FireStarterItem>();

            float num = TinderHelper.CalculateFireStartSuccess(selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant);
            if (selectedFireStarter.m_RequiresSunLight && !traverseInstance.Method("HasDirectSunlight").GetValue<bool>())
            {
                num = 0f;
            }

            Debug.LogWarning("[TinderMatters] Panel_FireStart RefreshChanceOfSuccessLabel called: " + num);
            __instance.m_Label_ChanceSuccess.text = num.ToString("F0") + "%";
            return false;
        }
    }

    // 1. Add tinder to CalclateFireStartSuccess
    // 2. Add the calls to that function in the 2 call sites
    // 3. Add values to the various tinder items
    // 4. OPTIONAL Add tinder to PlayerCalculateFireStartTime
    // 5. OPTIONAL Add the calls to that function in the 2 call sites
    // 6. OPTIONAL sort out m_HeatIncrease usage
    // 7. OPTIONAL sort out m_BurnDurationHours
    // 8. OPTIONAL always default to the best tinder in the UI

    [HarmonyPatch(typeof(FuelSourceItem))]
    [HarmonyPatch("Start")]
    class TinderValues
    {
        static void Postfix(FuelSourceItem __instance)
        {
            if (__instance.name == "GEAR_NewsprintRoll")
            {
                ChangeFireStartSkillModifier(__instance, 5f);
            }
            else if (__instance.name == "GEAR_PaperStack")
            {
                ChangeFireStartSkillModifier(__instance, 2f);
            }
            else if (__instance.name == "GEAR_Newsprint")
            {
                ChangeFireStartSkillModifier(__instance, 2f);
            }
            else if (__instance.name == "GEAR_CashBundle")
            {
                ChangeFireStartSkillModifier(__instance, 2f);
            }
            else if (__instance.name == "GEAR_BarkTinder")
            {
                ChangeFireStartSkillModifier(__instance, 0f);
            }
            else if (__instance.name == "GEAR_Tinder")
            {
                ChangeFireStartSkillModifier(__instance, -3f);
            }
            else if (__instance.name == "GEAR_CattailTinder")
            {
                ChangeFireStartSkillModifier(__instance, -5f);
            }
        }

        private static void ChangeFireStartSkillModifier(FuelSourceItem __instance, float value)
        {
            Debug.LogWarning("[TinderMatters] changed: " + __instance.name);
            __instance.m_FireStartSkillModifier = value;
        }

        private static void ChangeFireStartSkillModifier2(Traverse traverse, string name, float value)
        {
            Debug.LogWarning("[TinderMatters] changed: " + name);
            traverse.Field("m_FireStartSkillModifier").SetValue(value);
        }
    }

    public class TinderHelper
    {

        // COPY of CalclateFireStartSuccess with tinder added
        public static float CalculateFireStartSuccess(FireStarterItem starter, FuelSourceItem tinder, FuelSourceItem fuel, FireStarterItem accelerant)
        {
            if (starter == null || fuel == null)
            {
                return 0f;
            }
            float successChance = GameManager.GetSkillFireStarting().GetBaseChanceSuccess();
            successChance += starter.m_FireStartSkillModifier;

            Debug.LogWarning("[TinderMatters] CHECKED: " + tinder.name + " : " + tinder.m_FireStartSkillModifier);
//            successChance += tinder.m_FireStartSkillModifier;
            successChance += GetModifiedFireStartSkillModifier(tinder);
            successChance += fuel.m_FireStartSkillModifier;
            if (accelerant)
            {
                successChance += accelerant.m_FireStartSkillModifier;
            }
            return Mathf.Clamp(successChance, 0f, 100f);
        }

        public static float GetModifiedFireStartSkillModifier(FuelSourceItem __instance)
        {
            if (__instance.name == "GEAR_NewsprintRoll")
            {
                return 5f;
            }
            else if (__instance.name == "GEAR_PaperStack")
            {
                return 2f;
            }
            else if (__instance.name == "GEAR_Newsprint")
            {
                return 2f;
            }
            else if (__instance.name == "GEAR_CashBundle")
            {
                return 2f;
            }
            else if (__instance.name == "GEAR_BarkTinder")
            {
                return 0f;
            }
            else if (__instance.name == "GEAR_Tinder")
            {
                return -3f;
            }
            else if (__instance.name == "GEAR_CattailTinder")
            {
                return -5f;
            }
            else
            {
                Debug.LogWarning("[TinderMatters] MISSING TINDER " + __instance.name);
                return 0;
            }
        }
    }
}
/*
Tinder plug: 0
Cat tail : +2
Birch Bark: +5
Cash Bundle, newsprint, paper stack: +7f
Newsprint roll: +10f

OR

Tinder plug: -5
Cat tail : -3
Birch Bark: 0
Cash Bundle, newsprint, paper stack: +2f
Newsprint roll: +5f
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
*/
/*
 * GEAR_NewsprintRoll
 * 
m_FireStartSkillModifier                               15.0
m_FireStartDurationModifier                            -20.0
m_BurnDurationHours                                    0.4
m_HeatIncrease                                         10.0
m_WeightKG                                             0.1
 * 
 * 
 * GEAR_PaperStack
 * 
m_FireStartSkillModifier                               10.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.3
m_HeatIncrease                                         7.0
m_WeightKG                                             0.1
 * 
 * GEAR_Newsprint
 * 
m_FireStartSkillModifier                               10.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.3
m_HeatIncrease                                         7.0
m_WeightKG                                             0.1
 * 
 *  GEAR_CashBundle
 * 
m_FireStartSkillModifier                               10.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.3
m_HeatIncrease                                         7.0
m_WeightKG                                             0.05
 *  
 * GEAR_BarkTinder
 * 
 m_FireStartSkillModifier                               10.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.02
m_HeatIncrease                                         5.0
m_WeightKG                                             0.05

 *  GEAR_CattailTinder

m_FireStartSkillModifier                               10.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.02
m_HeatIncrease                                         5.0
m_WeightKG                                             0.05
 * 
 * GEAR_Tinder
 * 
m_FireStartSkillModifier                               5.0
m_FireStartDurationModifier                            -10.0
m_BurnDurationHours                                    0.02
m_HeatIncrease                                         5.0
m_WeightKG                                             0.05
 * 
 * 
 * 
 * m_FireStartSkillModifier set to +10 skill except tinder which is +5 and newsprint roll which is +15
 * m_FireStartDurationModifier set to -10 except newsprintroll which is -20
 * m_BurnDurationHours 0.02 for tinder, bark and cat tails, 0.3 for paper + newsprint + cash, 0.4 for newsprint roll, 
 * m_HeatIncrease +5 for cat tail, tinder, +7 for paper, cash, newsprint and +10 for newsprint
 */