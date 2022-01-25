using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;

namespace ImageStubber.Image.ImageGenerator
{
    public static class ImageParamsParser
    {
        public static (int Width, int Height) ParseResolution(string resolution)
        {
            var split = resolution.Split("x");

            if (split.Length != 2)
            {
                return (100, 100);
            }

            var width = split[0];
            var widthValue = 100;
            var height = split[1];
            var heightValue = 100;
            (int Width, int Height) result = (100, 100);

            if (int.TryParse(width, out widthValue))
            {
                result.Width = widthValue;
            }

            if (int.TryParse(height, out heightValue))
            {
                result.Height = heightValue;
            }

            return result;
        }

        public static ImageDescription Parse(int width, int height, string bgColor, string txtColor, string text = "")
        {
            var colorError = false;
            
            if (!ParseColor(bgColor, out var backgroundColor))
            {
                backgroundColor = Color.LightGray;
                colorError = true;

            }

            if (!ParseColor(txtColor, out var textColor))
            {
                textColor = Color.Red;
                colorError = true;
            }
            
            return new ImageDescription(width, height, backgroundColor!.Value, textColor!.Value, colorError, text);
        }

        public static bool ParseColor(string colorString, out Color? color)
        {
            try
            {
                color = ColorHelper.ParseColor(colorString);
                return true;
            }
            catch (Exception ex)
            {
                color = null;
                return false;
            }
        }
    }
    
    public static class ColorHelper
    {
        public static Color ParseColor(string cssColor)
        {
            cssColor = cssColor.Trim();

            if (cssColor.StartsWith("#"))
            {
                if (cssColor.Length <= 7)
                {
                    return ColorTranslator.FromHtml(cssColor);
                }

                var pureColor = cssColor.Substring(0, 7);

                var color = ColorTranslator.FromHtml(pureColor);

                var alphaString = cssColor.Substring(7);
                var alpha = int.Parse(alphaString, System.Globalization.NumberStyles.HexNumber);

                return Color.FromArgb(alpha, color);
            }

            if (cssColor.StartsWith("rgb")) //rgb or argb
            {
                var left = cssColor.IndexOf('(');
                var right = cssColor.IndexOf(')');

                if (left < 0 || right < 0)
                    throw new FormatException("rgba format error");
                string noBrackets = cssColor.Substring(left + 1, right - left - 1);

                string[] parts = noBrackets.Split(',');

                var r = int.Parse(parts[0]);
                var g = int.Parse(parts[1]);
                var b = int.Parse(parts[2]);

                if (parts.Length == 3)
                {
                    return Color.FromArgb(r, g, b);
                }

                if (parts.Length != 4) throw new FormatException("Not rgb, rgba or hexa color string");

                var a = double.Parse(parts[3]);

                a = Math.Round(a, 2);

                var hex = AlphaPercent[a];
                
                return Color.FromArgb(hex, r, g, b);
            }

            try
            {
                return ColorTranslator.FromHtml(cssColor);
            }
            catch
            {
                return ColorTranslator.FromHtml("#" + cssColor);
            }
        }

        private static readonly Dictionary<double, byte> AlphaPercent = new()
        {
            {1, 0xFF},
            {0.99, 0xFC},
            {0.98, 0xFA},
            {0.97, 0xF7},
            {0.96, 0xF5},
            {0.95, 0xF2},
            {0.94, 0xF0},
            {0.93, 0xED},
            {0.92, 0xEB},
            {0.91, 0xE8},
            {0.90, 0xE6},
            {0.89, 0xE3},
            {0.88, 0xE0},
            {0.87, 0xDE},
            {0.86, 0xDB},
            {0.85, 0xD9},
            {0.84, 0xD6},
            {0.83, 0xD4},
            {0.82, 0xD1},
            {0.81, 0xCF},
            {0.80, 0xCC},
            {0.79, 0xC9},
            {0.78, 0xC7},
            {0.77, 0xC4},
            {0.76, 0xC2},
            {0.75, 0xBF},
            {0.74, 0xBD},
            {0.73, 0xBA},
            {0.72, 0xB8},
            {0.71, 0xB5},
            {0.70, 0xB3},
            {0.69, 0xB0},
            {0.68, 0xAD},
            {0.67, 0xAB},
            {0.66, 0xA8},
            {0.65, 0xA6},
            {0.64, 0xA3},
            {0.63, 0xA1},
            {0.62, 0x9E},
            {0.61, 0x9C},
            {0.60, 0x99},
            {0.59, 0x96},
            {0.58, 0x94},
            {0.57, 0x91},
            {0.56, 0x8F},
            {0.55, 0x8C},
            {0.54, 0x8A},
            {0.53, 0x87},
            {0.52, 0x85},
            {0.51, 0x82},
            {0.50, 0x80},
            {0.49, 0x7D},
            {0.48, 0x7A},
            {0.47, 0x78},
            {0.46, 0x75},
            {0.45, 0x73},
            {0.44, 0x70},
            {0.43, 0x6E},
            {0.42, 0x6B},
            {0.41, 0x69},
            {0.40, 0x66},
            {0.39, 0x63},
            {0.38, 0x61},
            {0.37, 0x5E},
            {0.36, 0x5C},
            {0.35, 0x59},
            {0.34, 0x57},
            {0.33, 0x54},
            {0.32, 0x52},
            {0.31, 0x4F},
            {0.30, 0x4D},
            {0.29, 0x4A},
            {0.28, 0x47},
            {0.27, 0x45},
            {0.26, 0x42},
            {0.25, 0x40},
            {0.24, 0x3D},
            {0.23, 0x3B},
            {0.22, 0x38},
            {0.21, 0x36},
            {0.20, 0x33},
            {0.19, 0x30},
            {0.18, 0x2E},
            {0.17, 0x2B},
            {0.16, 0x29},
            {0.15, 0x26},
            {0.14, 0x24},
            {0.13, 0x21},
            {0.12, 0x1F},
            {0.11, 0x1C},
            {0.10, 0x1A},
            {0.09, 0x17},
            {0.08, 0x14},
            {0.07, 0x12},
            {0.06, 0x0F},
            {0.05, 0x0D},
            {0.04, 0x0A},
            {0.03, 0x08},
            {0.02, 0x05},
            {0.01, 0x03},
            {0.0, 0x00}
        };
    }
}