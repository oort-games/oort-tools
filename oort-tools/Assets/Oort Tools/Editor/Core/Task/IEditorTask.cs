#if UNITY_EDITOR
using System.Collections;

namespace OortTools
{
    public interface IEditorTask
    {
        string DisplayName { get; }
        int Priority { get; }

        bool IsCanceled { get; }

        void Cancel();

        IEnumerator Execute();
    }
}
#endif