#if UNITY_EDITOR
using OortTools.Core.Preferences;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace OortTools.Core.Utils
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
                if (EditorUtility.DisplayDialog("CSV ���� �Ϸ�", $"������ ����Ǿ����ϴ�.\n{path}\n\n���� ��ġ�� �����?", "����", "�ݱ�"))
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