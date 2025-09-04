#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OortTools.Core.Utils
{
    public static class EditorProgress
    {
        /// <summary>
        /// ��� ������ ProgressBar�� ǥ���մϴ�.
        /// </summary>
        /// <param name="title">ProgressBar �����쿡 ǥ�õ� ����</param>
        /// <param name="info">���� �۾� ���¸� �����ϴ� ���ڿ�</param>
        /// <param name="progressNormalized">0.0 ~ 1.0</param>
        /// <returns>����ڰ� Cancel�� ������ true</returns>
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