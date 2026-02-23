#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public static class EditorBackgroundTaskWindow
    {
        public static void Open()
        {
            var assembly = typeof(EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.ProgressWindow");

            if (type != null)
            {
                EditorWindow.GetWindow(type);
            }
            else
            {
                Debug.LogWarning("Unity ProgressWindow type not found.");
            }
        }
    }
}
#endif