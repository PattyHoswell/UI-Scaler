using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Patty_UIScaler_MOD
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource LogSource { get; private set; }

        internal static Harmony PluginHarmony { get; private set; }

        public static Dictionary<ScreenName, ConfigEntry<float>> Entries { get; private set; } = new Dictionary<ScreenName, ConfigEntry<float>>();

        void Awake()
        {
            LogSource = Logger;
            try
            {
                PluginHarmony = Harmony.CreateAndPatchAll(typeof(PatchList), PluginInfo.GUID);
            }
            catch (HarmonyException ex)
            {
                LogSource.LogError((ex.InnerException ?? ex).Message);
            }
            var maxScaleFactor = Config.Bind("Basic", "Max Scale Factor", 3f, "Set the max scale factor (restart to apply)");
            foreach (var screenName in Enum.GetValues(typeof(ScreenName)).Cast<ScreenName>())
            {
                Entries[screenName] = Config.Bind("Hud", screenName.ToString(), 1f,
                                                  new ConfigDescription("", new AcceptableValueRange<float>(0, maxScaleFactor.Value)));
                Entries[screenName].SettingChanged += HudScale_SettingChanged;
            }
        }

        private void HudScale_SettingChanged(object sender, System.EventArgs e)
        {
            UpdateScale();
        }

        public static void UpdateScale()
        {
            var allGameManager = AllGameManagers.Instance;
            if (allGameManager == null || allGameManager.GetScreenManager() == null)
            {
                return;
            }
            var screenManager = allGameManager.GetScreenManager();
            foreach (var entry in Entries)
            {
                var screen = screenManager.GetScreen(entry.Key) as UIScreen;
                if (screen != null && screen.transform.parent.TryGetComponent(out Canvas canvas))
                {
                    canvas.scaleFactor = entry.Value.Value;
                }
            }
        }
    }
}
