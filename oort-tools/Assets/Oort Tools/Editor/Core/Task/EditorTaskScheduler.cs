#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace OortTools
{
    [InitializeOnLoad]
    public class EditorTaskScheduler
    {
        static readonly List<IEditorTask> _tasks = new();
        
        static IEditorTask _currentTask;
        static bool _isRunning;
        static bool _waitOneFrame;

        public static IEditorTask CurrentTask => _currentTask;
        public static bool IsRunning => _isRunning;
        

        static EditorTaskScheduler() 
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static void Enqueue(IEditorTask task)
        {
            if (task == null)
                return;

            _tasks.Add(task);
            SortTask();
        }

        public static void CancelCurrent()
        {
            _currentTask?.Cancel();
        }

        static void SortTask()
        {
            _tasks.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        static void TryRunNext()
        {
            if (_isRunning)
                return;

            if (_waitOneFrame)
            {
                _waitOneFrame = false;
                return;
            }

            if (_tasks.Count == 0)
                return;

            _currentTask = _tasks[0];
            _tasks.RemoveAt(0);

            _isRunning = true;
            EditorTaskRunner.Start(_currentTask);
        }

        static void Update()
        {
            if (!_isRunning)
                TryRunNext();
        }

        internal static void NotifyTaskFinished()
        {
            _currentTask = null;
            _isRunning = false;

            _waitOneFrame = true;
        }
    }
}
#endif