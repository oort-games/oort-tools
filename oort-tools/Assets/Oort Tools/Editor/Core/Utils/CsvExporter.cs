#if UNITY_EDITOR
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using OortTools.Core.Preferences;

namespace OortTools.Core.Utils
{
    public static class CsvExporter
    {
        public static void Export(string defaultName, string headerLine, IEnumerable<string> lines)
        {
            string path = EditorUtility.SaveFilePanel("Export CSV", "", defaultName, "csv");
            if (string.IsNullOrEmpty(path)) return;

            StringBuilder sb = new();
            if (!string.IsNullOrEmpty(headerLine))
                sb.AppendLine(headerLine);
            foreach (string line in lines)
                sb.AppendLine(line);

            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));

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
            if (s.Contains(',') || s.Contains('"')) return '"' + s.Replace("\"", "\"\"") + '"';
            return s;
        }
    }
}
#endif