#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace OortTools
{
    public class PreferencesProviderView : SettingsProvider
    {
        const string PATH = "Preferences/Oort Tools";
        const string TITLE = "Oort Tools";
        const float LABEL_WIDTH = 250f;

        readonly GUIStyle marginStyle = new () { margin = new RectOffset(10, 10, 10, 10) };

        public PreferencesProviderView() : base (PATH, SettingsScope.User)
        {
            label = TITLE;
            keywords = new HashSet<string>(CollectKeywords());
        }

        public override void OnGUI(string searchContext)
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            GUILayout.BeginVertical(marginStyle);
            EditorGUIUtility.labelWidth = LABEL_WIDTH;
            foreach (var section in PreferencesRegistry.Sections)
            {
                EditorGUILayout.LabelField(section.Title, EditorStyles.boldLabel);
                section.OnGUI();
                EditorGUILayout.Space(10);
            }
            EditorGUIUtility.labelWidth = originalLabelWidth;
            GUILayout.EndVertical();
        }

        IEnumerable<string> CollectKeywords()
        {
            foreach (var section in PreferencesRegistry.Sections)
                foreach (var keyword in section.Keywords)
                    yield return keyword;
        }
    }
}
#endif
