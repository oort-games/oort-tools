#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using OortTools.Core.Utils;
using System;

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
        for (int i = 1; i <= 1000; i++)
            _dummyTasks.Add($"Task #{i}");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Progress Example", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("버튼을 누르면 ProgressBar가 표시됩니다.\nCancel을 누르면 중간에 취소됩니다.", MessageType.Info);

        using (new EditorGUI.DisabledScope(_isRunning))
        {
            if (GUILayout.Button("Run Task Simulation #Popup", GUILayout.Height(30)))
            {
                _isRunning = true;
                _currentIndex = 0;

                EditorApplication.update += RunTasks;
            }

            if (GUILayout.Button("Run Task Simulation #Background", GUILayout.Height(30)))
            {
                _isRunning = true;
                _currentIndex = 0;

                RunTasksThree();
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

        if (EditorProgress.Show("Progress Example #2", $"Processing {taskName}", _currentIndex, _dummyTasks.Count))
        {
            FinishTasks();
            return;
        }

        _currentIndex++;
    }

    private void RunTasksThree()
    {
        List<Action> tasks = new();
        for (int i = 0; i < _dummyTasks.Count; i++)
        {
            string task = _dummyTasks[i];
            tasks.Add(() => { Debug.Log($"{task}"); });
        }

        EditorProgressBackground.Show("Progress Example #3", "Processing", tasks, 
            ()=> { _isRunning = false; Debug.Log($"Finish"); }, 
            () => { _isRunning = false; Debug.Log($"Cancel"); });
    }

    private void FinishTasks()
    {
        _isRunning = false;
        EditorApplication.update -= RunTasks;
        EditorProgress.Clear();

        Repaint();
    }
}
#endif
