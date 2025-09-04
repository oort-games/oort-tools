#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OortTools.Core.Utils
{
    public static class EditorProgress
    {
        /// <summary>
        /// 취소 가능한 ProgressBar를 표시합니다.
        /// </summary>
        /// <param name="title">ProgressBar 윈도우에 표시될 제목</param>
        /// <param name="info">현재 작업 상태를 설명하는 문자열</param>
        /// <param name="progressNormalized">0.0 ~ 1.0</param>
        /// <returns>사용자가 Cancel을 누르면 true</returns>
        public static bool Show(string title, string info, float progressNormalized)
        {
            return EditorUtility.DisplayCancelableProgressBar(
                title, info, Mathf.Clamp01(progressNormalized));
        }

        public static bool Show(string title, string info, int current, int total)
        {
            float t = (total <= 0) ? 0f : (float)current / total;
            return Show(title, info, t);
        }

        public static void Clear() => EditorUtility.ClearProgressBar();
    }
}
#endif