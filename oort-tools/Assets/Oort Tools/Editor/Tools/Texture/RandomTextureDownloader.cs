#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OortTools
{
    public class RandomTextureDownloader : EditorWindow
    {
        [MenuItem("Oort Tools/Texture/Random Texture Downloader", false, 100)]
        public static void Open()
        {
            var window = GetWindow<RandomTextureDownloader>("Random Texture Downloader");
            window.minSize = new Vector2(350, 500);
        }

        IntegerField _widthField;
        IntegerField _heightField;
        IntegerField _countField;

        ObjectField _folderField;

        void OnEnable()
        {
            CreateUI();
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
            header.style.marginBottom = 8;

            var titleLabel = new Label("Random Texture Downloader");
            titleLabel.style.fontSize = 18;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.flexGrow = 1;
            titleLabel.style.color = new Color(0.9f, 0.9f, 0.9f);

            header.Add(titleLabel);
            rootVisualElement.Add(header);
            #endregion

            _widthField = new IntegerField("Width") { value = 800 };
            _heightField = new IntegerField("Height") { value = 600 };
            _countField = new IntegerField("Count") { value = 5 };

            rootVisualElement.Add(_widthField);
            rootVisualElement.Add(_heightField);
            rootVisualElement.Add(_countField);

            _folderField = new("Save Folder")
            {
                objectType = typeof(DefaultAsset),
                allowSceneObjects = false
            };
            _folderField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != null)
                {
                    string path = AssetDatabase.GetAssetPath(evt.newValue);
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        _folderField.value = null;
                        Debug.LogWarning("폴더만 선택할 수 있습니다!");
                    }
                }
            });

            rootVisualElement.Add(_folderField);

            var downloadButton = new Button(StartDownload)
            {
                text = "Download Images"
            };

            downloadButton.style.marginTop = 10;
            downloadButton.style.height = 35;

            rootVisualElement.Add(downloadButton);
        }

        void StartDownload()
        {
            if (_folderField.value == null)
            {
                Debug.LogError("Folder not selected");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(_folderField.value);

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                Debug.LogError("Selected object is not a folder.");
                return;
            }

            string absolutePath = Path.Combine(
                Application.dataPath,
                assetPath.Replace("Assets/", "")
            );

            int width = Mathf.Max(1, _widthField.value);
            int height = Mathf.Max(1, _heightField.value);
            int count = Mathf.Max(1, _countField.value);

            var urls = GenerateUrls(width, height, count);

            var task = new UrlBatchDownloadTask(
                "Random Texture Downloader",
                urls,
                absolutePath,
                $"{width}x{height}",
                "jpg",
                false
            );

            EditorTaskRunner.Start(task);
        }

        List<string> GenerateUrls(int width, int height, int count)
        {
            var list = new List<string>();

            for (int i = 0; i < count; i++)
            {
                list.Add($"https://picsum.photos/{width}/{height}?random={i}");
            }

            return list;
        }
    }
}

#endif