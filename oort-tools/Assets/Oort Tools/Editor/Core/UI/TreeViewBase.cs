#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace OortTools.CoreSystem.UI
{
    public abstract class TreeViewBase<TRow> : TreeView where TRow : class
    {
        #region Fields
        protected readonly List<TRow> _data = new();
        protected readonly List<TRow> _visible = new();

        public string SearchText { get; private set; } = string.Empty;
        protected Func<TRow, string, bool> _searchPredicate = (row, text) =>
            string.IsNullOrEmpty(text) ||
            (row?.ToString()?.IndexOf(text, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;

        protected readonly Dictionary<int, Comparison<TRow>> _columnComparers = new();
        #endregion

        #region Events (Delegates)
        public Action<IList<TRow>> OnSelectionChanged;
        public Action<TRow> OnDoubleClickedItem;
        public Action<TRow> OnContextClickedItem;
        #endregion

        #region Constructor & Init
        protected TreeViewBase(TreeViewState state, MultiColumnHeader header) :base(state, header)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            rowHeight = 20f;

            header.sortingChanged += OnSortingChanged;
        }
        #endregion

        #region Public API (Data / Search / Sort)
        public void SetColumnComparer(int columnIndex, Comparison<TRow> comparer)
        {
            if (comparer == null) _columnComparers.Remove(columnIndex);
            else _columnComparers[columnIndex] = comparer;
        }

        public void SetData(IEnumerable<TRow> rows)
        {
            _data.Clear();
            if (rows != null) _data.AddRange(rows);
            RebuildVisible();
        }

        public void SetSearch(string text)
        {
            SearchText = text ?? string.Empty;
            RebuildVisible();
        }

        public void SetSearchPredicate(Func<TRow, string, bool> predicate)
        {
            if (predicate != null) _searchPredicate = predicate;
            RebuildVisible();
        }
        #endregion

        #region Rebuild & Sorting
        private void OnSortingChanged(MultiColumnHeader _)
        {
            RebuildVisible();
        }

        protected virtual void RebuildVisible()
        {
            _visible.Clear();
            foreach (var r in _data)
            {
                if (_searchPredicate(r, SearchText))
                    _visible.Add(r);
            }
            ApplySorting();
            Reload();
        }

        protected virtual void ApplySorting()
        {
            int sortedIndex = multiColumnHeader.sortedColumnIndex;
            if (sortedIndex < 0) return;

            if (_columnComparers.TryGetValue(sortedIndex, out var comparer) && comparer != null)
            {
                bool ascending = true;
                var cols = multiColumnHeader.state.columns;
                if (sortedIndex >= 0 && sortedIndex < cols.Length)
                    ascending = cols[sortedIndex].sortedAscending;

                _visible.Sort((a, b) => ascending ? comparer(a, b) : comparer(b, a));
            }
        }
        #endregion

        #region Tree Build
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var children = new List<TreeViewItem>(_visible.Count);
            for (int i = 0; i < _visible.Count; i++)
            {
                var display = GetDisplayName(_visible[i], i);
                children.Add(new TreeViewItem { id = i + 1, depth = 0, displayName = display });
            }
            root.children = children;

            return root;
        }

        protected abstract string GetDisplayName(TRow row, int index);
        #endregion

        #region Rendering (RowGUI / Cells)
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                int columnIndex = args.GetColumn(i);
                DrawCell(rect, args, columnIndex);
            }            
        }

        protected virtual void DrawCell(Rect cellRect, RowGUIArgs args, int visibleColumnIndex)
        {
            EditorGUI.LabelField(cellRect, args.item.displayName);
        }
        #endregion

        #region Helpers
        protected TRow GetRowByItemId(int itemId)
        {
            int idx = itemId - 1;
            if (idx < 0 || idx >= _visible.Count) return null;
            return _visible[idx];
        }
        #endregion

        #region Event Overrides (Selection / DoubleClick / Context)
        // SelectionChanged : 선택 변경(클릭/Shift/Ctrl) → 선택 데이터 갱신
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (OnSelectionChanged == null) return;

            var list = new List<TRow>();
            foreach (var id in selectedIds)
            {
                var row = GetRowByItemId(id);
                if (row != null) list.Add(row);
            }
            OnSelectionChanged?.Invoke(list);
        }

        // DoubleClickedItem : 행 더블클릭
        protected override void DoubleClickedItem(int id)
        {
            var row = GetRowByItemId(id);
            if (row != null) OnDoubleClickedItem?.Invoke(row);
        }

        // ContextClickedItem : 행 우클릭 → GenericMenu로 컨텍스트 메뉴 띄우기
        protected override void ContextClickedItem(int id)
        {
            var row = GetRowByItemId(id);
            if (row != null) OnContextClickedItem?.Invoke(row);
        }
        #endregion
    }
}
#endif