#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace OortTools
{
    public class UrlBatchDownloadTask : EditorTask
    {
        readonly string _name;

        readonly List<string> _urls;
        readonly string _saveFolderPath;
        readonly string _baseName;
        readonly string _defaultExtension;
        readonly bool _useOriginalFileName;

        UnityWebRequest _currentRequest;

        bool _refreshPending = false;

        static readonly object _fileNameLock = new();
        static readonly HashSet<string> _reservedPath = new();

        public override string DisplayName => _name;

        public UrlBatchDownloadTask(string name, List<string> urls, string saveFolderPath, string baseName, string defaultExtension, bool useOriginalFileName = false)
        {
            _name = name;

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
                    AbortCurrentRequest();
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
                if (State == EditorTaskState.Canceled)
                    yield break;

                while (State == EditorTaskState.Paused)
                    yield return null;

                string url = _urls[i];
                string fileName = GetUniqueFileName(url);
                string fullPath = Path.Combine(_saveFolderPath, fileName);

                SetSubMessage($"[{i + 1}/{_urls.Count}] {fileName}");

                bool success = false;
                yield return DownloadFile(url, fullPath, s => success = s);

                ReleaseReservedPath(fullPath);

                if (success) successCount++;
                else failCount++;

                SetProgress((float)(i + 1) / _urls.Count);
            }

            SetProgress(1f);
            SetSubMessage($"완료! 성공: {successCount}, 실패: {failCount}");

            if (_saveFolderPath.StartsWith(Application.dataPath))
            {
                if (!_refreshPending)
                {
                    _refreshPending = true;
                    EditorApplication.delayCall += () =>
                    {
                        AssetDatabase.Refresh();
                        _refreshPending = false;
                    };
                }
            }
        }

        IEnumerator DownloadFile(string url, string savePath, System.Action<bool> onFinished)
        {
            _currentRequest = UnityWebRequest.Get(url);
            _currentRequest.timeout = 60;
            _currentRequest.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            yield return _currentRequest.SendWebRequest();

            while (_currentRequest != null && !_currentRequest.isDone)
            {
                if (State == EditorTaskState.Canceled)
                {
                    onFinished?.Invoke(false);
                    yield break;
                }
                yield return null;
            }

            bool success = false;

            if (_currentRequest != null)
            {
                if (_currentRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        File.WriteAllBytes(savePath, _currentRequest.downloadHandler.data);
                        Debug.Log($"저장 완료: {savePath}");
                        success = true;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"파일 저장 실패: {savePath} → {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"다운로드 실패 - URL: {url} | Result: {_currentRequest.result} | Code: {_currentRequest.responseCode} | Error: '{_currentRequest.error}'");
                }

                _currentRequest.Dispose();
                _currentRequest = null;
            }

            onFinished?.Invoke(success);
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
            lock (_fileNameLock)
            {
                string cleanUrl = url.Split('?')[0];
                string originalName = Path.GetFileNameWithoutExtension(cleanUrl);
                string ext = Path.GetExtension(cleanUrl);

                if (string.IsNullOrEmpty(ext)) ext = _defaultExtension;
                if (string.IsNullOrEmpty(originalName)) originalName = "file";

                string baseFileName = _useOriginalFileName
                    ? $"{_baseName}_{originalName}"
                    : _baseName;

                string uniqueFileName = PathUtility.GetUniqueFileName(_saveFolderPath, baseFileName, ext, _reservedPath);
                string fullPath = Path.Combine(_saveFolderPath, uniqueFileName);
                _reservedPath.Add(fullPath);

                return uniqueFileName;
            }
        }

        void ReleaseReservedPath(string path)
        {
            lock (_fileNameLock)
            {
                if (_reservedPath.Contains(path))
                {
                    _reservedPath.Remove(path);
                }
            }
        }
    }
}
#endif