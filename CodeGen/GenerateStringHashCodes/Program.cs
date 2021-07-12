// Code Generator for SvgColourConverter
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace GenerateStringHashCodes
{
    // https://foreverframe.net/c-internals-string-switch-statement/
    // https://github.com/dotnet/roslyn/blob/afd10305a37c0ffb2cfb2c2d8446154c68cfa87a/src/Compilers/CSharp/Portable/Compiler/MethodBodySynthesizer.Lowered.cs#L23
    class Program
    {
        static void Main(string[] args)
        {
            // Gey Colors

            var greyColors = new Dictionary<string, string>()
            {
                ["grey"] = "Color.Gray",
                ["lightgrey"] = "Color.LightGray",
                ["darkslategrey"] = "Color.DarkSlateGray"
            };

            var greyColorsStringBuilder = new StringBuilder();
            Generate(greyColors, "TryToGetGreyColor", "greyColor", "        ", greyColorsStringBuilder);
            Console.WriteLine(greyColorsStringBuilder.ToString());

            // SystemColors

            var systemColors = new Dictionary<string, string>()
            {
                ["activeborder"] = "SystemColors.ActiveBorder",
                ["activecaption"] = "SystemColors.ActiveCaption",
                ["appworkspace"] = "SystemColors.AppWorkspace",
                ["background"] = "SystemColors.Desktop",
                ["buttonface"] = "SystemColors.ButtonFace",
                ["buttonhighlight"] = "SystemColors.ControlLightLight",
                ["buttonshadow"] = "SystemColors.ControlDark",
                ["buttontext"] = "SystemColors.ControlText",
                ["captiontext"] = "SystemColors.ActiveCaptionText",
                ["graytext"] = "SystemColors.GrayText",
                ["highlight"] = "SystemColors.Highlight",
                ["highlighttext"] = "SystemColors.HighlightText",
                ["inactiveborder"] = "SystemColors.InactiveBorder",
                ["inactivecaption"] = "SystemColors.InactiveCaption",
                ["inactivecaptiontext"] = "SystemColors.InactiveCaptionText",
                ["infobackground"] = "SystemColors.Info",
                ["infotext"] = "SystemColors.InfoText",
                ["menu"] = "SystemColors.Menu",
                ["menutext"] = "SystemColors.MenuText",
                ["scrollbar"] = "SystemColors.ScrollBar",
                ["threeddarkshadow"] = "SystemColors.ControlDarkDark",
                ["threedface"] = "SystemColors.Control",
                ["threedhighlight"] = "SystemColors.ControlLight",
                ["threedlightshadow"] = "SystemColors.ControlLightLight",
                ["window"] = "SystemColors.Window",
                ["windowframe"] = "SystemColors.WindowFrame",
                ["windowtext"] = "SystemColors.WindowText"
            };

            var systemColorsStringBuilder = new StringBuilder();
            Generate(systemColors, "TryToGetSystemColor", "systemColor", "        ", systemColorsStringBuilder);
            Console.WriteLine(systemColorsStringBuilder.ToString());

            // Colors
            
            var dictionary = new Dictionary<string, Color>();
            GetColors(dictionary, typeof(Color));
            var colors = new Dictionary<string, string>();
  
            foreach (var (name, color) in dictionary)
            {
                colors[name.ToLower()] = $"Color.{color.Name}";
            }

            var colorsStringBuilder = new StringBuilder();
            Generate(colors, "TryToGetNamedColor", "namedColor", "        ", colorsStringBuilder);
            Console.WriteLine(colorsStringBuilder.ToString());
            
            // ARGB to Name Color

            var argbStringBuilder = new StringBuilder();

            argbStringBuilder.AppendLine($"        public static readonly Dictionary<uint, Func<Color>> ArgbToNamedColorDictionary = new Dictionary<uint, Func<Color>>()");
            argbStringBuilder.AppendLine($"        {{");

            foreach (var (name, color) in dictionary)
            {
                argbStringBuilder.AppendLine($"            [0x{color.ToArgb():X8}] = () => Color.{color.Name},");
                colors[name.ToLower()] = $"Color.{color.Name}";
            }

            argbStringBuilder.AppendLine($"        }};");
            Console.WriteLine(argbStringBuilder.ToString());
        }

        static void GetColors(Dictionary<string, Color> dictionary, Type type)
        {
            var attrs = MethodAttributes.Public | MethodAttributes.Static;
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(Color))
                {
                    var method = prop.GetGetMethod();
                    if (method != null && (method.Attributes & attrs) == attrs)
                    {
                        dictionary[prop.Name] = (Color)prop.GetValue(null, null);
                    }
                }
            }
        }

        private static uint ComputeStringHash(in ReadOnlySpan<char> text)
        {
            uint hashCode = 0;
            if (text != null)
            {
                var length = text.Length;

                hashCode = unchecked((uint)2166136261);

                int i = 0;
                goto start;

                again:
                hashCode = unchecked((text[i] ^ hashCode) * 16777619);
                i = i + 1;

                start:
                if (i < length)
                    goto again;
            }
            return hashCode;
        }

        public static void ToLowerAscii(in ReadOnlySpan<char> colour, ref Span<char> buffer)
        {
            for (int i = 0; i < colour.Length; i++)
            {
                var c = colour[i];
                switch (c)
                {
                    case 'A': buffer[i] = 'a'; break;
                    case 'B': buffer[i] = 'b'; break;
                    case 'C': buffer[i] = 'c'; break;
                    case 'D': buffer[i] = 'd'; break;
                    case 'E': buffer[i] = 'e'; break;
                    case 'F': buffer[i] = 'f'; break;
                    case 'G': buffer[i] = 'g'; break;
                    case 'H': buffer[i] = 'h'; break;
                    case 'I': buffer[i] = 'i'; break;
                    case 'J': buffer[i] = 'j'; break;
                    case 'K': buffer[i] = 'k'; break;
                    case 'L': buffer[i] = 'l'; break;
                    case 'M': buffer[i] = 'm'; break;
                    case 'N': buffer[i] = 'n'; break;
                    case 'O': buffer[i] = 'o'; break;
                    case 'P': buffer[i] = 'p'; break;
                    case 'Q': buffer[i] = 'q'; break;
                    case 'R': buffer[i] = 'r'; break;
                    case 'S': buffer[i] = 's'; break;
                    case 'T': buffer[i] = 't'; break;
                    case 'U': buffer[i] = 'u'; break;
                    case 'V': buffer[i] = 'v'; break;
                    case 'W': buffer[i] = 'w'; break;
                    case 'X': buffer[i] = 'x'; break;
                    case 'Y': buffer[i] = 'y'; break;
                    case 'Z': buffer[i] = 'z'; break;
                    default: buffer[i] = c; break;
                }
            }
        }

        private static void Generate(Dictionary<string, string> colours, string methodName, string varName, string indent, StringBuilder sb)
        {
            var results = new List<(string colour, string color, uint stringHash)>();

            foreach (var kvp in colours)
            {
                var colour = kvp.Key.AsSpan();
                var stringHash = ComputeStringHash(colour);
                results.Add((kvp.Key, kvp.Value, stringHash));
            }

            results.Sort((x, y) => x.colour.CompareTo(y.colour));

            sb.AppendLine($"{indent}public static bool {methodName}(ref ReadOnlySpan<char> colour, out Color {varName})");
            sb.AppendLine($"{indent}{{");

            sb.AppendLine($"{indent}    Span<char> buffer = stackalloc char[colour.Length];");
            sb.AppendLine($"{indent}    ToLowerAscii(colour, ref buffer);");
            sb.AppendLine($"{indent}    var stringHash = ComputeStringHash(buffer);");

            sb.AppendLine($"{indent}    switch(stringHash)");
            sb.AppendLine($"{indent}    {{");

            foreach (var result in results)
            {
                var colour = result.colour;
                var color = result.color;
                var stringHash = result.stringHash;

#if false
                sb.AppendLine($"{indent}        case 0x{stringHash:x8}: {varName} = {color}; return true; // {colour}");
#else
                sb.AppendLine($"{indent}        // {colour} = {color}");
                sb.AppendLine($"{indent}        case 0x{stringHash:x8}:");
                sb.AppendLine($"{indent}            {{");
                sb.AppendLine($"{indent}                if (buffer.SequenceEqual(\"{colour}\".AsSpan()))");
                sb.AppendLine($"{indent}                {{");
                sb.AppendLine($"{indent}                    {varName} = {color};");
                sb.AppendLine($"{indent}                    return true;");
                sb.AppendLine($"{indent}                }}");
                sb.AppendLine($"{indent}                {varName} = default;");
                sb.AppendLine($"{indent}                return false;");
                sb.AppendLine($"{indent}            }}");
#endif
            }

            sb.AppendLine($"{indent}    }}");
            sb.AppendLine($"{indent}    {varName} = default;");
            sb.AppendLine($"{indent}    return false;");

            sb.AppendLine($"{indent}}}");
        }
    }
}
