#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools.Core.Preferences
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
            bool autoOpenSelected = EditorGUILayout.Toggle("CSV ���� �� ���� ��ġ �ڵ� ����", autoOpenCurrent);
            if (autoOpenSelected != autoOpenCurrent) PreferencesPrefs.CsvAutoOpen = autoOpenSelected;
        }
    }
}
#endif
