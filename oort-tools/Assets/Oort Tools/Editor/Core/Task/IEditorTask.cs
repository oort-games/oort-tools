#if UNITY_EDITOR
using System;
using System.Collections;

namespace OortTools
{
    public interface IEditorTask
    {
        string DisplayName { get; }
        int Priority { get; }

        EditorTaskState State { get; }
        event Action<EditorTaskState> OnStateChanged;

        IEnumerator Execute();
        void Cancel();
    }
}
#endif