#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using OortTools.CoreSystem.UI;

public class TreeViewExample : EditorWindow
{
    private TreeViewState _treeViewState;
    private MultiColumnHeader _header;
    private ExampleTreeView _treeView;

    private string _searchText = "";

    [MenuItem("Oort Tools/Examples/TreeView Example")]
    public static void Open()
    {
        GetWindow<TreeViewExample>("TreeView Example");
    }

    private void OnEnable()
    {
        _treeViewState ??= new TreeViewState();

        var columns = new[]
        {
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Name"),
                width = 200,
                autoResize = true
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Score"),
                width = 80,
                autoResize = true
            },
        };
        _header ??= new MultiColumnHeader(new MultiColumnHeaderState(columns));

        _treeView = new ExampleTreeView(_treeViewState, _header);

        _treeView.OnSelectionChanged = rows =>
        {
            Debug.Log($"Selected {rows.Count} row(s)");
        };
        _treeView.OnDoubleClickedItem = row =>
        {
            Debug.Log($"DoubleClicked: {row.name} (score: {row.score})");
        };
        _treeView.OnContextClickedItem = row =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent($"{row.name}"), false, () =>
            {
                Debug.Log($"ContextClicked: {row.name}");
            });
            menu.ShowAsContext();
        };

        _treeView.SetColumnComparer(0, (a, b) => string.Compare(a.name, b.name, true));
        _treeView.SetColumnComparer(1, (a, b) => a.score.CompareTo(b.score));

        var rows = new List<ExampleRow>
        {
            new("Alice", 100),
            new("Bob", 95),
            new("Charlie", 88),
            new("David", 76),
            new("Eve", 92),
            new("Frank", 67),
            new("Grace", 81),
            new("Hannah", 73),
            new("Ian", 84),
            new("Jack", 59),
            new("Kelly", 90),
            new("Leo", 78),
            new("Mia", 85),
            new("Nina", 94),
            new("Oscar", 62),
            new("Paul", 70),
            new("Quinn", 88),
            new("Ryan", 91),
            new("Sophia", 99),
            new("Tom", 55),
            new("Uma", 83),
            new("Victor", 72),
            new("Wendy", 96),
            new("Xavier", 68),
            new("Yuna", 87),
            new("Zack", 74),
            new("Aaron", 65),
            new("Bella", 93),
            new("Chris", 80),
            new("Daisy", 77),
            new("Ethan", 89),
            new("Fiona", 60),
            new("George", 71),
            new("Hazel", 82),
            new("Ivy", 97),
            new("Jason", 66),
            new("Kara", 86),            
            new("Liam", 79),
            new("Mason", 58),
            new("Noah", 98),
            new("Olivia", 69),
            new("Parker", 75),
            new("Ruby", 84),
            new("Samuel", 63),
            new("Tina", 92),
            new("Ulysses", 57),
            new("Violet", 88),
            new("Will", 61),
            new("Zoe", 100),
            new("Alice", 82)
        };
        _treeView.SetData(rows);
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("Search", GUILayout.Width(50));
            string newSearch = GUILayout.TextField(_searchText, EditorStyles.toolbarTextField, GUILayout.MinWidth(120));
            if (newSearch != _searchText)
            {
                _searchText = newSearch;
                _treeView.SetSearch(_searchText);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear Search", EditorStyles.toolbarButton))
            {
                _searchText = string.Empty;
                _treeView.SetSearch(_searchText);
            }

            if (GUILayout.Button("Clear Sort", EditorStyles.toolbarButton))
            {
                _treeView.multiColumnHeader.sortedColumnIndex = -1;
                _treeView.SetSearch(_searchText);
            }
        }

        var rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        _treeView.OnGUI(rect);
    }

    private class ExampleRow
    {
        public string name;
        public int score;
        public ExampleRow(string name, int score) { this.name = name; this.score = score; }
        public override string ToString() => $"{name} ({score})";
    }

    private class ExampleTreeView : TreeViewBase<ExampleRow>
    {
        public ExampleTreeView(TreeViewState state, MultiColumnHeader header)
            : base(state, header) { }

        protected override string GetDisplayName(ExampleRow row, int index)
        {
            return row?.name ?? "(null)";
        }

        protected override void DrawCell(Rect cellRect, RowGUIArgs args, int visibleColumnIndex)
        {
            var row = GetRowByItemId(args.item.id);
            if (row == null) return;

            switch (visibleColumnIndex)
            {
                case 0: // Name
                    EditorGUI.LabelField(cellRect, row.name);
                    break;
                case 1: // Score
                    EditorGUI.LabelField(cellRect, row.score.ToString());
                    break;
                default:
                    base.DrawCell(cellRect, args, visibleColumnIndex);
                    break;
            }
        }
    }
}
#endif
