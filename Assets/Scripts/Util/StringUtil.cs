using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtil
{
    public static string ColorWrap(string s, Palette color)
    {
        string colorHex = PaletteUtil.Get(color);
        return $"<color={colorHex}>{s}</color>";
    }

    public static string GetBinaryStatic(int length = 10)
    {
        List<string> result = new List<string>();
        var rand = new System.Random();
        for (int i = 0; i < length; i++)
        {
            result.Add(Convert.ToString(rand.Next(1024), 2).PadLeft(length, '0'));
        }
        return string.Join("\n", result);
    }

    public static string FormatStringWithDict(string result, Dictionary<string, string> dict)
    {
        foreach (KeyValuePair<string, string> entry in dict)
        {
            result = result.Replace($"[{entry.Key}]", entry.Value);
        }
        return result;
    }
}