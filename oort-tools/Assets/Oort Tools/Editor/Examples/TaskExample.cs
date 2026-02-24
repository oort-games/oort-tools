#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public class TaskExample : EditorWindow
    {
        public bool isRunningSequential;
        public bool isRunningParallel;
        public bool isRunningParallelOne;
        public bool isRunningParallelTwo;
        public bool isRunningNested;

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

            using (new EditorGUI.DisabledScope(isRunningSequential))
            {
                if (GUILayout.Button("Run Task (Sequential)", GUILayout.Height(30)))
                {
                    isRunningSequential = true;
                    RunExample();
                }
            }

            using (new EditorGUI.DisabledScope(isRunningParallel))
            {
                if (GUILayout.Button("Run Task (Parallel Roots)", GUILayout.Height(30)))
                {
                    isRunningParallel = true;
                    isRunningParallelOne = true;
                    isRunningParallelTwo = true;
                    RunParallelExample();
                }
            }

            using (new EditorGUI.DisabledScope(isRunningNested))
            {
                if (GUILayout.Button("Run Task (Nested Coroutine)", GUILayout.Height(30)))
                {
                    isRunningNested = true;
                    EditorTaskRunner.Start(new ExampleNestedTask(() => 
                    { 
                        isRunningNested = false;
                        Repaint();
                    }));
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Open Editor Task Window", GUILayout.Height(30)))
            {
                EditorTaskWindow.Open();
            }
        }

        void RunExample()
        {
            var root = new RootExampleTask("RunExample", () => 
            {
                isRunningSequential = false; 
                Repaint();
            });

            var rootA = new RootExampleTask("A");
            rootA.AddChild(new ExampleTask("A #1", 100));
            rootA.AddChild(new ExampleTask("A #2", 200));
            rootA.AddChild(new ExampleTask("A #3", 300));

            var rootB = new RootExampleTask("B");
            rootB.AddChild(new ExampleTask("B #1", 100));
            rootB.AddChild(new ExampleTask("B #2", 200));

            root.AddChild(rootA);
            root.AddChild(rootB);
            root.AddChild(new ExampleTask("C", 200));

            EditorTaskRunner.Start(root);
        }

        void RunParallelExample()
        {
            var root1 = new RootExampleTask("RunParallelExample #1", () => 
            {
                isRunningParallelOne = false;
                CheckEndParallelExample(); 
            });
            root1.AddChild(new ExampleTask("Root1-A", 300));

            var root2 = new RootExampleTask("RunParallelExample #2", () => 
            { 
                isRunningParallelTwo = false;
                CheckEndParallelExample(); 
            });
            root2.AddChild(new ExampleTask("Root2-A", 500));

            EditorTaskRunner.Start(root1);
            EditorTaskRunner.Start(root2);
        }

        void CheckEndParallelExample()
        {
            if (isRunningParallelOne == false && isRunningParallelTwo == false)
            {
                isRunningParallel = false;
                Repaint();
            }
        }
    }

    public class RootExampleTask : EditorTask
    {
        readonly string _name;
        readonly Action _onFinish;

        public override string DisplayName => _name;

        public RootExampleTask(string name, Action onFinish = null)
        {
            _name = name;

            _onFinish = onFinish;
            OnStateChanged += TaskOnStateChanged;
        }

        protected override IEnumerator ExecuteTask()
        {
            Debug.Log($"[{_name}] Start");
            yield return null;
        }

        void TaskOnStateChanged(EditorTaskState state)
        {
            Debug.Log($"[{_name}] {state}");
            switch (state)
            {
                case EditorTaskState.Completed:
                case EditorTaskState.Canceled:
                case EditorTaskState.Failed:
                    Debug.Log($"[{_name}] {state}");
                    _onFinish?.Invoke();
                    break;
            }
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
        readonly Action _onFinish;

        public ExampleNestedTask(Action onFinish = null)
        {
            _onFinish = onFinish;
            OnStateChanged += TaskOnStateChanged;
        }

        protected override IEnumerator ExecuteTask()
        {
            Debug.Log("[Start] A");
            float start = (float)EditorApplication.timeSinceStartup;
            while (EditorApplication.timeSinceStartup < start + 1f)
            {
                yield return null;
            }
            SetProgress(1 / 3f);
            yield return B("A");
            Debug.Log("[End] A");
        }

        IEnumerator B(string parent)
        {
            Debug.Log($"[Start] {parent} - B");
            float start = (float)EditorApplication.timeSinceStartup;
            while (EditorApplication.timeSinceStartup < start + 1f)
            {
                yield return null;
            }
            SetProgress(2 / 3f);
            yield return C($"{parent} - B");
            Debug.Log($"[End] {parent} - B");
        }

        IEnumerator C(string parent)
        {
            Debug.Log($"[Start] {parent} - C");            
            float start = (float)EditorApplication.timeSinceStartup;
            while (EditorApplication.timeSinceStartup < start + 1f)
            {
                yield return null;
            }
            SetProgress(1);
            Debug.Log($"[End] {parent} - C");
        }

        void TaskOnStateChanged(EditorTaskState state)
        {
            switch (state)
            {
                case EditorTaskState.Completed:
                case EditorTaskState.Canceled:
                case EditorTaskState.Failed:
                    _onFinish?.Invoke();
                    break;
            }
        }
    }

}
#endif
