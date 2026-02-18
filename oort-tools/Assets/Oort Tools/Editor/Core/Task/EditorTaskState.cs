#if UNITY_EDITOR
using UnityEngine;

namespace OortTools
{
    public enum EditorTaskState
    {
        Queued,
        Running,
        Completed,
        Canceled,
        Failed
    }
}
#endif