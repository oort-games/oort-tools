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
            EditorGUILayout.HelpBox(
                "Run Task (Sequential) : 부모-자식 트리 구조의 순차 실행\n" +
                "Run Task (Parallel Roots) : 루트 단위 병렬 실행\n" +
                "Run Task (Nested Coroutine) : Nested IEnumerator 스택 처리",
                MessageType.Info);

            if (GUILayout.Button("Run Task (Sequential)", GUILayout.Height(30)))
            {
                RunExample();
            }

            if (GUILayout.Button("Run Task (Parallel Roots)", GUILayout.Height(30)))
            {
                RunParallelExample();
            }

            if (GUILayout.Button("Run Task (Nested Coroutine)", GUILayout.Height(30)))
            {
                EditorTaskRunner.Start(new ExampleNestedTask());
            }
        }

        void RunExample()
        {
            var root = new RootExampleTask("RunExample");

            root.AddChild(new ExampleTask("A", 300));
            root.AddChild(new ExampleTask("B", 500));
            root.AddChild(new ExampleTask("C", 200));

            EditorTaskRunner.Start(root);
        }

        void RunParallelExample()
        {
            var root1 = new RootExampleTask("RunParallelExample #1");
            root1.AddChild(new ExampleTask("Root1-A", 300));

            var root2 = new RootExampleTask("RunParallelExample #2");
            root2.AddChild(new ExampleTask("Root2-B", 500));

            EditorTaskRunner.Start(root1);
            EditorTaskRunner.Start(root2);
        }
    }

    public class RootExampleTask : EditorTask
    {
        readonly string _name;

        public override string DisplayName => _name;

        public RootExampleTask(string name)
        {
            _name = name;
        }

        protected override IEnumerator ExecuteTask()
        {
            Debug.Log($"[{_name}] Start");
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
            Debug.Log($"[{Parent.DisplayName}][{_name}] Start");

            for (int i = 1; i <= _steps; i++)
            {
                if (State == EditorTaskState.Canceled)
                    yield break;

                SetProgress((float)i/ _steps);
                SetSubMessage($"Processing {i}/{_steps}");
                Debug.Log($"[{Parent.DisplayName}][{_name}] Step {i}/{_steps}");
                yield return null;
            }

            Debug.Log($"[{Parent.DisplayName}][{_name}] End");
        }
    }

    public class ExampleNestedTask : EditorTask
    {
        protected override IEnumerator ExecuteTask()
        {
            Debug.Log("[Start] A");
            yield return B("A");
            Debug.Log("[End] A");
        }

        IEnumerator B(string parent)
        {
            Debug.Log($"[Start] {parent} - B");
            yield return C($"{parent} - B");
            Debug.Log($"[End] {parent} - B");
        }

        IEnumerator C(string parent)
        {
            Debug.Log($"[Start] {parent} - C");
            yield return null;
            Debug.Log($"[End] {parent} - C");
        }
    }

}
#endif
