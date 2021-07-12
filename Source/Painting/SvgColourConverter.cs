using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using Svg.Helpers;

namespace Svg
{
    public static class ColorConverterHelpers
    {
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

        public static bool TryToGetGreyColor(ref ReadOnlySpan<char> colour, out Color greyColor)
        {
            Span<char> buffer = stackalloc char[colour.Length];
            ToLowerAscii(colour, ref buffer);
            var stringHash = ComputeStringHash(buffer);
            switch(stringHash)
            {
                // darkslategrey = Color.DarkSlateGray
                case 0x7fb0e019:
                    {
                        if (buffer.SequenceEqual("darkslategrey".AsSpan()))
                        {
                            greyColor = Color.DarkSlateGray;
                            return true;
                        }
                        greyColor = default;
                        return false;
                    }
                // grey = Color.Gray
                case 0xb29019a6:
                    {
                        if (buffer.SequenceEqual("grey".AsSpan()))
                        {
                            greyColor = Color.Gray;
                            return true;
                        }
                        greyColor = default;
                        return false;
                    }
                // lightgrey = Color.LightGray
                case 0x23b55208:
                    {
                        if (buffer.SequenceEqual("lightgrey".AsSpan()))
                        {
                            greyColor = Color.LightGray;
                            return true;
                        }
                        greyColor = default;
                        return false;
                    }
            }
            greyColor = default;
            return false;
        }

