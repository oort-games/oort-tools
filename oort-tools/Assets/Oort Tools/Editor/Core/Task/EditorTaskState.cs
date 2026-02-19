#if UNITY_EDITOR
using UnityEngine;

namespace OortTools
{
    public enum EditorTaskState
    {
        Queued,
        Running,
        Paused,
        Completed,
        Canceled,
        Failed
    }
}
#endif