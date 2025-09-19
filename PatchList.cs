using HarmonyLib;
using ShinyShoe.Loading;

namespace Patty_UIScaler_MOD
{
    internal class PatchList
    {
        [HarmonyPostfix, HarmonyPatch(typeof(ShinyShoe.AppManager), "DoesThisBuildReportErrors")]
        public static void DisableErrorReportingPatch(ref bool __result)
        {
            __result = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LoadScreen), "StartLoadingScreen")]
        public static void StartLoadingScreen(LoadScreen __instance, ref ScreenManager.ScreenActiveCallback ___screenActiveCallback)
        {
            ___screenActiveCallback += (IScreen _) =>
            {
                Plugin.UpdateScale();
            };
        }
    }
}
