using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CsvReader
{
    private const string SplitRe = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
    private static readonly char[] TrimChars = {'\"'};

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        var data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LineSplitRe);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SplitRe);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SplitRe);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                var value = values[j];
                value = value.TrimStart(TrimChars).TrimEnd(TrimChars).Replace("\\", "");

                object finalValue = value;

                if (int.TryParse(value, out var intValue))
                {
                    finalValue = intValue;
                }
                else if (float.TryParse(value, out var floatValue))
                {
                    finalValue = floatValue;
                }

                entry[header[j]] = finalValue;
            }

            list.Add(entry);
        }

        return list;
    }
}