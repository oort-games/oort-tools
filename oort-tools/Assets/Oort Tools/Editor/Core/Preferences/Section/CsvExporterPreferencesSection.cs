#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace OortTools
{
    [InitializeOnLoad]
    public class CsvExporterPreferencesSection : IPreferencesSection
    {
        static CsvExporterPreferencesSection()
        {
            PreferencesRegistry.Register(new CsvExporterPreferencesSection());
        }

        public string Title => "CSV Exporter";
        public IEnumerable<string> Keywords => new[] { "csv", "export", "auto", "open" };

        public void OnGUI()
        {
            bool autoOpenCurrent = PreferencesPrefs.CsvAutoOpen;
            bool autoOpenSelected = EditorGUILayout.Toggle("CSV 저장 후 파일 위치 자동 열기", autoOpenCurrent);
            if (autoOpenSelected != autoOpenCurrent) PreferencesPrefs.CsvAutoOpen = autoOpenSelected;
        }
    }
}
#endif