        public static bool TryToGetSystemColor(ref ReadOnlySpan<char> colour, out Color systemColor)
        {
            Span<char> buffer = stackalloc char[colour.Length];
            ToLowerAscii(colour, ref buffer);
            var stringHash = ComputeStringHash(buffer);
            switch(stringHash)
            {
                // activeborder = SystemColors.ActiveBorder
                case 0x96b1f469:
                    {
                        if (buffer.SequenceEqual("activeborder".AsSpan()))
                        {
                            systemColor = SystemColors.ActiveBorder;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // activecaption = SystemColors.ActiveCaption
                case 0x2cc5885f:
                    {
                        if (buffer.SequenceEqual("activecaption".AsSpan()))
                        {
                            systemColor = SystemColors.ActiveCaption;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // appworkspace = SystemColors.AppWorkspace
                case 0xb4f0f429:
                    {
                        if (buffer.SequenceEqual("appworkspace".AsSpan()))
                        {
                            systemColor = SystemColors.AppWorkspace;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // background = SystemColors.Desktop
                case 0x4babd89d:
                    {
                        if (buffer.SequenceEqual("background".AsSpan()))
                        {
                            systemColor = SystemColors.Desktop;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // buttonface = SystemColors.ButtonFace
                case 0xb8a66038:
                    {
                        if (buffer.SequenceEqual("buttonface".AsSpan()))
                        {
                            systemColor = SystemColors.ButtonFace;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // buttonhighlight = SystemColors.ControlLightLight
                case 0x9050ce9b:
                    {
                        if (buffer.SequenceEqual("buttonhighlight".AsSpan()))
                        {
                            systemColor = SystemColors.ControlLightLight;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // buttonshadow = SystemColors.ControlDark
                case 0x6ceea1b5:
                    {
                        if (buffer.SequenceEqual("buttonshadow".AsSpan()))
                        {
                            systemColor = SystemColors.ControlDark;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // buttontext = SystemColors.ControlText
                case 0xb6b04242:
                    {
                        if (buffer.SequenceEqual("buttontext".AsSpan()))
                        {
                            systemColor = SystemColors.ControlText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // captiontext = SystemColors.ActiveCaptionText
                case 0xf29413de:
                    {
                        if (buffer.SequenceEqual("captiontext".AsSpan()))
                        {
                            systemColor = SystemColors.ActiveCaptionText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // graytext = SystemColors.GrayText
                case 0x9642ba91:
                    {
                        if (buffer.SequenceEqual("graytext".AsSpan()))
                        {
                            systemColor = SystemColors.GrayText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // highlight = SystemColors.Highlight
                case 0x1c9ff127:
                    {
                        if (buffer.SequenceEqual("highlight".AsSpan()))
                        {
                            systemColor = SystemColors.Highlight;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // highlighttext = SystemColors.HighlightText
                case 0x635b6be0:
                    {
                        if (buffer.SequenceEqual("highlighttext".AsSpan()))
                        {
                            systemColor = SystemColors.HighlightText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // inactiveborder = SystemColors.InactiveBorder
                case 0xa59d8bc6:
                    {
                        if (buffer.SequenceEqual("inactiveborder".AsSpan()))
                        {
                            systemColor = SystemColors.InactiveBorder;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // inactivecaption = SystemColors.InactiveCaption
                case 0xba88bd1a:
                    {
                        if (buffer.SequenceEqual("inactivecaption".AsSpan()))
                        {
                            systemColor = SystemColors.InactiveCaption;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // inactivecaptiontext = SystemColors.InactiveCaptionText
                case 0xcb67dc11:
                    {
                        if (buffer.SequenceEqual("inactivecaptiontext".AsSpan()))
                        {
                            systemColor = SystemColors.InactiveCaptionText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // infobackground = SystemColors.Info
                case 0x9c8c4bdd:
                    {
                        if (buffer.SequenceEqual("infobackground".AsSpan()))
                        {
                            systemColor = SystemColors.Info;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // infotext = SystemColors.InfoText
                case 0xb164837e:
                    {
                        if (buffer.SequenceEqual("infotext".AsSpan()))
                        {
                            systemColor = SystemColors.InfoText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // menu = SystemColors.Menu
                case 0x99e4dd3a:
                    {
                        if (buffer.SequenceEqual("menu".AsSpan()))
                        {
                            systemColor = SystemColors.Menu;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // menutext = SystemColors.MenuText
                case 0x4c924831:
                    {
                        if (buffer.SequenceEqual("menutext".AsSpan()))
                        {
                            systemColor = SystemColors.MenuText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // scrollbar = SystemColors.ScrollBar
                case 0xd5b6c079:
                    {
                        if (buffer.SequenceEqual("scrollbar".AsSpan()))
                        {
                            systemColor = SystemColors.ScrollBar;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // threeddarkshadow = SystemColors.ControlDarkDark
                case 0xffa62901:
                    {
                        if (buffer.SequenceEqual("threeddarkshadow".AsSpan()))
                        {
                            systemColor = SystemColors.ControlDarkDark;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // threedface = SystemColors.Control
                case 0x77fd6efc:
                    {
                        if (buffer.SequenceEqual("threedface".AsSpan()))
                        {
                            systemColor = SystemColors.Control;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // threedhighlight = SystemColors.ControlLight
                case 0xd4724bc7:
                    {
                        if (buffer.SequenceEqual("threedhighlight".AsSpan()))
                        {
                            systemColor = SystemColors.ControlLight;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // threedlightshadow = SystemColors.ControlLightLight
                case 0x238bb757:
                    {
                        if (buffer.SequenceEqual("threedlightshadow".AsSpan()))
                        {
                            systemColor = SystemColors.ControlLightLight;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // window = SystemColors.Window
                case 0xa172b7dd:
                    {
                        if (buffer.SequenceEqual("window".AsSpan()))
                        {
                            systemColor = SystemColors.Window;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // windowframe = SystemColors.WindowFrame
                case 0x6f554c7e:
                    {
                        if (buffer.SequenceEqual("windowframe".AsSpan()))
                        {
                            systemColor = SystemColors.WindowFrame;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
                // windowtext = SystemColors.WindowText
                case 0xe477b746:
                    {
                        if (buffer.SequenceEqual("windowtext".AsSpan()))
                        {
                            systemColor = SystemColors.WindowText;
                            return true;
                        }
                        systemColor = default;
                        return false;
                    }
            }
            systemColor = default;
            return false;
        }

        public static bool TryToGetNamedColor(ref ReadOnlySpan<char> colour, out Color namedColor)
        {
            Span<char> buffer = stackalloc char[colour.Length];
            ToLowerAscii(colour, ref buffer);
            var stringHash = ComputeStringHash(buffer);
            switch(stringHash)
            {
                // aliceblue = Color.AliceBlue
                case 0x262562c3:
                    {
                        if (buffer.SequenceEqual("aliceblue".AsSpan()))
                        {
                            namedColor = Color.AliceBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // antiquewhite = Color.AntiqueWhite
                case 0xcd4bddcf:
                    {
                        if (buffer.SequenceEqual("antiquewhite".AsSpan()))
                        {
                            namedColor = Color.AntiqueWhite;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // aqua = Color.Aqua
                case 0x42374f95:
                    {
                        if (buffer.SequenceEqual("aqua".AsSpan()))
                        {
                            namedColor = Color.Aqua;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // aquamarine = Color.Aquamarine
                case 0xf3aa9557:
                    {
                        if (buffer.SequenceEqual("aquamarine".AsSpan()))
                        {
                            namedColor = Color.Aquamarine;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // azure = Color.Azure
                case 0x3758c2e0:
                    {
                        if (buffer.SequenceEqual("azure".AsSpan()))
                        {
                            namedColor = Color.Azure;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // beige = Color.Beige
                case 0x38540573:
                    {
                        if (buffer.SequenceEqual("beige".AsSpan()))
                        {
                            namedColor = Color.Beige;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // bisque = Color.Bisque
                case 0xc17f9b3a:
                    {
                        if (buffer.SequenceEqual("bisque".AsSpan()))
                        {
                            namedColor = Color.Bisque;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // black = Color.Black
                case 0x568f4ba4:
                    {
                        if (buffer.SequenceEqual("black".AsSpan()))
                        {
                            namedColor = Color.Black;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // blanchedalmond = Color.BlanchedAlmond
                case 0x16ee77b1:
                    {
                        if (buffer.SequenceEqual("blanchedalmond".AsSpan()))
                        {
                            namedColor = Color.BlanchedAlmond;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // blue = Color.Blue
                case 0x82fbf5cd:
                    {
                        if (buffer.SequenceEqual("blue".AsSpan()))
                        {
                            namedColor = Color.Blue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // blueviolet = Color.BlueViolet
                case 0x419e5a8a:
                    {
                        if (buffer.SequenceEqual("blueviolet".AsSpan()))
                        {
                            namedColor = Color.BlueViolet;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // brown = Color.Brown
                case 0x30be372f:
                    {
                        if (buffer.SequenceEqual("brown".AsSpan()))
                        {
                            namedColor = Color.Brown;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // burlywood = Color.BurlyWood
                case 0x952635e8:
                    {
                        if (buffer.SequenceEqual("burlywood".AsSpan()))
                        {
                            namedColor = Color.BurlyWood;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // cadetblue = Color.CadetBlue
                case 0x480fb75e:
                    {
                        if (buffer.SequenceEqual("cadetblue".AsSpan()))
                        {
                            namedColor = Color.CadetBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // chartreuse = Color.Chartreuse
                case 0x66bc38dd:
                    {
                        if (buffer.SequenceEqual("chartreuse".AsSpan()))
                        {
                            namedColor = Color.Chartreuse;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // chocolate = Color.Chocolate
                case 0x429bb099:
                    {
                        if (buffer.SequenceEqual("chocolate".AsSpan()))
                        {
                            namedColor = Color.Chocolate;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // coral = Color.Coral
                case 0x6d9b9752:
                    {
                        if (buffer.SequenceEqual("coral".AsSpan()))
                        {
                            namedColor = Color.Coral;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // cornflowerblue = Color.CornflowerBlue
                case 0x559a4808:
                    {
                        if (buffer.SequenceEqual("cornflowerblue".AsSpan()))
                        {
                            namedColor = Color.CornflowerBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // cornsilk = Color.Cornsilk
                case 0x1ed7a4ea:
                    {
                        if (buffer.SequenceEqual("cornsilk".AsSpan()))
                        {
                            namedColor = Color.Cornsilk;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // crimson = Color.Crimson
                case 0x042602ee:
                    {
                        if (buffer.SequenceEqual("crimson".AsSpan()))
                        {
                            namedColor = Color.Crimson;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // cyan = Color.Cyan
                case 0x4961533a:
                    {
                        if (buffer.SequenceEqual("cyan".AsSpan()))
                        {
                            namedColor = Color.Cyan;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkblue = Color.DarkBlue
                case 0x0817f94d:
                    {
                        if (buffer.SequenceEqual("darkblue".AsSpan()))
                        {
                            namedColor = Color.DarkBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkcyan = Color.DarkCyan
                case 0xce7d56ba:
                    {
                        if (buffer.SequenceEqual("darkcyan".AsSpan()))
                        {
                            namedColor = Color.DarkCyan;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkgoldenrod = Color.DarkGoldenrod
                case 0x1a236609:
                    {
                        if (buffer.SequenceEqual("darkgoldenrod".AsSpan()))
                        {
                            namedColor = Color.DarkGoldenrod;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkgray = Color.DarkGray
                case 0x3fb6b71a:
                    {
                        if (buffer.SequenceEqual("darkgray".AsSpan()))
                        {
                            namedColor = Color.DarkGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkgreen = Color.DarkGreen
                case 0x0c376f3c:
                    {
                        if (buffer.SequenceEqual("darkgreen".AsSpan()))
                        {
                            namedColor = Color.DarkGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkkhaki = Color.DarkKhaki
                case 0x7cacbc39:
                    {
                        if (buffer.SequenceEqual("darkkhaki".AsSpan()))
                        {
                            namedColor = Color.DarkKhaki;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkmagenta = Color.DarkMagenta
                case 0x1e8db068:
                    {
                        if (buffer.SequenceEqual("darkmagenta".AsSpan()))
                        {
                            namedColor = Color.DarkMagenta;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkolivegreen = Color.DarkOliveGreen
                case 0x9049cd77:
                    {
                        if (buffer.SequenceEqual("darkolivegreen".AsSpan()))
                        {
                            namedColor = Color.DarkOliveGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkorange = Color.DarkOrange
                case 0x3edce36b:
                    {
                        if (buffer.SequenceEqual("darkorange".AsSpan()))
                        {
                            namedColor = Color.DarkOrange;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkorchid = Color.DarkOrchid
                case 0xa60f29ba:
                    {
                        if (buffer.SequenceEqual("darkorchid".AsSpan()))
                        {
                            namedColor = Color.DarkOrchid;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkred = Color.DarkRed
                case 0xebd89f5c:
                    {
                        if (buffer.SequenceEqual("darkred".AsSpan()))
                        {
                            namedColor = Color.DarkRed;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darksalmon = Color.DarkSalmon
                case 0x32db86df:
                    {
                        if (buffer.SequenceEqual("darksalmon".AsSpan()))
                        {
                            namedColor = Color.DarkSalmon;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkseagreen = Color.DarkSeaGreen
                case 0x0340e137:
                    {
                        if (buffer.SequenceEqual("darkseagreen".AsSpan()))
                        {
                            namedColor = Color.DarkSeaGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkslateblue = Color.DarkSlateBlue
                case 0x05b72af6:
                    {
                        if (buffer.SequenceEqual("darkslateblue".AsSpan()))
                        {
                            namedColor = Color.DarkSlateBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkslategray = Color.DarkSlateGray
                case 0x87a7f255:
                    {
                        if (buffer.SequenceEqual("darkslategray".AsSpan()))
                        {
                            namedColor = Color.DarkSlateGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkturquoise = Color.DarkTurquoise
                case 0xe707aada:
                    {
                        if (buffer.SequenceEqual("darkturquoise".AsSpan()))
                        {
                            namedColor = Color.DarkTurquoise;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // darkviolet = Color.DarkViolet
                case 0x7495c772:
                    {
                        if (buffer.SequenceEqual("darkviolet".AsSpan()))
                        {
                            namedColor = Color.DarkViolet;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // deeppink = Color.DeepPink
                case 0x4f9530f7:
                    {
                        if (buffer.SequenceEqual("deeppink".AsSpan()))
                        {
                            namedColor = Color.DeepPink;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // deepskyblue = Color.DeepSkyBlue
                case 0x5cabe370:
                    {
                        if (buffer.SequenceEqual("deepskyblue".AsSpan()))
                        {
                            namedColor = Color.DeepSkyBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // dimgray = Color.DimGray
                case 0x29f41e26:
                    {
                        if (buffer.SequenceEqual("dimgray".AsSpan()))
                        {
                            namedColor = Color.DimGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // dodgerblue = Color.DodgerBlue
                case 0x51b1437e:
                    {
                        if (buffer.SequenceEqual("dodgerblue".AsSpan()))
                        {
                            namedColor = Color.DodgerBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // firebrick = Color.Firebrick
                case 0x677785b6:
                    {
                        if (buffer.SequenceEqual("firebrick".AsSpan()))
                        {
                            namedColor = Color.Firebrick;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // floralwhite = Color.FloralWhite
                case 0x82e33fb4:
                    {
                        if (buffer.SequenceEqual("floralwhite".AsSpan()))
                        {
                            namedColor = Color.FloralWhite;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // forestgreen = Color.ForestGreen
                case 0xb8ea87d3:
                    {
                        if (buffer.SequenceEqual("forestgreen".AsSpan()))
                        {
                            namedColor = Color.ForestGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // fuchsia = Color.Fuchsia
                case 0x03e1343a:
                    {
                        if (buffer.SequenceEqual("fuchsia".AsSpan()))
                        {
                            namedColor = Color.Fuchsia;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // gainsboro = Color.Gainsboro
                case 0xe15092fb:
                    {
                        if (buffer.SequenceEqual("gainsboro".AsSpan()))
                        {
                            namedColor = Color.Gainsboro;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // ghostwhite = Color.GhostWhite
                case 0xd57e7bfd:
                    {
                        if (buffer.SequenceEqual("ghostwhite".AsSpan()))
                        {
                            namedColor = Color.GhostWhite;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // gold = Color.Gold
                case 0xec66d793:
                    {
                        if (buffer.SequenceEqual("gold".AsSpan()))
                        {
                            namedColor = Color.Gold;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // goldenrod = Color.Goldenrod
                case 0xa8543b89:
                    {
                        if (buffer.SequenceEqual("goldenrod".AsSpan()))
                        {
                            namedColor = Color.Goldenrod;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // gray = Color.Gray
                case 0xba9ab39a:
                    {
                        if (buffer.SequenceEqual("gray".AsSpan()))
                        {
                            namedColor = Color.Gray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // green = Color.Green
                case 0x011decbc:
                    {
                        if (buffer.SequenceEqual("green".AsSpan()))
                        {
                            namedColor = Color.Green;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // greenyellow = Color.GreenYellow
                case 0x0d8329fc:
                    {
                        if (buffer.SequenceEqual("greenyellow".AsSpan()))
                        {
                            namedColor = Color.GreenYellow;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // honeydew = Color.Honeydew
                case 0x60605b18:
                    {
                        if (buffer.SequenceEqual("honeydew".AsSpan()))
                        {
                            namedColor = Color.Honeydew;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // hotpink = Color.HotPink
                case 0x17c1a698:
                    {
                        if (buffer.SequenceEqual("hotpink".AsSpan()))
                        {
                            namedColor = Color.HotPink;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // indianred = Color.IndianRed
                case 0x9807620d:
                    {
                        if (buffer.SequenceEqual("indianred".AsSpan()))
                        {
                            namedColor = Color.IndianRed;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // indigo = Color.Indigo
                case 0x4d77512b:
                    {
                        if (buffer.SequenceEqual("indigo".AsSpan()))
                        {
                            namedColor = Color.Indigo;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // ivory = Color.Ivory
                case 0x9dea06b6:
                    {
                        if (buffer.SequenceEqual("ivory".AsSpan()))
                        {
                            namedColor = Color.Ivory;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // khaki = Color.Khaki
                case 0x719339b9:
                    {
                        if (buffer.SequenceEqual("khaki".AsSpan()))
                        {
                            namedColor = Color.Khaki;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lavender = Color.Lavender
                case 0xb928b3ee:
                    {
                        if (buffer.SequenceEqual("lavender".AsSpan()))
                        {
                            namedColor = Color.Lavender;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lavenderblush = Color.LavenderBlush
                case 0xd591a34c:
                    {
                        if (buffer.SequenceEqual("lavenderblush".AsSpan()))
                        {
                            namedColor = Color.LavenderBlush;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lawngreen = Color.LawnGreen
                case 0x8bb2ab96:
                    {
                        if (buffer.SequenceEqual("lawngreen".AsSpan()))
                        {
                            namedColor = Color.LawnGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lemonchiffon = Color.LemonChiffon
                case 0xa9ddfd93:
                    {
                        if (buffer.SequenceEqual("lemonchiffon".AsSpan()))
                        {
                            namedColor = Color.LemonChiffon;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightblue = Color.LightBlue
                case 0xc370be3b:
                    {
                        if (buffer.SequenceEqual("lightblue".AsSpan()))
                        {
                            namedColor = Color.LightBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightcoral = Color.LightCoral
                case 0x4efa960c:
                    {
                        if (buffer.SequenceEqual("lightcoral".AsSpan()))
                        {
                            namedColor = Color.LightCoral;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightcyan = Color.LightCyan
                case 0xf060b994:
                    {
                        if (buffer.SequenceEqual("lightcyan".AsSpan()))
                        {
                            namedColor = Color.LightCyan;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightgoldenrodyellow = Color.LightGoldenrodYellow
                case 0xb2f7293f:
                    {
                        if (buffer.SequenceEqual("lightgoldenrodyellow".AsSpan()))
                        {
                            namedColor = Color.LightGoldenrodYellow;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightgray = Color.LightGray
                case 0x2bbe58fc:
                    {
                        if (buffer.SequenceEqual("lightgray".AsSpan()))
                        {
                            namedColor = Color.LightGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightgreen = Color.LightGreen
                case 0x9c3f19c6:
                    {
                        if (buffer.SequenceEqual("lightgreen".AsSpan()))
                        {
                            namedColor = Color.LightGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightpink = Color.LightPink
                case 0x699ce1b7:
                    {
                        if (buffer.SequenceEqual("lightpink".AsSpan()))
                        {
                            namedColor = Color.LightPink;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightsalmon = Color.LightSalmon
                case 0x714b1745:
                    {
                        if (buffer.SequenceEqual("lightsalmon".AsSpan()))
                        {
                            namedColor = Color.LightSalmon;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightseagreen = Color.LightSeaGreen
                case 0xe243e671:
                    {
                        if (buffer.SequenceEqual("lightseagreen".AsSpan()))
                        {
                            namedColor = Color.LightSeaGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightskyblue = Color.LightSkyBlue
                case 0x09357b30:
                    {
                        if (buffer.SequenceEqual("lightskyblue".AsSpan()))
                        {
                            namedColor = Color.LightSkyBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightslategray = Color.LightSlateGray
                case 0x43bdda67:
                    {
                        if (buffer.SequenceEqual("lightslategray".AsSpan()))
                        {
                            namedColor = Color.LightSlateGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightsteelblue = Color.LightSteelBlue
                case 0x8989e5fe:
                    {
                        if (buffer.SequenceEqual("lightsteelblue".AsSpan()))
                        {
                            namedColor = Color.LightSteelBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lightyellow = Color.LightYellow
                case 0xa124f36b:
                    {
                        if (buffer.SequenceEqual("lightyellow".AsSpan()))
                        {
                            namedColor = Color.LightYellow;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // lime = Color.Lime
                case 0x07e34bbc:
                    {
                        if (buffer.SequenceEqual("lime".AsSpan()))
                        {
                            namedColor = Color.Lime;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // limegreen = Color.LimeGreen
                case 0x5f247ea7:
                    {
                        if (buffer.SequenceEqual("limegreen".AsSpan()))
                        {
                            namedColor = Color.LimeGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // linen = Color.Linen
                case 0xd6e414eb:
                    {
                        if (buffer.SequenceEqual("linen".AsSpan()))
                        {
                            namedColor = Color.Linen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // magenta = Color.Magenta
                case 0x63e629e8:
                    {
                        if (buffer.SequenceEqual("magenta".AsSpan()))
                        {
                            namedColor = Color.Magenta;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // maroon = Color.Maroon
                case 0x6e32ccf5:
                    {
                        if (buffer.SequenceEqual("maroon".AsSpan()))
                        {
                            namedColor = Color.Maroon;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumaquamarine = Color.MediumAquamarine
                case 0x22d8ff1c:
                    {
                        if (buffer.SequenceEqual("mediumaquamarine".AsSpan()))
                        {
                            namedColor = Color.MediumAquamarine;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumblue = Color.MediumBlue
                case 0xfdec2a8e:
                    {
                        if (buffer.SequenceEqual("mediumblue".AsSpan()))
                        {
                            namedColor = Color.MediumBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumorchid = Color.MediumOrchid
                case 0x1a0e44ad:
                    {
                        if (buffer.SequenceEqual("mediumorchid".AsSpan()))
                        {
                            namedColor = Color.MediumOrchid;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumpurple = Color.MediumPurple
                case 0xa9362f24:
                    {
                        if (buffer.SequenceEqual("mediumpurple".AsSpan()))
                        {
                            namedColor = Color.MediumPurple;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumseagreen = Color.MediumSeaGreen
                case 0x0ba64a14:
                    {
                        if (buffer.SequenceEqual("mediumseagreen".AsSpan()))
                        {
                            namedColor = Color.MediumSeaGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumslateblue = Color.MediumSlateBlue
                case 0x772890e7:
                    {
                        if (buffer.SequenceEqual("mediumslateblue".AsSpan()))
                        {
                            namedColor = Color.MediumSlateBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumspringgreen = Color.MediumSpringGreen
                case 0x71780822:
                    {
                        if (buffer.SequenceEqual("mediumspringgreen".AsSpan()))
                        {
                            namedColor = Color.MediumSpringGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumturquoise = Color.MediumTurquoise
                case 0xcbdc3f67:
                    {
                        if (buffer.SequenceEqual("mediumturquoise".AsSpan()))
                        {
                            namedColor = Color.MediumTurquoise;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mediumvioletred = Color.MediumVioletRed
                case 0x1bdaa4b0:
                    {
                        if (buffer.SequenceEqual("mediumvioletred".AsSpan()))
                        {
                            namedColor = Color.MediumVioletRed;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // midnightblue = Color.MidnightBlue
                case 0xcc8e0751:
                    {
                        if (buffer.SequenceEqual("midnightblue".AsSpan()))
                        {
                            namedColor = Color.MidnightBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mintcream = Color.MintCream
                case 0x0920e031:
                    {
                        if (buffer.SequenceEqual("mintcream".AsSpan()))
                        {
                            namedColor = Color.MintCream;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // mistyrose = Color.MistyRose
                case 0xe1858adc:
                    {
                        if (buffer.SequenceEqual("mistyrose".AsSpan()))
                        {
                            namedColor = Color.MistyRose;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // moccasin = Color.Moccasin
                case 0x45658504:
                    {
                        if (buffer.SequenceEqual("moccasin".AsSpan()))
                        {
                            namedColor = Color.Moccasin;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // navajowhite = Color.NavajoWhite
                case 0x4c7a1b8f:
                    {
                        if (buffer.SequenceEqual("navajowhite".AsSpan()))
                        {
                            namedColor = Color.NavajoWhite;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // navy = Color.Navy
                case 0xa0fff1d1:
                    {
                        if (buffer.SequenceEqual("navy".AsSpan()))
                        {
                            namedColor = Color.Navy;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // oldlace = Color.OldLace
                case 0x9b55348d:
                    {
                        if (buffer.SequenceEqual("oldlace".AsSpan()))
                        {
                            namedColor = Color.OldLace;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // olive = Color.Olive
                case 0x835cd3cc:
                    {
                        if (buffer.SequenceEqual("olive".AsSpan()))
                        {
                            namedColor = Color.Olive;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // olivedrab = Color.OliveDrab
                case 0xb351bdbd:
                    {
                        if (buffer.SequenceEqual("olivedrab".AsSpan()))
                        {
                            namedColor = Color.OliveDrab;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // orange = Color.Orange
                case 0x45b473eb:
                    {
                        if (buffer.SequenceEqual("orange".AsSpan()))
                        {
                            namedColor = Color.Orange;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // orangered = Color.OrangeRed
                case 0x97e0985a:
                    {
                        if (buffer.SequenceEqual("orangered".AsSpan()))
                        {
                            namedColor = Color.OrangeRed;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // orchid = Color.Orchid
                case 0xace6ba3a:
                    {
                        if (buffer.SequenceEqual("orchid".AsSpan()))
                        {
                            namedColor = Color.Orchid;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // palegoldenrod = Color.PaleGoldenrod
                case 0x6021b659:
                    {
                        if (buffer.SequenceEqual("palegoldenrod".AsSpan()))
                        {
                            namedColor = Color.PaleGoldenrod;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // palegreen = Color.PaleGreen
                case 0x8e1fedcc:
                    {
                        if (buffer.SequenceEqual("palegreen".AsSpan()))
                        {
                            namedColor = Color.PaleGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // paleturquoise = Color.PaleTurquoise
                case 0xcc91efca:
                    {
                        if (buffer.SequenceEqual("paleturquoise".AsSpan()))
                        {
                            namedColor = Color.PaleTurquoise;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // palevioletred = Color.PaleVioletRed
                case 0x907d3a11:
                    {
                        if (buffer.SequenceEqual("palevioletred".AsSpan()))
                        {
                            namedColor = Color.PaleVioletRed;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // papayawhip = Color.PapayaWhip
                case 0x8c361729:
                    {
                        if (buffer.SequenceEqual("papayawhip".AsSpan()))
                        {
                            namedColor = Color.PapayaWhip;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // peachpuff = Color.PeachPuff
                case 0xe92d961d:
                    {
                        if (buffer.SequenceEqual("peachpuff".AsSpan()))
                        {
                            namedColor = Color.PeachPuff;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // peru = Color.Peru
                case 0xb8f04003:
                    {
                        if (buffer.SequenceEqual("peru".AsSpan()))
                        {
                            namedColor = Color.Peru;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // pink = Color.Pink
                case 0x225e036d:
                    {
                        if (buffer.SequenceEqual("pink".AsSpan()))
                        {
                            namedColor = Color.Pink;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // plum = Color.Plum
                case 0xb6adb06b:
                    {
                        if (buffer.SequenceEqual("plum".AsSpan()))
                        {
                            namedColor = Color.Plum;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // powderblue = Color.PowderBlue
                case 0x6fdd84bc:
                    {
                        if (buffer.SequenceEqual("powderblue".AsSpan()))
                        {
                            namedColor = Color.PowderBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // purple = Color.Purple
                case 0x9a6e02ff:
                    {
                        if (buffer.SequenceEqual("purple".AsSpan()))
                        {
                            namedColor = Color.Purple;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // red = Color.Red
                case 0x40f480dc:
                    {
                        if (buffer.SequenceEqual("red".AsSpan()))
                        {
                            namedColor = Color.Red;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // rosybrown = Color.RosyBrown
                case 0xf8d1f5a6:
                    {
                        if (buffer.SequenceEqual("rosybrown".AsSpan()))
                        {
                            namedColor = Color.RosyBrown;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // royalblue = Color.RoyalBlue
                case 0xbfbdf46c:
                    {
                        if (buffer.SequenceEqual("royalblue".AsSpan()))
                        {
                            namedColor = Color.RoyalBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // saddlebrown = Color.SaddleBrown
                case 0xe800849a:
                    {
                        if (buffer.SequenceEqual("saddlebrown".AsSpan()))
                        {
                            namedColor = Color.SaddleBrown;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // salmon = Color.Salmon
                case 0x39b3175f:
                    {
                        if (buffer.SequenceEqual("salmon".AsSpan()))
                        {
                            namedColor = Color.Salmon;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // sandybrown = Color.SandyBrown
                case 0x7b75476a:
                    {
                        if (buffer.SequenceEqual("sandybrown".AsSpan()))
                        {
                            namedColor = Color.SandyBrown;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // seagreen = Color.SeaGreen
                case 0xad8825b7:
                    {
                        if (buffer.SequenceEqual("seagreen".AsSpan()))
                        {
                            namedColor = Color.SeaGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // seashell = Color.SeaShell
                case 0x46aadbde:
                    {
                        if (buffer.SequenceEqual("seashell".AsSpan()))
                        {
                            namedColor = Color.SeaShell;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // sienna = Color.Sienna
                case 0x3227246b:
                    {
                        if (buffer.SequenceEqual("sienna".AsSpan()))
                        {
                            namedColor = Color.Sienna;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // silver = Color.Silver
                case 0xb554f920:
                    {
                        if (buffer.SequenceEqual("silver".AsSpan()))
                        {
                            namedColor = Color.Silver;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // skyblue = Color.SkyBlue
                case 0x1c96ce4e:
                    {
                        if (buffer.SequenceEqual("skyblue".AsSpan()))
                        {
                            namedColor = Color.SkyBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // slateblue = Color.SlateBlue
                case 0x93e80076:
                    {
                        if (buffer.SequenceEqual("slateblue".AsSpan()))
                        {
                            namedColor = Color.SlateBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // slategray = Color.SlateGray
                case 0x15d8c7d5:
                    {
                        if (buffer.SequenceEqual("slategray".AsSpan()))
                        {
                            namedColor = Color.SlateGray;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // snow = Color.Snow
                case 0x848317f2:
                    {
                        if (buffer.SequenceEqual("snow".AsSpan()))
                        {
                            namedColor = Color.Snow;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // springgreen = Color.SpringGreen
                case 0xf79b7417:
                    {
                        if (buffer.SequenceEqual("springgreen".AsSpan()))
                        {
                            namedColor = Color.SpringGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // steelblue = Color.SteelBlue
                case 0x54408ac8:
                    {
                        if (buffer.SequenceEqual("steelblue".AsSpan()))
                        {
                            namedColor = Color.SteelBlue;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // tan = Color.Tan
                case 0x9cf73498:
                    {
                        if (buffer.SequenceEqual("tan".AsSpan()))
                        {
                            namedColor = Color.Tan;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // teal = Color.Teal
                case 0xa3fd7e9f:
                    {
                        if (buffer.SequenceEqual("teal".AsSpan()))
                        {
                            namedColor = Color.Teal;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // thistle = Color.Thistle
                case 0x08a71a94:
                    {
                        if (buffer.SequenceEqual("thistle".AsSpan()))
                        {
                            namedColor = Color.Thistle;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // tomato = Color.Tomato
                case 0x35a2e5c3:
                    {
                        if (buffer.SequenceEqual("tomato".AsSpan()))
                        {
                            namedColor = Color.Tomato;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // transparent = Color.Transparent
                case 0xafe34731:
                    {
                        if (buffer.SequenceEqual("transparent".AsSpan()))
                        {
                            namedColor = Color.Transparent;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // turquoise = Color.Turquoise
                case 0x7538805a:
                    {
                        if (buffer.SequenceEqual("turquoise".AsSpan()))
                        {
                            namedColor = Color.Turquoise;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // violet = Color.Violet
                case 0x7b6d57f2:
                    {
                        if (buffer.SequenceEqual("violet".AsSpan()))
                        {
                            namedColor = Color.Violet;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // wheat = Color.Wheat
                case 0x043cb490:
                    {
                        if (buffer.SequenceEqual("wheat".AsSpan()))
                        {
                            namedColor = Color.Wheat;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // white = Color.White
                case 0xde020766:
                    {
                        if (buffer.SequenceEqual("white".AsSpan()))
                        {
                            namedColor = Color.White;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // whitesmoke = Color.WhiteSmoke
                case 0xaa3e3a1f:
                    {
                        if (buffer.SequenceEqual("whitesmoke".AsSpan()))
                        {
                            namedColor = Color.WhiteSmoke;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // yellow = Color.Yellow
                case 0x05bf6449:
                    {
                        if (buffer.SequenceEqual("yellow".AsSpan()))
                        {
                            namedColor = Color.Yellow;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
                // yellowgreen = Color.YellowGreen
                case 0xb6b77908:
                    {
                        if (buffer.SequenceEqual("yellowgreen".AsSpan()))
                        {
                            namedColor = Color.YellowGreen;
                            return true;
                        }
                        namedColor = default;
                        return false;
                    }
            }
            namedColor = default;
            return false;
        }

        public static readonly Dictionary<uint, Func<Color>> ArgbToNamedColorDictionary = new Dictionary<uint, Func<Color>>()
        {
            [0x00FFFFFF] = () => Color.Transparent,
            [0xFFF0F8FF] = () => Color.AliceBlue,
            [0xFFFAEBD7] = () => Color.AntiqueWhite,
            [0xFF00FFFF] = () => Color.Aqua,
            [0xFF7FFFD4] = () => Color.Aquamarine,
            [0xFFF0FFFF] = () => Color.Azure,
            [0xFFF5F5DC] = () => Color.Beige,
            [0xFFFFE4C4] = () => Color.Bisque,
            [0xFF000000] = () => Color.Black,
            [0xFFFFEBCD] = () => Color.BlanchedAlmond,
            [0xFF0000FF] = () => Color.Blue,
            [0xFF8A2BE2] = () => Color.BlueViolet,
            [0xFFA52A2A] = () => Color.Brown,
            [0xFFDEB887] = () => Color.BurlyWood,
            [0xFF5F9EA0] = () => Color.CadetBlue,
            [0xFF7FFF00] = () => Color.Chartreuse,
            [0xFFD2691E] = () => Color.Chocolate,
            [0xFFFF7F50] = () => Color.Coral,
            [0xFF6495ED] = () => Color.CornflowerBlue,
            [0xFFFFF8DC] = () => Color.Cornsilk,
            [0xFFDC143C] = () => Color.Crimson,
            [0xFF00FFFF] = () => Color.Cyan,
            [0xFF00008B] = () => Color.DarkBlue,
            [0xFF008B8B] = () => Color.DarkCyan,
            [0xFFB8860B] = () => Color.DarkGoldenrod,
            [0xFFA9A9A9] = () => Color.DarkGray,
            [0xFF006400] = () => Color.DarkGreen,
            [0xFFBDB76B] = () => Color.DarkKhaki,
            [0xFF8B008B] = () => Color.DarkMagenta,
            [0xFF556B2F] = () => Color.DarkOliveGreen,
            [0xFFFF8C00] = () => Color.DarkOrange,
            [0xFF9932CC] = () => Color.DarkOrchid,
            [0xFF8B0000] = () => Color.DarkRed,
            [0xFFE9967A] = () => Color.DarkSalmon,
            [0xFF8FBC8B] = () => Color.DarkSeaGreen,
            [0xFF483D8B] = () => Color.DarkSlateBlue,
            [0xFF2F4F4F] = () => Color.DarkSlateGray,
            [0xFF00CED1] = () => Color.DarkTurquoise,
            [0xFF9400D3] = () => Color.DarkViolet,
            [0xFFFF1493] = () => Color.DeepPink,
            [0xFF00BFFF] = () => Color.DeepSkyBlue,
            [0xFF696969] = () => Color.DimGray,
            [0xFF1E90FF] = () => Color.DodgerBlue,
            [0xFFB22222] = () => Color.Firebrick,
            [0xFFFFFAF0] = () => Color.FloralWhite,
            [0xFF228B22] = () => Color.ForestGreen,
            [0xFFFF00FF] = () => Color.Fuchsia,
            [0xFFDCDCDC] = () => Color.Gainsboro,
            [0xFFF8F8FF] = () => Color.GhostWhite,
            [0xFFFFD700] = () => Color.Gold,
            [0xFFDAA520] = () => Color.Goldenrod,
            [0xFF808080] = () => Color.Gray,
            [0xFF008000] = () => Color.Green,
            [0xFFADFF2F] = () => Color.GreenYellow,
            [0xFFF0FFF0] = () => Color.Honeydew,
            [0xFFFF69B4] = () => Color.HotPink,
            [0xFFCD5C5C] = () => Color.IndianRed,
            [0xFF4B0082] = () => Color.Indigo,
            [0xFFFFFFF0] = () => Color.Ivory,
            [0xFFF0E68C] = () => Color.Khaki,
            [0xFFE6E6FA] = () => Color.Lavender,
            [0xFFFFF0F5] = () => Color.LavenderBlush,
            [0xFF7CFC00] = () => Color.LawnGreen,
            [0xFFFFFACD] = () => Color.LemonChiffon,
            [0xFFADD8E6] = () => Color.LightBlue,
            [0xFFF08080] = () => Color.LightCoral,
            [0xFFE0FFFF] = () => Color.LightCyan,
            [0xFFFAFAD2] = () => Color.LightGoldenrodYellow,
            [0xFF90EE90] = () => Color.LightGreen,
            [0xFFD3D3D3] = () => Color.LightGray,
            [0xFFFFB6C1] = () => Color.LightPink,
            [0xFFFFA07A] = () => Color.LightSalmon,
            [0xFF20B2AA] = () => Color.LightSeaGreen,
            [0xFF87CEFA] = () => Color.LightSkyBlue,
            [0xFF778899] = () => Color.LightSlateGray,
            [0xFFB0C4DE] = () => Color.LightSteelBlue,
            [0xFFFFFFE0] = () => Color.LightYellow,
            [0xFF00FF00] = () => Color.Lime,
            [0xFF32CD32] = () => Color.LimeGreen,
            [0xFFFAF0E6] = () => Color.Linen,
            [0xFFFF00FF] = () => Color.Magenta,
            [0xFF800000] = () => Color.Maroon,
            [0xFF66CDAA] = () => Color.MediumAquamarine,
            [0xFF0000CD] = () => Color.MediumBlue,
            [0xFFBA55D3] = () => Color.MediumOrchid,
            [0xFF9370DB] = () => Color.MediumPurple,
            [0xFF3CB371] = () => Color.MediumSeaGreen,
            [0xFF7B68EE] = () => Color.MediumSlateBlue,
            [0xFF00FA9A] = () => Color.MediumSpringGreen,
            [0xFF48D1CC] = () => Color.MediumTurquoise,
            [0xFFC71585] = () => Color.MediumVioletRed,
            [0xFF191970] = () => Color.MidnightBlue,
            [0xFFF5FFFA] = () => Color.MintCream,
            [0xFFFFE4E1] = () => Color.MistyRose,
            [0xFFFFE4B5] = () => Color.Moccasin,
            [0xFFFFDEAD] = () => Color.NavajoWhite,
            [0xFF000080] = () => Color.Navy,
            [0xFFFDF5E6] = () => Color.OldLace,
            [0xFF808000] = () => Color.Olive,
            [0xFF6B8E23] = () => Color.OliveDrab,
            [0xFFFFA500] = () => Color.Orange,
            [0xFFFF4500] = () => Color.OrangeRed,
            [0xFFDA70D6] = () => Color.Orchid,
            [0xFFEEE8AA] = () => Color.PaleGoldenrod,
            [0xFF98FB98] = () => Color.PaleGreen,
            [0xFFAFEEEE] = () => Color.PaleTurquoise,
            [0xFFDB7093] = () => Color.PaleVioletRed,
            [0xFFFFEFD5] = () => Color.PapayaWhip,
            [0xFFFFDAB9] = () => Color.PeachPuff,
            [0xFFCD853F] = () => Color.Peru,
            [0xFFFFC0CB] = () => Color.Pink,
            [0xFFDDA0DD] = () => Color.Plum,
            [0xFFB0E0E6] = () => Color.PowderBlue,
            [0xFF800080] = () => Color.Purple,
            [0xFFFF0000] = () => Color.Red,
            [0xFFBC8F8F] = () => Color.RosyBrown,
            [0xFF4169E1] = () => Color.RoyalBlue,
            [0xFF8B4513] = () => Color.SaddleBrown,
            [0xFFFA8072] = () => Color.Salmon,
            [0xFFF4A460] = () => Color.SandyBrown,
            [0xFF2E8B57] = () => Color.SeaGreen,
            [0xFFFFF5EE] = () => Color.SeaShell,
            [0xFFA0522D] = () => Color.Sienna,
            [0xFFC0C0C0] = () => Color.Silver,
            [0xFF87CEEB] = () => Color.SkyBlue,
            [0xFF6A5ACD] = () => Color.SlateBlue,
            [0xFF708090] = () => Color.SlateGray,
            [0xFFFFFAFA] = () => Color.Snow,
            [0xFF00FF7F] = () => Color.SpringGreen,
            [0xFF4682B4] = () => Color.SteelBlue,
            [0xFFD2B48C] = () => Color.Tan,
            [0xFF008080] = () => Color.Teal,
            [0xFFD8BFD8] = () => Color.Thistle,
            [0xFFFF6347] = () => Color.Tomato,
            [0xFF40E0D0] = () => Color.Turquoise,
            [0xFFEE82EE] = () => Color.Violet,
            [0xFFF5DEB3] = () => Color.Wheat,
            [0xFFFFFFFF] = () => Color.White,
            [0xFFF5F5F5] = () => Color.WhiteSmoke,
            [0xFFFFFF00] = () => Color.Yellow,
            [0xFF9ACD32] = () => Color.YellowGreen,
        };
    }

    /// <summary>
    /// Converts string representations of colours into <see cref="Color"/> objects.
    /// </summary>
    public class SvgColourConverter : ColorConverter
    {
        private static readonly char[] SplitChars = new char[] { ',' , ' ' };

        private static int ClampInt(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static float ClampFloat(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static double ClampDouble(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static double ClampHue(double h)
        {
            if (h > 360.0)
            {
                h = h % 360.0;
            }

            if (h < 0)
            {
                h = 360.0 - (-h) % 360.0;
            }

            return h;
        }

        public static uint ToArgb(int alpha, int red, int green, int blue)
        {
            return (uint) alpha << 24 | (uint) red << 16 | (uint) green << 8 | (uint) blue << 0;
        }

        public static Color Parse(ReadOnlySpan<char> colour)
        {
            var span = colour.Trim();

            // RGB support
            if (span.IndexOf("rgb".AsSpan()) == 0)
            {
                try
                {
                    // get the values from the RGB string
                    var start = span.IndexOf('(') + 1;
                    var length = span.IndexOf(')') - start;
                    var parts = new StringSplitEnumerator(span.Slice(start, length), SplitChars.AsSpan());
                    var count = 0;
                    int alpha = 255;
                    int red = default;
                    int green = default;
                    int blue = default;
                    bool isDecimal = false;

                    foreach (var part in parts)
                    {
                        var partValue = part.Value;
                        if (count == 0)
                        {
                            if (partValue.IndexOf('%') == partValue.Length - 1)
                            {
                                partValue = partValue.TrimEnd('%');
                                var redDecimal = StringParser.ToFloatAny(ref partValue);
                                redDecimal = ClampFloat(redDecimal, 0f, 100f);
                                red = (int) Math.Round(255 * redDecimal / 100f);
                                isDecimal = true;
                            }
                            else
                            {
                                red = StringParser.ToInt(ref partValue);
                                red = ClampInt(red, 0, 255);
                                isDecimal = false;
                            }
                        }
                        else if (count == 1)
                        {
                            if (partValue.IndexOf('%') == partValue.Length - 1)
                            {
                                if (!isDecimal)
                                {
                                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                                }

                                partValue = partValue.TrimEnd('%');
                                var greenDecimal = StringParser.ToFloatAny(ref partValue);
                                greenDecimal = ClampFloat(greenDecimal, 0f, 100f);
                                green = (int) Math.Round(255 * greenDecimal / 100f);
                            }
                            else
                            {
                                if (isDecimal)
                                {
                                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                                }

                                green = StringParser.ToInt(ref partValue);
                                green = ClampInt(green, 0, 255);
                            }
                        }
                        else if (count == 2)
                        {
                            if (partValue.IndexOf('%') == partValue.Length - 1)
                            {
                                if (!isDecimal)
                                {
                                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                                }

                                partValue = partValue.TrimEnd('%');
                                var blueDecimal = StringParser.ToFloatAny(ref partValue);
                                blueDecimal = ClampFloat(blueDecimal, 0f, 100f);
                                blue = (int) Math.Round(255 * blueDecimal / 100f);
                            }
                            else
                            {
                                if (isDecimal)
                                {
                                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                                }

                                blue = StringParser.ToInt(ref partValue);
                                blue = ClampInt(blue, 0, 255);
                            }
                        }
                        else if (count == 3)
                        {
                            // determine the alpha value if this is an RGBA (it will be the 4th value if there is one)
                            // the alpha portion of the rgba is not an int 0-255 it is a decimal between 0 and 1
                            // so we have to determine the corresponding byte value
                            if (partValue.IndexOf('.') == 0)
                            {
                                // TODO: partValue = "0" + partValue;
                            }

                            var alphaDecimal = StringParser.ToDouble(ref partValue);
                            if (alphaDecimal <= 1)
                            {
                                alphaDecimal = ClampDouble(alphaDecimal, 0f, 1f);
                                alpha = (int) Math.Round(alphaDecimal * 255);
                            }
                            else
                            {
                                alphaDecimal = ClampDouble(alphaDecimal, 0f, 255f);
                                alpha = (int) Math.Round(alphaDecimal);
                            }
                        }
                        else
                        {
                            throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                        }

                        count++;
                    }

                    var argb = ToArgb(alpha, red, green, blue);
                    if (ColorConverterHelpers.ArgbToNamedColorDictionary.TryGetValue(argb, out var argbNamedColor))
                    {
                        return argbNamedColor();
                    }
                    return Color.FromArgb(alpha, red, green, blue);
                }
                catch
                {
                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                }
            }

            // HSL support
            if (span.IndexOf("hsl".AsSpan()) == 0)
            {
                try
                {
                    // get the values from the HSL string
                    var start = span.IndexOf('(') + 1;
                    var length = span.IndexOf(')') - start;
                    var parts = new StringSplitEnumerator(span.Slice(start, length), SplitChars.AsSpan());
                    var count = 0;
                    var h = default(double);
                    var s = default(double);
                    var l = default(double);

                    // Get the HSL values in a range from 0 to 1.
                    foreach (var part in parts)
                    {
                        var partValue = part.Value;
                        if (count == 0)
                        {
                            var hDecimal = StringParser.ToDouble(ref partValue);
                            hDecimal = ClampHue(hDecimal);
                            h = hDecimal / 360.0;
                        }
                        else if (count == 1)
                        {
                            if (partValue.IndexOf('%') == partValue.Length - 1)
                            {
                                partValue = partValue.TrimEnd('%');

                                var sDecimal = StringParser.ToDouble(ref partValue);
                                sDecimal = ClampDouble(sDecimal, 0f, 100f);
                                s = sDecimal / 100.0;
                            }
                            else
                            {
                                throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                            }
                        }
                        else if (count == 2)
                        {
                            if (partValue.IndexOf('%') == partValue.Length - 1)
                            {
                                partValue = partValue.TrimEnd('%');
                                var lDecimal = StringParser.ToDouble(ref partValue);
                                lDecimal = ClampDouble(lDecimal, 0f, 100f);
                                l = lDecimal / 100.0;
                            }
                            else
                            {
                                throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                            }
                        }
                        else
                        {
                            throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                        }

                        count++;
                    }

                    // Convert the HSL color to an RGB color
                    Hsl2Rgb(h, s, l, out var red, out var green, out var blue);
                    var argb = ToArgb(255, red, green, blue);
                    if (ColorConverterHelpers.ArgbToNamedColorDictionary.TryGetValue(argb, out var argbNamedColor))
                    {
                        return argbNamedColor();
                    }
                    return Color.FromArgb(255, red, green, blue);
                }
                catch
                {
                    throw new SvgException("Colour is in an invalid format: '" + span.ToString() + "'");
                }
            }

            // HEX support
            if (span.IndexOf('#') == 0)
            {
                // The format of an RGB value in hexadecimal notation is a '#' immediately followed by either three or six hexadecimal characters.

                if (span.Length == 4)
                {
                    // The three-digit RGB notation (#rgb) is converted into six-digit form (#rrggbb) by replicating digits, not by adding zeros.
                    // For example, #fb0 expands to #ffbb00.
                    Span<char> redString = stackalloc char[2] {span[1], span[1]};
                    Span<char> greenString = stackalloc char[2] {span[2], span[2]};
                    Span<char> blueString = stackalloc char[2] {span[3], span[3]};
#if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
                    var red = int.Parse(redString, NumberStyles.AllowHexSpecifier);
                    var green = int.Parse(greenString, NumberStyles.AllowHexSpecifier);
                    var blue = int.Parse(blueString, NumberStyles.AllowHexSpecifier);
#else
                    var red = int.Parse(redString.ToString(), NumberStyles.AllowHexSpecifier);
                    var green = int.Parse(greenString.ToString(), NumberStyles.AllowHexSpecifier);
                    var blue = int.Parse(blueString.ToString(), NumberStyles.AllowHexSpecifier);
#endif
                    var argb = ToArgb(255, red, green, blue);
                    if (ColorConverterHelpers.ArgbToNamedColorDictionary.TryGetValue(argb, out var argbNamedColor))
                    {
                        return argbNamedColor();
                    }
                    return Color.FromArgb(255, red, green, blue);
                }

                if (span.Length == 7)
                {
                    var redString = span.Slice(1, 2);
                    var greenString = span.Slice(3, 2);
                    var blueString = span.Slice(5, 2);
#if NETSTANDARD2_1 || NETCORE || NETCOREAPP2_1 || NETCOREAPP3_1 || NET5_0
                    var red = int.Parse(redString, NumberStyles.AllowHexSpecifier);
                    var green = int.Parse(greenString, NumberStyles.AllowHexSpecifier);
                    var blue = int.Parse(blueString, NumberStyles.AllowHexSpecifier);
#else
                    var red = int.Parse(redString.ToString(), NumberStyles.AllowHexSpecifier);
                    var green = int.Parse(greenString.ToString(), NumberStyles.AllowHexSpecifier);
                    var blue = int.Parse(blueString.ToString(), NumberStyles.AllowHexSpecifier);
#endif
                    var argb = ToArgb(255, red, green, blue);
                    if (ColorConverterHelpers.ArgbToNamedColorDictionary.TryGetValue(argb, out var argbNamedColor))
                    {
                        return argbNamedColor();
                    }
                    return Color.FromArgb(255, red, green, blue);
                }

                return Color.Empty;
            }

            // Colors support
            if (ColorConverterHelpers.TryToGetNamedColor(ref span, out var namedColor))
            {
                return namedColor;
            }

            // System Colors support
            if (ColorConverterHelpers.TryToGetSystemColor(ref span, out var systemColor))
            {
                return systemColor;
            }

            // Grey Colors support
            if (ColorConverterHelpers.TryToGetGreyColor(ref span, out var greyColor))
            {
                return greyColor;
            }

            return Color.Empty;
        }

        /// <summary>
        /// Converts the given object to the converter's native type.
        /// </summary>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor"/> that provides a format context. You can use this object to get additional information about the environment from which this converter is being invoked.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/> that specifies the culture to represent the color.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">The conversion cannot be performed.</exception>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string colour)
            {
                return Parse(colour.AsSpan());
            }

            return base.ConvertFrom(context, culture, value);
        }

        public object Parse_OLD(ITypeDescriptorContext context, CultureInfo culture, string colour)
        {
            colour = colour.Trim();

            // RGB support
            if (colour.StartsWith("rgb", StringComparison.InvariantCulture))
            {
                try
                {
                    int start = colour.IndexOf("(", StringComparison.InvariantCulture) + 1;

                    //get the values from the RGB string
                    string[] values = colour.Substring(start, colour.IndexOf(")", StringComparison.InvariantCulture) - start)
                        .Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);

                    //determine the alpha value if this is an RGBA (it will be the 4th value if there is one)
                    int alphaValue = 255;
                    if (values.Length > 3)
                    {
                        //the alpha portion of the rgba is not an int 0-255 it is a decimal between 0 and 1
                        //so we have to determine the corosponding byte value
                        var alphastring = values[3];
                        if (alphastring.StartsWith(".", StringComparison.InvariantCulture))
                        {
                            alphastring = "0" + alphastring;
                        }

                        var alphaDecimal = decimal.Parse(alphastring, CultureInfo.InvariantCulture);

                        if (alphaDecimal <= 1)
                        {
                            alphaValue = (int) Math.Round(alphaDecimal * 255);
                        }
                        else
                        {
                            alphaValue = (int) Math.Round(alphaDecimal);
                        }
                    }

                    Color colorpart;
                    if (values[0].Trim().EndsWith("%", StringComparison.InvariantCulture))
                    {
                        colorpart = Color.FromArgb(
                            alphaValue,
                            (int) Math.Round(255 * float.Parse(values[0].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100f),
                            (int) Math.Round(255 * float.Parse(values[1].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100f),
                            (int) Math.Round(255 * float.Parse(values[2].Trim().TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100f));
                    }
                    else
                    {
                        colorpart = Color.FromArgb(
                            alphaValue,
                            int.Parse(values[0], CultureInfo.InvariantCulture),
                            int.Parse(values[1], CultureInfo.InvariantCulture),
                            int.Parse(values[2], CultureInfo.InvariantCulture));
                    }

                    return colorpart;
                }
                catch
                {
                    throw new SvgException("Colour is in an invalid format: '" + colour + "'");
                }
            }

            // HSL support
            if (colour.StartsWith("hsl", StringComparison.InvariantCulture))
            {
                try
                {
                    int start = colour.IndexOf("(", StringComparison.InvariantCulture) + 1;

                    //get the values from the RGB string
                    string[] values = colour.Substring(start, colour.IndexOf(")", StringComparison.InvariantCulture) - start)
                        .Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (values[1].EndsWith("%", StringComparison.InvariantCulture))
                    {
                        values[1] = values[1].TrimEnd('%');
                    }

                    if (values[2].EndsWith("%", StringComparison.InvariantCulture))
                    {
                        values[2] = values[2].TrimEnd('%');
                    }

                    // Get the HSL values in a range from 0 to 1.
                    double h = double.Parse(values[0], CultureInfo.InvariantCulture) / 360.0;
                    double s = double.Parse(values[1], CultureInfo.InvariantCulture) / 100.0;
                    double l = double.Parse(values[2], CultureInfo.InvariantCulture) / 100.0;
                    // Convert the HSL color to an RGB color
                    Color colorpart = Hsl2Rgb(h, s, l);
                    return colorpart;
                }
                catch
                {
                    throw new SvgException("Colour is in an invalid format: '" + colour + "'");
                }
            }

            // HEX support
            if (colour.StartsWith("#", StringComparison.InvariantCulture) && colour.Length == 4)
            {
                colour = string.Format(culture, "#{0}{0}{1}{1}{2}{2}", colour[1], colour[2], colour[3]);
                return base.ConvertFrom(context, culture, colour);
            }

            // SystemColors support
            if (TryToGetSystemColor_OLD(colour, out var systemColor))
            {
                return systemColor;
            }

            // Numbers are handled as colors by System.Drawing.ColorConverter - we
            // have to prevent this and ignore the color instead (see #342).
            if (int.TryParse(colour, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                return SvgPaintServer.NotSet;
            }

            var index = colour.LastIndexOf("grey", StringComparison.InvariantCultureIgnoreCase);
            if (index >= 0 && index + 4 == colour.Length)
            {
                var gray = new StringBuilder(colour)
                    .Replace("grey", "gray", index, 4)
                    .Replace("Grey", "Gray", index, 4)
                    .ToString();

                return base.ConvertFrom(context, culture, gray);
            }

            return base.ConvertFrom(context, culture, colour);
        }

        public static bool TryToGetSystemColor_OLD(string colour, out Color systemColor)
        {
            switch (colour.ToLowerInvariant())
            {
                case "activeborder":
                {
                    systemColor = SystemColors.ActiveBorder;
                    return true;
                }
                case "activecaption":
                {
                    systemColor = SystemColors.ActiveCaption;
                    return true;
                }
                case "appworkspace":
                {
                    systemColor = SystemColors.AppWorkspace;
                    return true;
                }
                case "background":
                {
                    systemColor = SystemColors.Desktop;
                    return true;
                }
                case "buttonface":
                {
                    systemColor = SystemColors.Control;
                    return true;
                }
                case "buttonhighlight":
                {
                    systemColor = SystemColors.ControlLightLight;
                    return true;
                }
                case "buttonshadow":
                {
                    systemColor = SystemColors.ControlDark;
                    return true;
                }
                case "buttontext":
                {
                    systemColor = SystemColors.ControlText;
                    return true;
                }
                case "captiontext":
                {
                    systemColor = SystemColors.ActiveCaptionText;
                    return true;
                }
                case "graytext":
                {
                    systemColor = SystemColors.GrayText;
                    return true;
                }
                case "highlight":
                {
                    systemColor = SystemColors.Highlight;
                    return true;
                }
                case "highlighttext":
                {
                    systemColor = SystemColors.HighlightText;
                    return true;
                }
                case "inactiveborder":
                {
                    systemColor = SystemColors.InactiveBorder;
                    return true;
                }
                case "inactivecaption":
                {
                    systemColor = SystemColors.InactiveCaption;
                    return true;
                }
                case "inactivecaptiontext":
                {
                    systemColor = SystemColors.InactiveCaptionText;
                    return true;
                }
                case "infobackground":
                {
                    systemColor = SystemColors.Info;
                    return true;
                }
                case "infotext":
                {
                    systemColor = SystemColors.InfoText;
                    return true;
                }
                case "menu":
                {
                    systemColor = SystemColors.Menu;
                    return true;
                }
                case "menutext":
                {
                    systemColor = SystemColors.MenuText;
                    return true;
                }
                case "scrollbar":
                {
                    systemColor = SystemColors.ScrollBar;
                    return true;
                }
                case "threeddarkshadow":
                {
                    systemColor = SystemColors.ControlDarkDark;
                    return true;
                }
                case "threedface":
                {
                    systemColor = SystemColors.Control;
                    return true;
                }
                case "threedhighlight":
                {
                    systemColor = SystemColors.ControlLight;
                    return true;
                }
                case "threedlightshadow":
                {
                    systemColor = SystemColors.ControlLightLight;
                    return true;
                }
                case "window":
                {
                    systemColor = SystemColors.Window;
                    return true;
                }
                case "windowframe":
                {
                    systemColor = SystemColors.WindowFrame;
                    return true;
                }
                case "windowtext":
                {
                    systemColor = SystemColors.WindowText;
                    return true;
                }
            }

            systemColor = default;
            return false;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var colorString = ColorTranslator.ToHtml((Color)value).Replace("LightGrey", "LightGray");
                // color names are expected to be lower case in XML
                return colorString.StartsWith("#", StringComparison.InvariantCulture) ? colorString : colorString.ToLowerInvariant();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Converts HSL color (with HSL specified from 0 to 1) to RGB color.
        /// Taken from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
        /// </summary>
        /// <param name="h"></param>
        /// <param name="sl"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private static Color Hsl2Rgb(double h, double sl, double l)
        {
            double r = l;   // default to gray
            double g = l;
            double b = l;
            double v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Color rgb = Color.FromArgb((int)Math.Round(r * 255.0), (int)Math.Round(g * 255.0), (int)Math.Round(b * 255.0));
            return rgb;
        }

        /// <summary>
        /// Converts HSL color (with HSL specified from 0 to 1) to RGB color.
        /// Taken from https://geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
        /// </summary>
        /// <param name="h">The hue value.</param>
        /// <param name="sl">The saturation value.</param>
        /// <param name="l">The lightness value.</param>
        /// <param name="red">The red value.</param>
        /// <param name="green">The green value.</param>
        /// <param name="blue">The blue value.</param>
        private static void Hsl2Rgb(double h, double sl, double l, out int red, out int green, out int blue)
        {
            double r = l;   // default to gray
            double g = l;
            double b = l;
            double v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            red = (int)Math.Round(r * 255.0);
            green = (int) Math.Round(g * 255.0);
            blue = (int) Math.Round(b * 255.0);
        }
    }
}
