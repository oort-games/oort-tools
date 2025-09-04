#if UNITY_EDITOR
using OortTools.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CsvExporterExample : EditorWindow
{
    private List<string> _header;
    private List<string[]> _rows;

    [MenuItem("Oort Tools/Examples/CSV Exporter Example")]
    public static void Open()
    {
        GetWindow<CsvExporterExample>("CSV Exporter Example");
    }

    private void OnEnable()
    {
        _header = new List<string> { "ID", "Name", "Score" };
        _rows = new List<string[]>
        {
            new[] { "1", "Alice", "100" },
            new[] { "2", "Bob, Jr.", "95" },
            new[] { "3", "He said \"Hi\"", "88" },
        };
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("CSV Export Example", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("이 예제는 간단한 데이터를 CSV 파일로 내보내는 샘플입니다.", MessageType.Info);

        if (GUILayout.Button("Export Sample CSV", GUILayout.Height(30)))
        {
            CsvExporter.Export("sample_result", _header, _rows);
        }

        GUILayout.Space(10);
        GUILayout.Label("Preview:");
        GUILayout.Label(string.Join(",", _header.Select(CsvExporter.EscapeCsvField)));
        foreach (var row in _rows)
            GUILayout.Label(string.Join(",", row.Select(CsvExporter.EscapeCsvField)));
    }
}
#endif
