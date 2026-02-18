#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OortTools
{
    public class ProgressExample : EditorWindow
    {
        List<string> _dummyTasks;
        int _currentIndex;

        bool _isRunningPopup;
        bool _isRunningBackgroundOne;
        bool _isRunningBackgroundTwo;

        [MenuItem("Oort Tools/Examples/Progress Example")]
        public static void Open()
        {
            GetWindow<ProgressExample>("Progress Example");
        }

        void OnEnable()
        {
            _dummyTasks = new List<string>();
            for (int i = 1; i <= 1000; i++)
                _dummyTasks.Add($"Task #{i}");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Progress Example", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("버튼을 누르면 ProgressBar가 표시됩니다.\nCancel을 누르면 중간에 취소됩니다.", MessageType.Info);

            using (new EditorGUI.DisabledScope(_isRunningPopup))
            {
                if (GUILayout.Button("Run Task Simulation Popup", GUILayout.Height(30)))
                {
                    _currentIndex = 0;
                    _isRunningPopup = true;
                    EditorApplication.update += RunTask;
                }
            }
            using (new EditorGUI.DisabledScope(_isRunningBackgroundOne))
            {
                if (GUILayout.Button("Run Task Simulation Background #1", GUILayout.Height(30)))
                {
                    _isRunningBackgroundOne = true;
                    RunTaskBackgroundOne();
                }
            }
            using (new EditorGUI.DisabledScope(_isRunningBackgroundTwo))
            {
                if (GUILayout.Button("Run Task Simulation Background #2", GUILayout.Height(30)))
                {
                    _isRunningBackgroundTwo = true;
                    RunTaskBackgroundTwo();
                }
            }
        }

        void RunTask()
        {
            if (_currentIndex >= _dummyTasks.Count)
            {
                FinishTasks();
                return;
            }

            string taskName = _dummyTasks[_currentIndex];

            if (EditorProgress.ShowCancelable("Progress Example Popup", $"Processing {taskName}", _currentIndex, _dummyTasks.Count))
            {
                FinishTasks();
                return;
            }

            _currentIndex++;
        }

        void RunTaskBackgroundOne()
        {
            List<Action> tasks = new();
            for (int i = 0; i < _dummyTasks.Count; i++)
            {
                string task = _dummyTasks[i];
                tasks.Add(() => { Debug.Log($"{task}"); });
            }

            new EditorBackgroundProgress("Progress Example Background #1", "Processing", tasks,
                () => { _isRunningBackgroundOne = false; Debug.Log($"Finish"); },
                () => { _isRunningBackgroundOne = false; Debug.Log($"Cancel"); });
        }

        void RunTaskBackgroundTwo()
        {
            List<Action> tasks = new();
            for (int i = 0; i < _dummyTasks.Count; i++)
            {
                string task = _dummyTasks[i];
                tasks.Add(() => { Debug.Log($"{task}"); });
            }

            new EditorBackgroundProgress("Progress Example Background #2", "Processing", tasks,
                () => { _isRunningBackgroundTwo = false; Debug.Log($"Finish"); },
                () => { _isRunningBackgroundTwo = false; Debug.Log($"Cancel"); });
        }

        void FinishTasks()
        {
            _isRunningPopup = false;
            EditorApplication.update -= RunTask;
            EditorProgress.Clear();

            Repaint();
        }
    }
}
#endif
