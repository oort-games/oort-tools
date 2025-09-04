#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace OortTools.Core.Utils
{
    public class ProgressExample : EditorWindow
    {
        private List<string> _dummyTasks;
        private bool _isRunning;
        private int _currentIndex;

        [MenuItem("Oort Tools/Examples/Progress Example")]
        public static void Open()
        {
            GetWindow<ProgressExample>("Progress Example");
        }

        private void OnEnable()
        {
            _dummyTasks = new List<string>();
            for (int i = 1; i <= 50; i++)
                _dummyTasks.Add($"Task #{i}");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Progress Example", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("��ư�� ������ ProgressBar�� ǥ�õ˴ϴ�.\nCancel�� ������ �߰��� ��ҵ˴ϴ�.", MessageType.Info);

            using (new EditorGUI.DisabledScope(_isRunning))
            {
                if (GUILayout.Button("Run Task Simulation", GUILayout.Height(30)))
                {
                    _isRunning = true;
                    _currentIndex = 0;

                    EditorApplication.update += RunTasks;
                }
            }
        }

        private void RunTasks()
        {
            if (_currentIndex >= _dummyTasks.Count)
            {
                FinishTasks();
                return;
            }

            string taskName = _dummyTasks[_currentIndex];
            float progress = (float)(_currentIndex + 1) / _dummyTasks.Count;

            if (EditorProgress.Show("Progress Example", $"Processing {taskName}", progress))
            {
                FinishTasks();
                return;
            }

            System.Threading.Thread.Sleep(50);
            _currentIndex++;
        }

        private void FinishTasks()
        {
            _isRunning = false;
            EditorApplication.update -= RunTasks;
            EditorProgress.Clear();

            Repaint();
        }
    }
}
#endif
