#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OortTools
{
    public static class CsvExporter
    {
        public static void Export(string defaultName, IEnumerable<string> header, IEnumerable<IEnumerable<string>> rows)
        {
            string path = EditorUtility.SaveFilePanel("Export CSV", "", defaultName, "csv");
            if (string.IsNullOrEmpty(path)) return;

            using (var sw = new StreamWriter(path, false, new UTF8Encoding(true)))
            {
                if (header != null)
                    sw.WriteLine(string.Join(",", header.Select(EscapeCsvField)));

                if (rows != null)
                {
                    foreach (var row in rows)
                        sw.WriteLine(string.Join(",", row.Select(EscapeCsvField)));
                }
            }

            if (PreferencesPrefs.CsvAutoOpen)
            {
                EditorUtility.RevealInFinder(path);
            }
            else
            {
                if (EditorUtility.DisplayDialog("CSV 저장 완료", $"파일이 저장되었습니다.\n{path}\n\n파일 위치를 열까요?", "열기", "닫기"))
                {
                    EditorUtility.RevealInFinder(path);
                }
            }
        }

        public static string EscapeCsvField(string s)
        {
            if (s == null) return string.Empty;

            bool needQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            if (!needQuote) return s;

            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }
    }
}
#endif