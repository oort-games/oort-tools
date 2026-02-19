#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public static class EditorTaskRunner
    {
        static readonly List<IEditorTask> _runningTasks = new();
        static readonly List<EditorTaskExecution> _executions = new();

        static bool _isSubscribed;

        public static IReadOnlyList<IEditorTask> RunningTasks => _runningTasks;

        public static void Start(IEditorTask task)
        {
            if (task == null)
                return;

            _runningTasks.Add(task);
            _executions.Add(new EditorTaskExecution(task.Execute()));

            if (!_isSubscribed)
            {
                EditorApplication.update += Update;
                _isSubscribed = true;
            }
        }

        static void Update()
        {
            for (int i = _executions.Count - 1; i >= 0; i--)
            {
                if (_runningTasks[i].State == EditorTaskState.Paused)
                    continue;

                try
                {
                    if (!_executions[i].MoveNext())
                    {
                        _runningTasks.RemoveAt(i);
                        _executions.RemoveAt(i);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    _runningTasks.RemoveAt(i);
                    _executions.RemoveAt(i);
                }
            }

            if (_executions.Count == 0)
            {
                EditorApplication.update -= Update;
                _isSubscribed = false;
            }
        }

        public static void CancelAll()
        {
            for (int i = 0; i < _runningTasks.Count; i++)
                _runningTasks[i]?.Cancel();
        }
    }
}
#endif