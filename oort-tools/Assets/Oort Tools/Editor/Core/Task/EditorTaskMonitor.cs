#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OortTools
{
    public class EditorTaskMonitor : EditorWindow
    {
        [MenuItem("Oort Tools/Task Monitor (UI Toolkit)")]
        public static void Open()
        {
            var window = GetWindow<EditorTaskMonitor>();
            window.titleContent = new GUIContent("Task Monitor");
            window.minSize = new Vector2(320, 400);
        }

        ScrollView _scroll;
        TextField _searchField;

        readonly Dictionary<IEditorTask, TaskCard> _activeCards = new();

        void OnEnable()
        {
            CreateUI();
            EditorApplication.update += RefreshUI;
        }

        void OnDisable()
        {
            EditorApplication.update -= RefreshUI;
        }

        void CreateUI()
        {
            rootVisualElement.Clear();

            var toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.paddingBottom = 4;
            toolbar.style.borderBottomWidth = 1;
            toolbar.style.borderBottomColor = new Color(0.25f, 0.25f, 0.25f);

            var cancelAllBtn = new Button(() => EditorTaskRunner.CancelAll())
            { text = "Cancel All" };

            _searchField = new TextField { label = "Search" };
            _searchField.style.flexGrow = 1;

            toolbar.Add(cancelAllBtn);
            toolbar.Add(_searchField);

            rootVisualElement.Add(toolbar);

            _scroll = new ScrollView();
            _scroll.style.flexGrow = 1;
            rootVisualElement.Add(_scroll);
        }

        void RefreshUI()
        {
            if (_scroll == null)
                return;

            var currentTasks = EditorTaskRunner.RunningTasks.ToList();
            string searchText = _searchField.value?.ToLower() ?? "";

            // 제거된 카드 정리
            var toRemove = _activeCards.Keys
                .Where(t => !currentTasks.Contains(t))
                .ToList();

            foreach (var task in toRemove)
            {
                _activeCards[task].Root.RemoveFromHierarchy();
                _activeCards.Remove(task);
            }

            UpdateTaskTree(currentTasks, _scroll, 0, searchText);
        }

        void UpdateTaskTree(
            IEnumerable<IEditorTask> tasks,
            VisualElement parent,
            int depth,
            string filter)
        {
            foreach (var task in tasks)
            {
                if (!_activeCards.TryGetValue(task, out var card))
                {
                    card = new TaskCard(task, depth);
                    _activeCards.Add(task, card);
                }

                if (card.Root.parent != parent)
                {
                    card.Root.RemoveFromHierarchy();
                    parent.Add(card.Root);
                }

                bool visible = string.IsNullOrEmpty(filter) ||
                               task.DisplayName.ToLower().Contains(filter);

                card.Root.style.display =
                    visible ? DisplayStyle.Flex : DisplayStyle.None;

                card.Update();

                if (task is EditorTask editorTask &&
                    editorTask.Child != null &&
                    editorTask.Child.Count > 0)
                {
                    UpdateTaskTree(
                        editorTask.Child,
                        card.ChildContainer,
                        depth + 1,
                        filter);
                }
            }
        }

        // ==========================================================
        // ======================== TASK CARD ========================
        // ==========================================================

        class TaskCard
        {
            public VisualElement Root;
            public VisualElement ChildContainer => _childContainer;

            Label _percent;
            Label _sub;
            ProgressBar _progress;

            Button _btnPause;
            Button _btnResume;
            Button _btnCancel;

            VisualElement _childContainer;

            IEditorTask _task;
            EditorTask _editorTask;
            bool _isParent;

            public TaskCard(IEditorTask task, int depth)
            {
                _task = task;
                _editorTask = task as EditorTask;
                _isParent = _editorTask?.Child?.Count > 0;

                // ================= ROOT =================
                Root = new VisualElement();
                Root.style.marginLeft = depth * 16;
                Root.style.marginBottom = 4;
                Root.style.paddingLeft = 8;
                Root.style.paddingRight = 8;
                Root.style.paddingTop = 6;
                Root.style.paddingBottom = 6;
                Root.style.backgroundColor =
                    _isParent
                    ? new Color(0.23f, 0.23f, 0.23f)
                    : new Color(0.18f, 0.18f, 0.18f);
                Root.style.borderBottomWidth = 1;
                Root.style.borderBottomColor = Color.black;

                // 부모는 클릭 무시
                Root.pickingMode = PickingMode.Ignore;

                // ================= HEADER =================
                var header = new VisualElement();
                header.style.flexDirection = FlexDirection.Row;
                header.pickingMode = PickingMode.Ignore;

                var title = new Label(task.DisplayName);
                title.style.unityFontStyleAndWeight =
                    _isParent ? FontStyle.Bold : FontStyle.Normal;
                title.style.flexGrow = 1;

                header.Add(title);

                _percent = new Label();
                _percent.style.marginRight = 6;
                header.Add(_percent);

                // ================= BUTTONS =================
                _btnPause = new Button(() => _editorTask?.Pause())
                { text = "Ⅱ" };

                _btnResume = new Button(() => _editorTask?.Resume())
                { text = "▶" };

                _btnCancel = new Button(() => task.Cancel())
                { text = "✕" };

                _btnPause.tooltip = "Pause";
                _btnResume.tooltip = "Resume";
                _btnCancel.tooltip = "Cancel";

                _btnCancel.style.color = Color.red;

                // 버튼만 클릭 허용
                _btnPause.pickingMode = PickingMode.Position;
                _btnResume.pickingMode = PickingMode.Position;
                _btnCancel.pickingMode = PickingMode.Position;

                header.Add(_btnPause);
                header.Add(_btnResume);
                header.Add(_btnCancel);

                Root.Add(header);

                // ================= SUB =================
                _sub = new Label();
                _sub.style.fontSize = 11;
                _sub.style.color = Color.gray;
                _sub.pickingMode = PickingMode.Ignore;

                Root.Add(_sub);

                // ================= PROGRESS =================
                _progress = new ProgressBar
                {
                    lowValue = 0,
                    highValue = 100
                };

                _progress.style.height = 12;
                _progress.pickingMode = PickingMode.Ignore;

                Root.Add(_progress);

                // ================= CHILD CONTAINER =================
                _childContainer = new VisualElement();
                _childContainer.style.marginTop = 4;
                _childContainer.pickingMode = PickingMode.Ignore;

                Root.Add(_childContainer);
            }

            public void Update()
            {
                float progress = GetAggregatedProgress();

                _percent.text =
                    $"{Mathf.RoundToInt(progress * 100f)}%";

                _progress.value = progress * 100f;

                if (_editorTask == null)
                    return;

                if (!_isParent)
                    _sub.text = _editorTask.SubMessage;

                bool isRunning =
                    _editorTask.State == EditorTaskState.Running;

                bool isPaused =
                    _editorTask.State == EditorTaskState.Paused;

                bool isFinished =
                    _editorTask.State == EditorTaskState.Completed ||
                    _editorTask.State == EditorTaskState.Failed;

                _btnPause.style.display =
                    (isRunning && !isPaused)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                _btnResume.style.display =
                    isPaused
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                _btnCancel.style.display =
                    (!isFinished)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }

            float GetAggregatedProgress()
            {
                if (!_isParent)
                    return _editorTask?.Progress ?? 0f;

                var children = _editorTask.Child;
                if (children == null || children.Count == 0)
                    return 0f;

                float sum =
                    children.OfType<EditorTask>()
                            .Sum(child => child.Progress);

                return sum / children.Count;
            }
        }
    }
}
#endif