using UnityEngine;
using System.Globalization;

namespace Avrahamy.Utils {
    public static class ColorUtils {
        /// <summary>
        /// Supports formats 0xAABBCCDD or #AABBCCDD with or without alpha channel.
        /// </summary>
        public static Color HexToColor(this string hex) {
            if (hex.StartsWith("Ox")) {
                hex = hex.Substring(2);
            } else if (hex.StartsWith("#")) {
                hex = hex.Substring(1);
            }
            var a = hex.Length == 8 ? byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) : (byte)255;
            var r = byte.Parse(hex.Substring(0,2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2,2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4,2), NumberStyles.HexNumber);
            return new Color32(r,g,b,a);
        }

        public static string ToHex(this Color color) {
            var color32 = (Color32)color;
            return color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2") + color32.a.ToString("X2");
        }
    }
}
