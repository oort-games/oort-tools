#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorProgressBackground
{
    static int _progressId;
    static string _info;
    static List<Action> _onTasks;
    static Action _onCompleted;
    static int _currentIndex = 0;
    static int _total = 0;

    static bool _isRunning = false;

    public static void Show(string title, string info, List<Action> onTasks, Action onCompleted, Action onCanceled)
    {
        if (onTasks == null || onTasks.Count == 0) return;

        _progressId = Progress.Start(title);
        _info = info;
        _onTasks = onTasks;
        _onCompleted = onCompleted;
        _currentIndex = 0;
        _total = onTasks.Count;

        Progress.RegisterCancelCallback(_progressId, () =>
        {
            onCanceled?.Invoke();
            return true;
        });
        Progress.RegisterPauseCallback(_progressId, (isPauseRequested) =>
        {
            if (isPauseRequested)
            {
                _isRunning = false;
            }
            else
            {
                _isRunning = true;
            }
            return true;
        });

        EditorApplication.update += RunTask;
        _isRunning = true;
    }

    static void RunTask()
    {
        if (_isRunning == false) return;

        if (_currentIndex >= _total)
        {
            Finish();
            _onCompleted?.Invoke();
            return;
        }

        if (Progress.GetStatus(_progressId) == Progress.Status.Canceled)
        {
            Finish();
            return;
        }

        float progress = (float)_currentIndex / _total;
        Progress.Report(_progressId, progress, $"{_info} Task #{_currentIndex}");

        _onTasks[_currentIndex]?.Invoke();
        _currentIndex++;
    }

    static void Finish()
    {
        Progress.Remove(_progressId);
        EditorApplication.update -= RunTask;
        _isRunning = false;
    }
}
#endif