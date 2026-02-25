#if UNITY_EDITOR
using UnityEditor;

namespace OortTools
{
    public static class PreferencesWindow
    {
        [MenuItem("Oort Tools/Preferences", false, 1)]
        public static void OpenOortToolsPreferences()
        {
            SettingsService.OpenUserPreferences("Preferences/Oort Tools");
        }
    }
}
#endif