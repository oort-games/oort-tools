#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public static class EditorTaskRunner
    {
        static readonly List<EditorTaskExecution> _running = new();
        static bool _isSubscribed;

        public static void Start(IEditorTask task)
        {
            if (task == null)
                return;

            _running.Add(new EditorTaskExecution(task.Execute()));

            if (!_isSubscribed)
            {
                EditorApplication.update += Update;
                _isSubscribed = true;
            }
        }

        static void Update()
        {
            for (int i = _running.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (!_running[i].MoveNext())
                        _running.RemoveAt(i);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    _running.RemoveAt(i);
                }
            }

            if (_running.Count == 0)
            {
                EditorApplication.update -= Update;
                _isSubscribed = false;
            }
        }
    }
}
#endif