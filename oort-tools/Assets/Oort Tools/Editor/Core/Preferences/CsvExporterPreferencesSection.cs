#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools.Core.Preferences
{
    [InitializeOnLoad]
    public class CsvExporterPreferencesSection : IUtilitiesPreferencesSection
    {
        static CsvExporterPreferencesSection()
        {
            UtilitiesPreferencesRegistry.Register(new CsvExporterPreferencesSection());
        }

        public string Title => "CSV Exporter";
        public IEnumerable<string> Keywords => new[] { "csv", "export", "auto", "open" };

        public void OnGUI()
        {
            bool autoOpenCurrentv = UtilitiesPrefs.CsvAutoOpen;
            bool autoOpenSelected = EditorGUILayout.Toggle("CSV ���� �� ���� ��ġ �ڵ� ����", autoOpenCurrentv);
            if (autoOpenSelected != autoOpenCurrentv) UtilitiesPrefs.CsvAutoOpen = autoOpenSelected;
        }
    }
}
#endif
