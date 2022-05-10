using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using Relics;
using UnityEngine;

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
            Logger.LogInfo("EndlessPeglin mod loaded");
        }
        
        public static bool ShouldLoop()
        {
            return GameInitPatch.stolenLoadMapData != null &&
                   GameInitPatch.stolenLoadMapData.NewGame &&
                   PlayerPrefs.GetInt(GameInit.BOSS_STREAK_PREF_STRING, 0) > 0;
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
                stolenLoadMapData = ___LoadData;
            }

            if (EndlessPeglin.ShouldLoop())
            {
                Debug.Log("Going endless!");
            }
        }

        public static void Postfix()
        {
            if (EndlessPeglin.ShouldLoop())
            {
                Debug.Log("Done going endless!");
                PlayerPrefs.SetInt(GameInit.BOSS_STREAK_PREF_STRING, 0);
            }
        }
    }

    [HarmonyPatch(typeof(DeckManager), "InstantiateDeck")]
    public class DeckManagerInstantiatePatch
    {
        public static bool Prefix()
        {
            if (EndlessPeglin.ShouldLoop())
            {
                Debug.Log("Skipping deck instantiate");
                return false;
            }
            return true;
        }
    }
    
   
    [HarmonyPatch(typeof(RelicManager), "Reset")]
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
            if (EndlessPeglin.ShouldLoop())
            {
                Debug.Log("Re-adding relics after reset");
                __state.ForEach(__instance.AddRelic);
            }
        }
    }

}
