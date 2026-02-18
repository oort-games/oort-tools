#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace OortTools
{
    public static class EditorTaskRunner
    {
        static IEnumerator _routine;

        public static void Start(IEditorTask task)
        {
            if (task == null)
                return;

            _routine = task.Execute();

            EditorApplication.update -= Update;
            EditorApplication.update += Update;            
        }

        static void Update()
        {
            if (_routine == null)
                return;

            bool hasNext;

            try
            {
                hasNext = _routine.MoveNext();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                hasNext = false;
            }

            if (!hasNext)
            {
                Stop();
            }
        }

        static void Stop()
        {
            EditorApplication.update -= Update;
            _routine = null;

            EditorTaskScheduler.NotifyTaskFinished();
        }
    }
}
#endif
