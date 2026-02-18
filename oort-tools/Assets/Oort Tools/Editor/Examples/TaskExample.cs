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

            if (GUILayout.Button("Run Task Simulation", GUILayout.Height(30)))
            {
                RunExample();
            }

            if (GUILayout.Button("Run Two Roots (Parallel)", GUILayout.Height(30)))
            {
                RunParallelExample();
            }
        }

        void RunExample()
        {
            var root = new RootExampleTask();

            root.AddChild(new ExampleTask("A", 3));
            root.AddChild(new ExampleTask("B", 5));
            root.AddChild(new ExampleTask("C", 2));

            EditorTaskRunner.Start(root);
        }

        void RunParallelExample()
        {
            var root1 = new RootExampleTask();
            root1.AddChild(new ExampleTask("Root1-A", 3));

            var root2 = new RootExampleTask();
            root2.AddChild(new ExampleTask("Root2-B", 5));

            EditorTaskRunner.Start(root1);
            EditorTaskRunner.Start(root2);
        }
    }

    public class RootExampleTask : EditorTask
    {
        protected override IEnumerator ExecuteTask()
        {
            Debug.Log("[Root] Start");
            yield return null;
        }
    }

    public class ExampleTask : EditorTask
    {
        readonly string _name;
        readonly int _steps;

        public override string DisplayName => _name;

        public ExampleTask(string name, int steps)
        {
            _name = name;
            _steps = steps;
        }

        protected override IEnumerator ExecuteTask()
        {
            Debug.Log($"[{_name}] Start");

            for (int i = 1; i <= _steps; i++)
            {
                if (State == EditorTaskState.Canceled)
                    yield break;

                Debug.Log($"[{_name}] Step {i}/{_steps}");
                yield return null;
            }

            Debug.Log($"[{_name}] End");
        }
    }
}
#endif
