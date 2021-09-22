using BepInEx;
using HarmonyLib;

namespace PeglinHealth
{
    [BepInPlugin(guid, name, version)]
    [BepInProcess("Peglin.exe")]
    public class PeglinHealth : BaseUnityPlugin
    {
        private const string guid = "me.bo0tzz.peglinmods.health";
        private const string name = "Example Health Plug-In";
        private const string version = "1.0.0.0";
        private readonly Harmony harmony = new Harmony(guid);

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(GameInit), "Start")]
        [HarmonyPostfix]
        static void AddExtraHealth(FloatVariable ___maxPlayerHealth, FloatVariable ___playerHealth)
        {
            ___maxPlayerHealth.Add(50f);
            ___playerHealth.Add(50f);
        }
    }
}
