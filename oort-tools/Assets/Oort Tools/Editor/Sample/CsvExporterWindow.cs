#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OortTools.Core.Utils;

public class CsvExporterWindow : EditorWindow
{
    private List<string> _sampleData = new List<string>();

    [MenuItem("Oort Tools/Sample/CSV Exporter Example")]
    public static void Open()
    {
        GetWindow<CsvExporterWindow>("CSV Exporter Example");
    }

    private void OnEnable()
    {
        _sampleData.Clear();
        _sampleData.Add($"{CsvExporter.EscapeCsvField("ID")},{CsvExporter.EscapeCsvField("Name")},{CsvExporter.EscapeCsvField("Score")}");
        _sampleData.Add($"{CsvExporter.EscapeCsvField("1")},{CsvExporter.EscapeCsvField("Alice")},{CsvExporter.EscapeCsvField("100")}");
        _sampleData.Add($"{CsvExporter.EscapeCsvField("2")},{CsvExporter.EscapeCsvField("Bob, Jr.")},{CsvExporter.EscapeCsvField("95")}");
        _sampleData.Add($"{CsvExporter.EscapeCsvField("3")},{CsvExporter.EscapeCsvField("He said \"Hi\"")},{CsvExporter.EscapeCsvField("88")}");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("CSV Export Example", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("이 예제는 간단한 데이터를 CSV 파일로 내보내는 샘플입니다.", MessageType.Info);

        if (GUILayout.Button("Export Sample CSV", GUILayout.Height(30)))
        {
            string header = _sampleData[0];
            var lines = _sampleData.GetRange(1, _sampleData.Count - 1);

            CsvExporter.Export("sample_result", header, lines);
        }

        GUILayout.Space(10);
        GUILayout.Label("Preview:");
        foreach (var line in _sampleData)
        {
            GUILayout.Label(line);
        }
    }
}
#endif
