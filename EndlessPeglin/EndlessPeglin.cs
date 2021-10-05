using BepInEx;
using HarmonyLib;

namespace EndlessPeglin
{
    [BepInPlugin(modGuid, modName, modVersion)]
    [BepInProcess("Peglin.exe")]
    public class EndlessPeglin : BaseUnityPlugin
    {
        private const string modGuid = "me.bo0tzz.peglinmods.endless";
        private const string modName = "Endless Peglin";
        private const string modVersion = "1.0.0.0";
        private readonly Harmony harmony = new Harmony(modGuid);

        void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo("Endless peglin mod loaded");
        }
    }
    
    [HarmonyPatch(typeof(GameInit), "Start")]
    public class GameInitPatch
    {
        public static LoadMapData stolenLoadMapData;

        public static void Prefix(LoadMapData ___LoadData)
        {
            if (___LoadData != null)
            {
                GameInitPatch.stolenLoadMapData = ___LoadData;
            }
        }
    }

    [HarmonyPatch(typeof(DeckManager), "InstantiateDeck")]
    public class DeckManagerInstantiatePatch
    {
        public static bool Prefix()
        {
            if (GameInitPatch.stolenLoadMapData != null && 
                GameInitPatch.stolenLoadMapData.NewGame && 
                UnityEngine.PlayerPrefs.GetInt(GameInit.BOSS_STREAK_PREF_STRING, 0) > 0)
            {
                UnityEngine.Debug.Log("Skipping deck instantiate");
                return false;
            }
            return true;
        }
    }
    
   
    [HarmonyPatch(typeof(Relics.RelicManager), "Reset")]
    public class RelicResetPatch
    {
        public static bool Prefix()
        {
            if (GameInitPatch.stolenLoadMapData != null && 
                GameInitPatch.stolenLoadMapData.NewGame && 
                UnityEngine.PlayerPrefs.GetInt(GameInit.BOSS_STREAK_PREF_STRING, 0) > 0)
            {
                UnityEngine.Debug.Log("Skipping relic reset");
                return false;
            }
            return true;
        }
    }

}
