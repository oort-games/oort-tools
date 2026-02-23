#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace OortTools
{
    public static class VisualElementExtensions
    {
        public static void EnablePickingRecursively(this VisualElement element)
        {
            if (element == null) return;

            element.pickingMode = PickingMode.Position;

            foreach (var child in element.Children())
            {
                child.EnablePickingRecursively();
            }
        }

        public static void DisablePickingRecursively(this VisualElement element)
        {
            if (element == null) return;

            element.pickingMode = PickingMode.Ignore;

            foreach (var child in element.Children())
            {
                child.DisablePickingRecursively();
            }
        }

        public static void DisableProgressBarPicking(this ProgressBar progressBar)
        {
            if (progressBar == null) return;

            var bg = GetProgressBarBackground(progressBar);
            if (bg != null) bg.pickingMode = PickingMode.Ignore;

            var fill = GetProgressBarProgress(progressBar);
            if (fill != null) fill.pickingMode = PickingMode.Ignore;

            var title = GetProgressBarTitle(progressBar);
            if (title != null) title.pickingMode = PickingMode.Ignore;
        }

        public static VisualElement GetProgressBarBackground(this ProgressBar progressBar)
        {
            return progressBar.Q<VisualElement>("unity-progress-bar__background");
        }

        public static VisualElement GetProgressBarProgress(this ProgressBar progressBar)
        {
            return progressBar.Q<VisualElement>("unity-progress-bar__progress");
        }

        public static VisualElement GetProgressBarTitle(this ProgressBar progressBar)
        {
            return progressBar.Q<VisualElement>("unity-progress-bar__title");
        }
    }
}
#endif