#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OortTools
{
    public class EditorTaskWindow : EditorWindow
    {
        [MenuItem("Oort Tools/Core/Editor Tasks", false, 2)]
        public static void Open()
        {
            var window = GetWindow<EditorTaskWindow>("Editor Tasks");
            window.minSize = new Vector2(350, 500);
        }

        readonly Dictionary<IEditorTask, TaskVisualElement> _taskVisualElements = new();

        ScrollView _scroll;

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
            rootVisualElement.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

            #region Header
            var header = new VisualElement();
            header.style.paddingLeft = 15;
            header.style.paddingRight = 15;
            header.style.paddingTop = 15;
            header.style.paddingBottom = 15;
            header.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(0.1f, 0.1f, 0.1f);
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;

            var titleLabel = new Label("Editor Tasks");
            titleLabel.style.fontSize = 18;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.flexGrow = 1;
            titleLabel.style.color = new Color(0.9f, 0.9f, 0.9f);

            header.Add(titleLabel);
            rootVisualElement.Add(header);
            #endregion

            #region Sub Header
            var subHeader = new VisualElement();
            subHeader.style.flexDirection = FlexDirection.Row;
            subHeader.style.marginTop = 8;
            subHeader.style.justifyContent = Justify.FlexEnd;

            var clearAllBtn = new Button(() =>
            {
                EditorTaskRunner.ClearAll();
                RefreshUI();
            })
            { text = "Clear All" };
            clearAllBtn.style.marginLeft = 10;

            var cancelAllBtn = new Button(() => EditorTaskRunner.CancelAll()) { text = "Cancel All" };
            cancelAllBtn.style.backgroundColor = new Color(0.7f, 0.2f, 0.2f);

            subHeader.Add(clearAllBtn);
            subHeader.Add(cancelAllBtn);

            rootVisualElement.Add(subHeader);
            #endregion

            #region Scroll
            _scroll = new ScrollView();
            _scroll.style.flexGrow = 1;
            _scroll.style.paddingTop = 10;
            _scroll.style.paddingBottom = 10;
            _scroll.style.paddingLeft = 10;
            _scroll.style.paddingRight = 10;
            rootVisualElement.Add(_scroll);
            #endregion
        }

        void RefreshUI()
        {
            if (_scroll == null) return;

            var currentTasks = EditorTaskRunner.HistoryTasks.ToList();

            var toRemove = _taskVisualElements.Keys.Where(t => !currentTasks.Contains(t)).ToList();
            foreach (var task in toRemove)
            {
                _taskVisualElements[task].Root.RemoveFromHierarchy();
                _taskVisualElements.Remove(task);
            }

            UpdateTaskTree(currentTasks, _scroll, 0);
        }

        void UpdateTaskTree(IEnumerable<IEditorTask> tasks, VisualElement parent, int depth)
        {
            foreach (var task in tasks)
            {
                if (!_taskVisualElements.TryGetValue(task, out var taskVisualElement))
                {
                    taskVisualElement = new TaskVisualElement(task, depth);
                    _taskVisualElements.Add(task, taskVisualElement);
                }

                if (taskVisualElement.Root.parent != parent)
                {
                    taskVisualElement.Root.RemoveFromHierarchy();
                    parent.Add(taskVisualElement.Root);
                }

                taskVisualElement.Update();

                if (task is EditorTask editorTask && editorTask.Child != null && editorTask.Child.Count > 0)
                {
                    UpdateTaskTree(editorTask.Child, taskVisualElement.ChildContainer, depth + 1);
                }
            }
        }

        class TaskVisualElement
        {
            public VisualElement Root;

            readonly VisualElement _childContainer;
            public VisualElement ChildContainer => _childContainer;

            readonly Label _title;
            readonly Label _sub;
            readonly Label _percent;

            readonly VisualElement _statusDot;

            readonly Button _btnFold;
            readonly Button _btnPause;
            readonly Button _btnResume;
            readonly Button _btnCancel;
            readonly Button _btnClear;

            readonly ProgressBar _progress;

            readonly IEditorTask _task;
            readonly EditorTask _editorTask;

            readonly bool _isRootTask;
            readonly bool _hasChild;
            bool _isExpanded = true;

            public TaskVisualElement(IEditorTask task, int depth)
            {
                _task = task;
                _editorTask = task as EditorTask;
                _isRootTask = _editorTask?.Parent == null;
                _hasChild = _editorTask?.Child?.Count > 0;

                #region Root
                Root = new VisualElement();
                Root.style.marginLeft = depth > 0 ? 12 : 0;
                Root.style.marginBottom = 8;
                Root.style.paddingLeft = 10;
                Root.style.paddingRight = 10;
                Root.style.paddingTop = 8;
                Root.style.paddingBottom = 8;
                Root.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f);
                Root.style.borderLeftWidth = depth > 0 ? 2 : 0;
                Root.style.borderLeftColor = new Color(0.4f, 0.4f, 0.4f);
                Root.style.borderTopLeftRadius = 5;
                Root.style.borderBottomLeftRadius = 5;
                Root.style.borderTopRightRadius = 5;
                Root.style.borderBottomRightRadius = 5;
                #endregion

                #region TopRow
                var topRow = new VisualElement();
                topRow.style.flexDirection = FlexDirection.Row;
                topRow.style.alignItems = Align.Center;

                if (_isRootTask && _hasChild)
                {
                    _btnFold = CreateIconButton(
                        _isExpanded ? "d_IN_foldout_on" : "d_IN_foldout",
                        _isExpanded ? "Collapse children" : "Expand children",
                        ToggleFold,
                        Color.clear
                        );
                    _btnFold.style.width = 16;
                    _btnFold.style.height = 16;
                    _btnFold.style.marginRight = 4;
                    topRow.Add(_btnFold);
                }

                _statusDot = new VisualElement();
                _statusDot.style.width = 8;
                _statusDot.style.height = 8;
                _statusDot.style.marginRight = 8;
                _statusDot.style.borderTopLeftRadius = 4;
                _statusDot.style.borderBottomLeftRadius = 4;
                _statusDot.style.borderTopRightRadius = 4;
                _statusDot.style.borderBottomRightRadius = 4;
                _statusDot.style.backgroundColor = Color.cyan;

                _title = new Label(task.DisplayName);
                _title.style.unityFontStyleAndWeight = _isRootTask ? FontStyle.Bold : FontStyle.Normal;
                _title.style.flexGrow = 1;
                _title.style.fontSize = 13;

                _percent = new Label("0%");
                _percent.style.width = 35;
                _percent.style.unityTextAlign = TextAnchor.MiddleRight;
                _percent.style.marginRight = 8;
                _percent.style.unityFontStyleAndWeight = FontStyle.Bold;

                topRow.Add(_statusDot);
                topRow.Add(_title);
                topRow.Add(_percent);

                if (_isRootTask)
                {
                    var btnGroup = new VisualElement();
                    btnGroup.style.flexDirection = FlexDirection.Row;

                    _btnPause = CreateIconButton("d_PauseButton", "Pause",
                        () => _editorTask?.Pause(), new Color(0.25f, 0.25f, 0.25f));

                    _btnResume = CreateIconButton("d_PlayButton", "Resume",
                        () => _editorTask?.Resume(), new Color(0.25f, 0.25f, 0.25f));

                    _btnCancel = CreateIconButton("d_clear", "Cancel",
                        () => task.Cancel(), new Color(0.3f, 0.15f, 0.15f));

                    _btnClear = CreateIconButton("d_TreeEditor.Trash", "Clear",
                        () => EditorTaskRunner.Clear(task), new Color(0.25f, 0.25f, 0.25f));

                    btnGroup.Add(_btnPause);
                    btnGroup.Add(_btnResume);
                    btnGroup.Add(_btnCancel);
                    btnGroup.Add(_btnClear);

                    topRow.Add(btnGroup);
                }

                Root.Add(topRow);
                #endregion

                #region Sub
                _sub = new Label("");
                _sub.style.fontSize = 10;
                _sub.style.color = new Color(0.6f, 0.6f, 0.6f);
                _sub.style.marginTop = 2;
                _sub.style.marginBottom = 4;
                _sub.style.paddingLeft = 16;

                Root.Add(_sub);
                #endregion

                #region Progress
                _progress = new ProgressBar { lowValue = 0, highValue = 100 };
                _progress.style.height = 6;
                _progress.style.marginTop = 2;

                Root.Add(_progress);
                #endregion

                #region Child
                _childContainer = new VisualElement();
                _childContainer.style.marginTop = 6;
                Root.Add(_childContainer);
                #endregion
            }

            public void Update()
            {
                float progress = GetAggregatedProgress();
                _percent.text = $"{Mathf.RoundToInt(progress * 100f)}%";
                _progress.value = progress * 100f;

                if (_editorTask == null) return;

                string subText = "";

                if (!_hasChild)
                {
                    subText = _editorTask.SubMessage;
                }
                else
                {
                    bool allWaiting = _editorTask.Child.All(c => c.State == EditorTaskState.Queued);
                    bool allCompleted = _editorTask.Child.All(c => c.State == EditorTaskState.Completed);

                    if (allWaiting)
                    {
                        subText = $"{_editorTask.DisplayName} Waiting...";
                    }
                    else if (allCompleted)
                    {
                        subText = $"{_editorTask.DisplayName} Completed!";
                    }
                    else
                    {
                        var currentRunningTask = _editorTask.Child.FirstOrDefault(c => c.State == EditorTaskState.Running || c.State == EditorTaskState.Paused);
                        if (currentRunningTask != null)
                        {
                            subText = $"{currentRunningTask.DisplayName} {currentRunningTask.State}";
                        }
                    }
                }

                double totalElapsed = GetAggregatedElapsedTime();
                string timeStr = totalElapsed.ToString("F1") + "s";

                if (!string.IsNullOrEmpty(subText))
                {
                    _sub.text = subText + "  •  " + timeStr;
                }
                else
                {
                    _sub.text = timeStr;
                }

                var state = _editorTask.State;
                _statusDot.style.backgroundColor = state switch
                {
                    EditorTaskState.Running => new Color(0.2f, 0.8f, 1f),
                    EditorTaskState.Paused => new Color(1f, 0.7f, 0.1f),
                    EditorTaskState.Failed => Color.red,
                    EditorTaskState.Completed => Color.green,
                    _ => Color.gray
                };

                if (_isRootTask)
                {
                    _btnPause.style.display = (state == EditorTaskState.Running) ? DisplayStyle.Flex : DisplayStyle.None;
                    _btnResume.style.display = (state == EditorTaskState.Paused) ? DisplayStyle.Flex : DisplayStyle.None;
                    _btnCancel.style.display = (state != EditorTaskState.Completed && state != EditorTaskState.Failed) ? DisplayStyle.Flex : DisplayStyle.None;
                    _btnClear.style.display = (state == EditorTaskState.Completed || state == EditorTaskState.Failed) ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }

            Button CreateIconButton(string iconName, string tooltip, System.Action onClick, Color bgColor)
            {
                var icon = EditorGUIUtility.IconContent(iconName)?.image as Texture2D;

                var btn = new Button(onClick)
                {
                    tooltip = tooltip,
                    iconImage = icon,
                    pickingMode = PickingMode.Position
                };

                btn.style.width = 24;
                btn.style.height = 24;
                btn.style.marginLeft = 2;
                btn.style.marginRight = 0;
                btn.style.paddingLeft = 0;
                btn.style.paddingRight = 0;
                btn.style.backgroundColor = bgColor;

                return btn;
            }

            float GetAggregatedProgress()
            {
                if (_editorTask == null) return 0f;

                var (totalProgress, totalCount) = CalculateProgressRecursive(_editorTask);
                return totalCount > 0 ? totalProgress / totalCount : 0f;
            }

            (float progressSum, int count) CalculateProgressRecursive(EditorTask task)
            {
                if (task == null) return (0f, 0);

                if (task.Child == null || task.Child.Count == 0)
                {
                    return (task.Progress, 1);
                }

                float sum = 0f;
                int cnt = 0;

                foreach (var child in task.Child.OfType<EditorTask>())
                {
                    var (childSum, childCnt) = CalculateProgressRecursive(child);
                    sum += childSum;
                    cnt += childCnt;
                }

                return (sum, cnt);
            }

            double GetAggregatedElapsedTime()
            {
                if (_editorTask == null) return 0.0;

                return CalculateElapsedRecursive(_editorTask);
            }

            double CalculateElapsedRecursive(EditorTask task)
            {
                if (task == null) return 0.0;

                double total = 0.0;

                if (task.Child != null && task.Child.Count > 0)
                {
                    foreach (var child in task.Child.OfType<EditorTask>())
                    {
                        total += CalculateElapsedRecursive(child);
                    }
                }
                else
                {
                    total += task.ElapsedTime;
                }
                return total;
            }

            void ToggleFold()
            {
                _isExpanded = !_isExpanded;

                if (_btnFold != null)
                {
                    var iconName = _isExpanded ? "d_IN_foldout_on" : "d_IN_foldout";
                    var icon = EditorGUIUtility.IconContent(iconName)?.image as Texture2D;
                    _btnFold.iconImage = icon;
                    _btnFold.tooltip = _isExpanded ? "Collapse children" : "Expand children";
                }

                if (_childContainer != null)
                {
                    _childContainer.style.display = _isExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }
    }
}
#endif