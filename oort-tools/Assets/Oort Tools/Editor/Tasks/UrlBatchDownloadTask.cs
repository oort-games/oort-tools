#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace OortTools
{
    public class UrlBatchDownloadTask : EditorTask
    {
        readonly List<string> _urls;

        readonly string _saveFolderPath;
        readonly string _baseName;
        readonly string _defaultExtension;

        readonly bool _useOriginalFileName;

        UnityWebRequest _currentRequest;

        public override string DisplayName => "URL Batch Downloader";

        public UrlBatchDownloadTask(List<string> urls, string saveFolderPath, string baseName, string defaultExtension, bool useOriginalFileName)
        {
            _urls = urls ?? new List<string>();
            _saveFolderPath = saveFolderPath;
            _baseName = string.IsNullOrEmpty(baseName) ? "DownloadedFile" : baseName;
            _defaultExtension = defaultExtension;

            _useOriginalFileName = useOriginalFileName;

            if (!Directory.Exists(_saveFolderPath))
                Directory.CreateDirectory(_saveFolderPath);

            OnStateChanged += state =>
            {
                if (state == EditorTaskState.Canceled)
                {
                    AbortCurrentRequest();
                }
            };
        }

        protected override IEnumerator ExecuteTask()
        {
            if (_urls.Count == 0)
            {
                SetSubMessage("다운로드할 파일이 없습니다.");
                yield break;
            }

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < _urls.Count; i++)
            {
                if (State == EditorTaskState.Canceled) yield break;
                while (State == EditorTaskState.Paused) yield return null;

                string url = _urls[i];

                string fileName = GetUniqueFileName(url);
                string fullPath = Path.Combine(_saveFolderPath, fileName);

                SetSubMessage($"[{i + 1}/{_urls.Count}] 다운로드 중: {fileName}");

                bool success = false;
                yield return DownloadFile(url, fullPath, s => success = s);

                if (success) successCount++;
                else failCount++;

                SetProgress((float)(i + 1) / _urls.Count);
            }

            SetProgress(1f);
            SetSubMessage($"완료! (성공: {successCount}, 실패: {failCount})");

            if (_saveFolderPath.StartsWith(Application.dataPath))
                AssetDatabase.Refresh();
        }

        private IEnumerator DownloadFile(string url, string savePath, System.Action<bool> onFinished)
        {
            _currentRequest = UnityWebRequest.Get(url);
            _currentRequest.timeout = 60;

            _currentRequest.SetRequestHeader("User-Agent", "Mozilla/5.0");

            yield return _currentRequest.SendWebRequest();
            while (!_currentRequest.isDone) yield return null;

            bool success = false;

            if (_currentRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    File.WriteAllBytes(savePath, _currentRequest.downloadHandler.data);
                    success = true;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Write Fail] {savePath}: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"[Fail] Result: {_currentRequest.result} | Code: {_currentRequest.responseCode} | URL: {url}");
            }

            onFinished?.Invoke(success);
            _currentRequest.Dispose();
            _currentRequest = null;
        }

        void AbortCurrentRequest()
        {
            if (_currentRequest != null)
            {
                _currentRequest.Abort();
                _currentRequest.Dispose();
                _currentRequest = null;
            }
        }

        string GetUniqueFileName(string url)
        {
            string cleanUrl = url.Split('?')[0];
            string originalFileName = Path.GetFileNameWithoutExtension(cleanUrl);
            string extension = Path.GetExtension(cleanUrl);

            if (string.IsNullOrEmpty(extension)) extension = _defaultExtension;
            if (string.IsNullOrEmpty(originalFileName)) originalFileName = "file";

            string saveFileName;

            if (string.IsNullOrEmpty(_baseName))
            {
                saveFileName = originalFileName;
            }
            else
            {
                saveFileName = _useOriginalFileName
                    ? $"{_baseName}_{originalFileName}"
                    : _baseName;
            }

            return PathUtility.GetUniqueFileName(_saveFolderPath, saveFileName, extension);
        }
    }
}
#endif