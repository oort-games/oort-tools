#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace OortTools
{
    public class CsvExporterExample : EditorWindow
    {
        List<string> _header;
        List<string[]> _rows;

        [MenuItem("Oort Tools/Examples/CSV Exporter Example")]
        public static void Open()
        {
            GetWindow<CsvExporterExample>("CSV Exporter Example");
        }

        void OnEnable()
        {
            _header = new List<string> { "ID", "Name", "Score" };
            _rows = new List<string[]>
        {
            new[] { "1", "Alice", "100" },
            new[] { "2", "Bob, Jr.", "95" },
            new[] { "3", "He said \"Hi\"", "88" },
        };
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("CSV Export Example", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("버튼을 누르면 아래 데이터를 CSV 파일로 내보냅니다.\n저장 후 파일 위치를 자동으로 열려면 Preference/Oort Tools에서 설정 가능합니다.", MessageType.Info);

            if (GUILayout.Button("Export Sample CSV", GUILayout.Height(30)))
            {
                CsvExporter.Export("Example", _header, _rows);
            }

            GUILayout.Space(10);
            GUILayout.Label("Preview:");
            GUILayout.Label(string.Join(",", _header.Select(CsvExporter.EscapeCsvField)));
            foreach (var row in _rows)
            {
                GUILayout.Label(string.Join(",", row.Select(CsvExporter.EscapeCsvField)));
            }
        }
    }
}

#endif
