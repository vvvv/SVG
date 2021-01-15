using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgColourConverterBenchmarks
    {
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;
        private readonly SvgColourConverter _converter = new SvgColourConverter();

        [Benchmark]
        public void SvgColourConverter_Parse_html_4()
        {
           SvgColourConverter.Parse("aqua");
           SvgColourConverter.Parse("black");
           SvgColourConverter.Parse("blue");
           SvgColourConverter.Parse("fuchsia");
           SvgColourConverter.Parse("gray");
           SvgColourConverter.Parse("green");
           SvgColourConverter.Parse("lime");
           SvgColourConverter.Parse("maroon");
           SvgColourConverter.Parse("navy");
           SvgColourConverter.Parse("olive");
           SvgColourConverter.Parse("purple");
           SvgColourConverter.Parse("red");
           SvgColourConverter.Parse("silver");
           SvgColourConverter.Parse("teal");
           SvgColourConverter.Parse("white");
           SvgColourConverter.Parse("yellow");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_html_4_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "aqua");
            _converter.Parse_OLD(null, _cultureInfo, "black");
            _converter.Parse_OLD(null, _cultureInfo, "blue");
            _converter.Parse_OLD(null, _cultureInfo, "fuchsia");
            _converter.Parse_OLD(null, _cultureInfo, "gray");
            _converter.Parse_OLD(null, _cultureInfo, "green");
            _converter.Parse_OLD(null, _cultureInfo, "lime");
            _converter.Parse_OLD(null, _cultureInfo, "maroon");
            _converter.Parse_OLD(null, _cultureInfo, "navy");
            _converter.Parse_OLD(null, _cultureInfo, "olive");
            _converter.Parse_OLD(null, _cultureInfo, "purple");
            _converter.Parse_OLD(null, _cultureInfo, "red");
            _converter.Parse_OLD(null, _cultureInfo, "silver");
            _converter.Parse_OLD(null, _cultureInfo, "teal");
            _converter.Parse_OLD(null, _cultureInfo, "white");
            _converter.Parse_OLD(null, _cultureInfo, "yellow");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_first()
        {
            var colour = "ActiveBorder".AsSpan().Trim();
            ColorConverterHelpers.TryToGetSystemColor(ref colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_middle()
        {
            var colour = "InactiveCaption".AsSpan().Trim();
            ColorConverterHelpers.TryToGetSystemColor(ref colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_last()
        {
            var colour = "WindowText".AsSpan().Trim();
            ColorConverterHelpers.TryToGetSystemColor(ref colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_first()
        {
            SvgColourConverter.Parse("ActiveBorder");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_middle()
        {
            SvgColourConverter.Parse("InactiveCaption");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_last()
        {
            SvgColourConverter.Parse("WindowText");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors()
        {
            SvgColourConverter.Parse("ActiveBorder");
            SvgColourConverter.Parse("ActiveCaption");
            SvgColourConverter.Parse("AppWorkspace");
            SvgColourConverter.Parse("Background");
            SvgColourConverter.Parse("ButtonFace");
            SvgColourConverter.Parse("ButtonHighlight");
            SvgColourConverter.Parse("ButtonShadow");
            SvgColourConverter.Parse("ButtonText");
            SvgColourConverter.Parse("CaptionText");
            SvgColourConverter.Parse("GrayText");
            SvgColourConverter.Parse("Highlight");
            SvgColourConverter.Parse("HighlightText");
            SvgColourConverter.Parse("InactiveBorder");
            SvgColourConverter.Parse("InactiveCaption");
            SvgColourConverter.Parse("InactiveCaptionText");
            SvgColourConverter.Parse("InfoBackground");
            SvgColourConverter.Parse("InfoText");
            SvgColourConverter.Parse("Menu");
            SvgColourConverter.Parse("MenuText");
            SvgColourConverter.Parse("ScrollBar");
            SvgColourConverter.Parse("ThreeDDarkShadow");
            SvgColourConverter.Parse("ThreeDFace");
            SvgColourConverter.Parse("ThreeDHighlight");
            SvgColourConverter.Parse("ThreeDLightShadow");
            // TODO: SvgColourConverter.Parse("ThreeDShadow");
            SvgColourConverter.Parse("Window");
            SvgColourConverter.Parse("WindowFrame");
            SvgColourConverter.Parse("WindowText");
        }
        
        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_first_OLD()
        {
            var colour = "ActiveBorder".Trim();
            SvgColourConverter.TryToGetSystemColor_OLD(colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_middle_OLD()
        {
            var colour = "InactiveCaption".Trim();
            SvgColourConverter.TryToGetSystemColor_OLD(colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_DIRECT_last_OLD()
        {
            var colour = "WindowText".Trim();
            SvgColourConverter.TryToGetSystemColor_OLD(colour, out var color);
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_first_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "ActiveBorder");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_middle_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "InactiveCaption");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_single_last_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "WindowText");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_system_colors_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "ActiveBorder");
            _converter.Parse_OLD(null, _cultureInfo, "ActiveCaption");
            _converter.Parse_OLD(null, _cultureInfo, "AppWorkspace");
            _converter.Parse_OLD(null, _cultureInfo, "Background");
            _converter.Parse_OLD(null, _cultureInfo, "ButtonFace");
            _converter.Parse_OLD(null, _cultureInfo, "ButtonHighlight");
            _converter.Parse_OLD(null, _cultureInfo, "ButtonShadow");
            _converter.Parse_OLD(null, _cultureInfo, "ButtonText");
            _converter.Parse_OLD(null, _cultureInfo, "CaptionText");
            _converter.Parse_OLD(null, _cultureInfo, "GrayText");
            _converter.Parse_OLD(null, _cultureInfo, "Highlight");
            _converter.Parse_OLD(null, _cultureInfo, "HighlightText");
            _converter.Parse_OLD(null, _cultureInfo, "InactiveBorder");
            _converter.Parse_OLD(null, _cultureInfo, "InactiveCaption");
            _converter.Parse_OLD(null, _cultureInfo, "InactiveCaptionText");
            _converter.Parse_OLD(null, _cultureInfo, "InfoBackground");
            _converter.Parse_OLD(null, _cultureInfo, "InfoText");
            _converter.Parse_OLD(null, _cultureInfo, "Menu");
            _converter.Parse_OLD(null, _cultureInfo, "MenuText");
            _converter.Parse_OLD(null, _cultureInfo, "Scrollbar");
            _converter.Parse_OLD(null, _cultureInfo, "ThreeDDarkShadow");
            _converter.Parse_OLD(null, _cultureInfo, "ThreeDFace");
            _converter.Parse_OLD(null, _cultureInfo, "ThreeDHighlight");
            _converter.Parse_OLD(null, _cultureInfo, "ThreeDLightShadow");
            // TODO: _converter.Parse_OLD(null, _cultureInfo, "ThreeDShadow");
            _converter.Parse_OLD(null, _cultureInfo, "Window");
            _converter.Parse_OLD(null, _cultureInfo, "WindowFrame");
            _converter.Parse_OLD(null, _cultureInfo, "WindowText");
        }
        
        [Benchmark]
        public void SvgColourConverter_Parse_hex_rgb()
        {
            SvgColourConverter.Parse("#f00");
            SvgColourConverter.Parse("#fb0");
            SvgColourConverter.Parse("#fff");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_hex_rgb_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "#f00");
            _converter.Parse_OLD(null, _cultureInfo, "#fb0");
            _converter.Parse_OLD(null, _cultureInfo, "#fff");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_hex_rrggbb()
        {
            SvgColourConverter.Parse("#ff0000");
            SvgColourConverter.Parse("#00ff00");
            SvgColourConverter.Parse("#0000ff");
            SvgColourConverter.Parse("#000000");
            SvgColourConverter.Parse("#ffffff");
            SvgColourConverter.Parse("#ffbb00");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_hex_rrggbb_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "#ff0000");
            _converter.Parse_OLD(null, _cultureInfo, "#00ff00");
            _converter.Parse_OLD(null, _cultureInfo, "#0000ff");
            _converter.Parse_OLD(null, _cultureInfo, "#000000");
            _converter.Parse_OLD(null, _cultureInfo, "#ffffff");
            _converter.Parse_OLD(null, _cultureInfo, "#ffbb00");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_rgb_integer_range()
        {
            SvgColourConverter.Parse("rgb(255,0,0)");
            SvgColourConverter.Parse("rgb(0,255,0)");
            SvgColourConverter.Parse("rgb(0,0,255)");
            SvgColourConverter.Parse("rgb(0,0,0)");
            SvgColourConverter.Parse("rgb(255,255,255)");
            // TODO: SvgColourConverter.Parse("rgb(300,0,0)");
            // TODO: SvgColourConverter.Parse("rgb(255,-10,0)");
        }
        
        [Benchmark]
        public void SvgColourConverter_Parse_rgb_integer_range_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "rgb(255,0,0)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0,255,0)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0,0,255)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0,0,0)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(255,255,255)");
            // TODO: _converter.Parse_OLD(null, _cultureInfo, "rgb(300,0,0)");
            // TODO: _converter.Parse_OLD(null, _cultureInfo, "rgb(255,-10,0)");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_rgb_float_range()
        {
            SvgColourConverter.Parse("rgb(100%, 0%, 0%)");
            SvgColourConverter.Parse("rgb(0%, 100%, 0%)");
            SvgColourConverter.Parse("rgb(0%, 0%, 100%)");
            SvgColourConverter.Parse("rgb(100%, 100%, 100%)");
            SvgColourConverter.Parse("rgb(0%, 0%, 0%)");
            // TODO: SvgColourConverter.Parse("rgb(110%, 0%, 0%)");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_rgb_float_range_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "rgb(100%, 0%, 0%)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 100%, 0%)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 0%, 100%)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(100%, 100%, 100%)");
            _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 0%, 0%)");
            // TODO: _converter.Parse_OLD(null, _cultureInfo, "rgb(110%, 0%, 0%)");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_rgb_hsl()
        {
            SvgColourConverter.Parse("hsl(0, 100%, 50%)");
            SvgColourConverter.Parse("hsl(120, 100%, 50%)");
            SvgColourConverter.Parse("hsl(240, 100%, 50%)");
            SvgColourConverter.Parse("hsl(0, 0%, 100%)");
            SvgColourConverter.Parse("hsl(0, 0%, 0%)");
            SvgColourConverter.Parse("hsl(359, 0%, 0%)");
            SvgColourConverter.Parse("hsl(180, 50%, 50%)");
        }

        [Benchmark]
        public void SvgColourConverter_Parse_rgb_hsl_OLD()
        {
            _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 100%, 50%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(120, 100%, 50%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(240, 100%, 50%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 0%, 100%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 0%, 0%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(359, 0%, 0%)");
            _converter.Parse_OLD(null, _cultureInfo, "hsl(180, 50%, 50%)");
        }
    }
}
