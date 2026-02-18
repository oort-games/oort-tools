#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public static class EditorProgress
    {
        /// <summary>
        /// 취소 불가능한 ProgressBar
        /// </summary>
        public static void Show(string title, string info, int current, int total)
        {
            float t = (total <= 0) ? 0f : (float)current / total;
            EditorUtility.DisplayProgressBar(title, info, Mathf.Clamp01(t));
        }

        /// <summary>
        /// 취소 가능한 ProgressBar
        /// </summary>
        public static bool ShowCancelable(string title, string info, int current, int total)
        {
            float t = (total <= 0) ? 0f : (float)current / total;
            return EditorUtility.DisplayCancelableProgressBar(title, info, Mathf.Clamp01(t));
        }

        public static void Clear() => EditorUtility.ClearProgressBar();
    }
}
#endif