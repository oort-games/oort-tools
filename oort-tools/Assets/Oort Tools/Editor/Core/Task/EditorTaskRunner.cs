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

        static readonly List<IEditorTask> _historyTasks = new();

        static bool _isSubscribed;

        public static IReadOnlyList<IEditorTask> RunningTasks => _runningTasks;
        public static IReadOnlyList<IEditorTask> HistoryTasks => _historyTasks;

        public static void Start(IEditorTask task)
        {
            if (task == null)
                return;

            _runningTasks.Add(task);
            _historyTasks.Add(task);
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
                var task = _runningTasks[i];
                if (task.State == EditorTaskState.Paused)
                    continue;

                try
                {
                    if (!_executions[i].MoveNext())
                    {
                        if (task.State == EditorTaskState.Canceled)
                        {
                            _historyTasks.Remove(task);
                        }
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

        public static void ClearAll()
        {
            for (int i = _historyTasks.Count - 1; i >= 0; i--)
            {
                var state = _historyTasks[i].State;
                if (state == EditorTaskState.Completed ||
                    state == EditorTaskState.Failed)
                    _historyTasks.RemoveAt(i);
            }
        }

        public static void Clear(IEditorTask task)
        {
            _historyTasks.Remove(task);
        }
    }
}
#endif