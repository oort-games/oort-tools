#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections;

namespace OortTools
{
    public class TaskExample : EditorWindow
    {
        [MenuItem("Oort Tools/Examples/Task Example")]
        public static void Open()
        {
            GetWindow<TaskExample>("Task Example");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Task Example", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("미완성 입니다.", MessageType.Info);

            if (GUILayout.Button("Run Task Simulation Popup", GUILayout.Height(30)))
            {
                EditorTaskScheduler.Enqueue(new ExampleTask("#1", 10));
                EditorTaskScheduler.Enqueue(new ExampleTask("#2", 20));
            }
        }
    }

    public class ExampleTask : EditorTask
    {
        public override string DisplayName => $"Example Task {_name}";
        public override int Priority => _priority;

        readonly string _name;
        readonly int _priority;

        public ExampleTask(string name, int priority)
        {
            _name = name;
            _priority = priority;
        }

        public override IEnumerator Execute()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (CheckCanceled())
                        yield break;

                    Debug.Log($"Example Task {_name} : {i}");
                    EditorProgress.Show($"Example Task {_name}", $"# {i}", i, 1000);
                    yield return null;
                }
            }
            finally
            {
                EditorProgress.Clear();
            }
        }
    }
}
#endif
