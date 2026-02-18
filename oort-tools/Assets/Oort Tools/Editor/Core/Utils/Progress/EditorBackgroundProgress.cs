#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public class EditorBackgroundProgress : IDisposable
    {
        readonly int _progressId;
        readonly string _info;
        readonly List<Action> _tasks;
        readonly Action _onCompleted;
        readonly Action _onCanceled;

        int _currentIndex;
        bool _isRunning = true;
        bool _isFinished;

        public bool IsFinished => _isFinished;

        public EditorBackgroundProgress(string title, string info, List<Action> tasks, Action onCompleted, Action onCanceled)
        {
            if (tasks == null || tasks.Count == 0) return;

            _tasks = tasks;
            _onCompleted = onCompleted;
            _onCanceled = onCanceled;
            _info = info;

            _progressId = Progress.Start(title);

            Progress.RegisterCancelCallback(_progressId, () =>
            {
                Cancel();
                return true;
            });

            Progress.RegisterPauseCallback(_progressId, isPause =>
            {
                _isRunning = !isPause;
                return true;
            });

            EditorBackgroundProgressRunner.Register(this);
        }

        public void RunTask()
        {
            if (_isFinished || !_isRunning)
                return;

            if (_currentIndex >= _tasks.Count)
            {
                Complete();
                return;
            }

            try
            {
                float progress = (float)(_currentIndex + 1) / _tasks.Count;

                Progress.Report(_progressId, progress, $"{_info} Task #{_currentIndex}");

                _tasks[_currentIndex]?.Invoke();
                _currentIndex++;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Cancel();
            }
        }

        void Complete()
        {
            if (_isFinished) return;

            _isFinished = true;
            _onCompleted?.Invoke();
            Dispose();
        }

        void Cancel()
        {
            if (_isFinished) return;

            _isFinished = true;
            _onCanceled?.Invoke();
            Dispose();
        }

        public void Dispose()
        {
            Progress.Remove(_progressId);
            EditorBackgroundProgressRunner.Unregister(this);
        }
    }
}
#endif