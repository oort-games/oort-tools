#if UNITY_EDITOR
using System.Collections;
using UnityEditor;

namespace OortTools
{
    public abstract class EditorTask : IEditorTask
    {
        public virtual string DisplayName => GetType().Name;
        public virtual int Priority => 0;

        bool _isCanceled;
        public bool IsCanceled => _isCanceled;

        public void Cancel()
        {
            _isCanceled = true;
            OnCanceled();
        }

        protected virtual void OnCanceled() { }

        public abstract IEnumerator Execute();

        protected bool CheckCanceled()
        {
            if (_isCanceled)
            {
                return true;
            }

            return false;
        }
    }
}
#endif
