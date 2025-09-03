#if UNITY_EDITOR
using UnityEditor;

namespace OortTools.Core.Preferences
{
    public static class PreferencesPrefs
    {
        public const string KEY_CSV_AUTO_OPEN = "OortTools.CsvExporter.AutoOpen";

        public static bool CsvAutoOpen
        {
            get => EditorPrefs.GetBool(KEY_CSV_AUTO_OPEN, false);
            set => EditorPrefs.SetBool(KEY_CSV_AUTO_OPEN, value);
        }
    }
}
#endif