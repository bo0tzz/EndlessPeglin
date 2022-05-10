using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using Relics;

namespace EndlessPeglin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Peglin.exe")]
    public class EndlessPeglin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

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
        public static void Prefix(Dictionary<RelicEffect, Relic> ____ownedRelics, ref List<Relic> __state)
        {
            if (____ownedRelics == null) return;
            __state = ____ownedRelics.Values.ToList();
        }

        public static void Postfix(RelicManager __instance, ref List<Relic> __state)
        {
            if (__state == null) return;
            if (GameInitPatch.stolenLoadMapData != null && 
                GameInitPatch.stolenLoadMapData.NewGame && 
                UnityEngine.PlayerPrefs.GetInt(GameInit.BOSS_STREAK_PREF_STRING, 0) > 0)
            {
                UnityEngine.Debug.Log("Re-adding relics after reset");
                __state.ForEach(__instance.AddRelic);
            }
        }
    }

}
