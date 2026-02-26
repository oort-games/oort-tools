#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OortTools
{
    public class OortVisualElement
    {
        static StyleSheet _cachedStyleSheet;

        public static StyleSheet GetStyleSheet
        {
            get
            {
                if (_cachedStyleSheet == null)
                {
                    const string path = "Assets/Oort Tools/Editor/UI/Styles/OortStyles.uss";
                    _cachedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

                    if (_cachedStyleSheet == null)
                    {
                        Debug.LogWarning($"USS 파일 로드 실패: {path}");
                    }
                }
                return _cachedStyleSheet;
            }
        }

        public static void ApplyStyles(VisualElement element)
        {
            var ss = GetStyleSheet;
            if (ss != null && !element.styleSheets.Contains(ss))
            {
                element.styleSheets.Add(ss);
            }
        }

        public static void ApplyRootStyle(VisualElement root)
        {
            root.AddToClassList("root");
        }

        public static VisualElement CreateHeader(string title)
        {
            var header = new VisualElement();
            header.AddToClassList("header");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("header-title");

            header.Add(titleLabel);

            return header;
        }
    }
}
#endif