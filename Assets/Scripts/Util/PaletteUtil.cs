using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Palette
{
    WHITE, BLACK, MAGENTA, TEAL, RED, BLUE, GREEN, YELLOW, RANDOM
}

public class PaletteUtil
{
    public static string Get(Palette colorEnum) {
        switch (colorEnum)
        {
            case Palette.WHITE:
                return "#fff";
            case Palette.BLACK:
                return "#000";
            case Palette.MAGENTA:
                return "#f6019d";
            case Palette.TEAL:
                return "#2de2e6";
            case Palette.RED:
                return "#f00";
            case Palette.GREEN:
                return "#0f0";
            case Palette.BLUE:
                return "#9095ff";
            case Palette.YELLOW:
                return "#f3ea5f";
            case Palette.RANDOM:
                return GetRandomColor();
        }
        return "#fff";
    }
    public static string GetRandomColor() {
        Color color = new Color(
            UnityEngine.Random.Range(0.5f, 1f),
            UnityEngine.Random.Range(0.5f, 1f),
            UnityEngine.Random.Range(0.5f, 1f)
        );
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
}
