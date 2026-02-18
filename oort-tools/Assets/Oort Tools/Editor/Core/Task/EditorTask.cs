#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public abstract class EditorTask : IEditorTask
    {
        public virtual string DisplayName => GetType().Name;
        public virtual int Priority => 0;

        public EditorTaskState State { get; protected set; } = EditorTaskState.Queued;
        public event Action<EditorTaskState> OnStateChanged;

        public EditorTask Parent { get; private set; }
        readonly List<EditorTask> _child = new();
        public IReadOnlyList<EditorTask> Child => _child;

        bool _isCanceled;

        protected void SetState(EditorTaskState state)
        {
            if (State == state)
                return;

            State = state;
            OnStateChanged?.Invoke(state);
        }

        public void AddChild(EditorTask child)
        {
            if (child == null || child == this)
                return;

            if (IsAncestor(child))
                return;

            if (_child.Contains(child))
                return;

            child.Parent?._child.Remove(child);

            child.Parent = this;
            _child.Add(child);

            _child.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        bool IsAncestor(EditorTask task)
        {
            var current = this;
            while (current != null)
            {
                if (current == task)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        public IEnumerator Execute()
        {
            if (State != EditorTaskState.Queued)
                yield break;

            if (_isCanceled)
            {
                SetState(EditorTaskState.Canceled);
                yield break;
            }

            SetState(EditorTaskState.Running);

            IEnumerator routine;

            try
            {
                routine = ExecuteTask();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SetState(EditorTaskState.Failed);
                yield break;
            }

            if (routine != null)
            {
                while (true)
                {
                    if (_isCanceled)
                    {
                        yield break;
                    }

                    bool hasNext;

                    try
                    {
                        hasNext = routine.MoveNext();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        SetState(EditorTaskState.Failed);
                        yield break;
                    }

                    if (!hasNext)
                        break;

                    yield return routine.Current;
                }
            }

            foreach (var child in _child)
            {
                if (_isCanceled)
                {
                    yield break;
                }

                yield return child.Execute();

                if (child.State == EditorTaskState.Failed)
                {
                    SetState(EditorTaskState.Failed);
                    yield break;
                }

                if (child.State == EditorTaskState.Canceled)
                {
                    _isCanceled = true;
                    yield break;
                }
            }

            if (!_isCanceled)
                SetState(EditorTaskState.Completed);
        }

        public void Cancel()
        {
            if (State == EditorTaskState.Completed ||
                State == EditorTaskState.Failed ||
                State == EditorTaskState.Canceled)
                return;
            
            _isCanceled = true;
            SetState(EditorTaskState.Canceled);

            foreach (var child in _child)
            {
                child.Cancel();
            }
        }

        protected abstract IEnumerator ExecuteTask();
    }
}
#endif
