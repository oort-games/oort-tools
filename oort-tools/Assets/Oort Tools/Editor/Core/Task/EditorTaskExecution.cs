#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OortTools
{
    public class EditorTaskExecution
    {
        readonly Stack<IEnumerator> _stack = new();

        public EditorTaskExecution(IEnumerator root)
        {
            if (root != null)
                _stack.Push(root);
        }

        public bool MoveNext()
        {
            while (_stack.Count > 0)
            {
                var top = _stack.Peek();

                if (!top.MoveNext())
                {
                    _stack.Pop();
                    continue;
                }

                if (top.Current is IEnumerator nested)
                {
                    if (nested != null)
                        _stack.Push(nested);

                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
#endif