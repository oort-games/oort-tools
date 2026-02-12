#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace OortTools.Core.Utils
{
    [InitializeOnLoad]
    public static class EditorBackgroundProgressRunner
    {
        static readonly List<EditorBackgroundProgress> _progresses = new();

        static bool _isSubscribed;

        static EditorBackgroundProgressRunner()
        {

        }

        public static void Register(EditorBackgroundProgress progress)
        {
            _progresses.Add(progress);

            if (!_isSubscribed)
            {
                EditorApplication.update += Update;
                _isSubscribed = true;
            }
        }

        public static void Unregister(EditorBackgroundProgress progress)
        {
            _progresses.Remove(progress);

            if (_progresses.Count == 0 && _isSubscribed)
            {
                EditorApplication.update -= Update;
                _isSubscribed = false;
            }
        }

        static void Update()
        {
            for (int i = _progresses.Count - 1; i >= 0; i--)
            {
                var p = _progresses[i];

                if (p.IsFinished)
                {
                    _progresses.RemoveAt(i);
                    continue;
                }

                p.RunTask();
            }
        }
    }
}
#endif