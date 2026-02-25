#if UNITY_EDITOR
using OortTools;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OortTools
{
    public class RandomTextureDownloader : EditorWindow
    {
        [MenuItem("Oort Tools/Texture/Random Texture Downloader", false, 100)]
        public static void Open()
        {
            GetWindow<RandomTextureDownloader>("Random Texture Downloader");
        }

        void OnGUI()
        {
            if (GUILayout.Button("Download Sample Images", GUILayout.Height(30)))
            {
                var urls = new List<string>
                {
                    "https://picsum.photos/800/600",
                    "https://picsum.photos/800/600?random=1",
                    "https://picsum.photos/800/600?random=2",
                    "https://picsum.photos/800/600?random=3",
                    "https://picsum.photos/800/600?random=4",
                };

                string saveFolder = Path.Combine(Application.dataPath, "DownloadedImages");
                var task = new UrlBatchDownloadTask(urls, saveFolder, "800x600", "jpg", false);
                EditorTaskRunner.Start(task);
            }
        }
    }
}
#endif