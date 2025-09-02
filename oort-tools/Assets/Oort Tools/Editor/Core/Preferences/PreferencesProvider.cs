#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools.Core.Preferences
{
    public interface IUtilitiesPreferencesSection
    {
        string Title { get; }
        IEnumerable<string> Keywords { get; }
        void OnGUI();
    }

    public static class UtilitiesPreferencesRegistry
    {
        static readonly List<IUtilitiesPreferencesSection> _sections = new();
        public static void Register(IUtilitiesPreferencesSection section)
        {
            if (section != null && !_sections.Contains(section)) _sections.Add(section);
        }
        public static IReadOnlyList<IUtilitiesPreferencesSection> Sections => _sections;
    }

    public static class UtilitiesPrefs
    {
        public const string Csv_AutoOpen_Key = "OortTools.CsvExporter.AutoOpen";

        public static bool CsvAutoOpen
        {
            get => EditorPrefs.GetBool(Csv_AutoOpen_Key, false);
            set => EditorPrefs.SetBool(Csv_AutoOpen_Key, value);
        }
    }

    public static class UtilitiesPreferencesProviderFactory
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider("Preferences/Oort Tools", SettingsScope.User)
            {
                label = "Oort Tools",
                guiHandler = _ =>
                {
                    EditorGUILayout.Space(4);

                    foreach (var section in UtilitiesPreferencesRegistry.Sections)
                    {
                        EditorGUILayout.LabelField(section.Title, EditorStyles.boldLabel);
                        section.OnGUI();
                        EditorGUILayout.Space(10);
                    }
                },
                keywords = new HashSet<string>(CollectKeywords())
            };
            return provider;

            static IEnumerable<string> CollectKeywords()
            {
                foreach (var s in UtilitiesPreferencesRegistry.Sections)
                    foreach (var k in s.Keywords)
                        yield return k;
            }
        }
    }
}
#endif
