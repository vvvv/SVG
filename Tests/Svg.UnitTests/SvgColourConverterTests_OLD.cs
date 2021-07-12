using System.Drawing;
using System.Globalization;
using NUnit.Framework;

namespace Svg.UnitTests
{
    [TestFixture]
    public class SvgColourConverterTests_OLD
    {
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;
        private readonly SvgColourConverter _converter = new SvgColourConverter();

        [Test]
        public void Parse_OLDReturnsValidColor_html_4()
        {
            var aqua = _converter.Parse_OLD(null, _cultureInfo, "aqua");
            Assert.AreEqual(Color.Aqua, aqua);

            var black = _converter.Parse_OLD(null, _cultureInfo, "black");
            Assert.AreEqual(Color.Black, black);

            var blue = _converter.Parse_OLD(null, _cultureInfo, "blue");
            Assert.AreEqual(Color.Blue, blue);

            var fuchsia = _converter.Parse_OLD(null, _cultureInfo, "fuchsia");
            Assert.AreEqual(Color.Fuchsia, fuchsia);

            var gray = _converter.Parse_OLD(null, _cultureInfo, "gray");
            Assert.AreEqual(Color.Gray, gray);

            var green = _converter.Parse_OLD(null, _cultureInfo, "green");
            Assert.AreEqual(Color.Green, green);

            var lime = _converter.Parse_OLD(null, _cultureInfo, "lime");
            Assert.AreEqual(Color.Lime, lime);

            var maroon = _converter.Parse_OLD(null, _cultureInfo, "maroon");
            Assert.AreEqual(Color.Maroon, maroon);

            var navy = _converter.Parse_OLD(null, _cultureInfo, "navy");
            Assert.AreEqual(Color.Navy, navy);

            var olive = _converter.Parse_OLD(null, _cultureInfo, "olive");
            Assert.AreEqual(Color.Olive, olive);

            var purple = _converter.Parse_OLD(null, _cultureInfo, "purple");
            Assert.AreEqual(Color.Purple, purple);

            var red = _converter.Parse_OLD(null, _cultureInfo, "red");
            Assert.AreEqual(Color.Red, red);

            var silver = _converter.Parse_OLD(null, _cultureInfo, "silver");
            Assert.AreEqual(Color.Silver, silver);

            var teal = _converter.Parse_OLD(null, _cultureInfo, "teal");
            Assert.AreEqual(Color.Teal, teal);

            var white = _converter.Parse_OLD(null, _cultureInfo, "white");
            Assert.AreEqual(Color.White, white);

            var yellow = _converter.Parse_OLD(null, _cultureInfo, "yellow");
            Assert.AreEqual(Color.Yellow, yellow);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_system_colors()
        {
            var activeBorder = _converter.Parse_OLD(null, _cultureInfo, "ActiveBorder");
            Assert.AreEqual(SystemColors.ActiveBorder, activeBorder);

            var activeCaption = _converter.Parse_OLD(null, _cultureInfo, "ActiveCaption");
            Assert.AreEqual(SystemColors.ActiveCaption, activeCaption);

            var appWorkspace = _converter.Parse_OLD(null, _cultureInfo, "AppWorkspace");
            Assert.AreEqual(SystemColors.AppWorkspace, appWorkspace);

            // TODO: Check if SystemColors.Desktop is valid color for Background.
            var background = _converter.Parse_OLD(null, _cultureInfo, "Background");
            Assert.AreEqual(SystemColors.Desktop, background);

            var buttonFace = _converter.Parse_OLD(null, _cultureInfo, "ButtonFace");
            Assert.AreEqual(SystemColors.Control, buttonFace);

            var buttonHighlight = _converter.Parse_OLD(null, _cultureInfo, "ButtonHighlight");
            Assert.AreEqual(SystemColors.ControlLightLight, buttonHighlight);

            var buttonShadow = _converter.Parse_OLD(null, _cultureInfo, "ButtonShadow");
            Assert.AreEqual(SystemColors.ControlDark, buttonShadow);

            // TODO: Check if SystemColors.ControlText is valid color for ButtonText.
            var buttonText = _converter.Parse_OLD(null, _cultureInfo, "ButtonText");
            Assert.AreEqual(SystemColors.ControlText, buttonText);

            // TODO: Check if SystemColors.ActiveCaptionText is valid color for CaptionText.
            var captionText = _converter.Parse_OLD(null, _cultureInfo, "CaptionText");
            Assert.AreEqual(SystemColors.ActiveCaptionText, captionText);

            var grayText = _converter.Parse_OLD(null, _cultureInfo, "GrayText");
            Assert.AreEqual(SystemColors.GrayText, grayText);

            var highlight = _converter.Parse_OLD(null, _cultureInfo, "Highlight");
            Assert.AreEqual(SystemColors.Highlight, highlight);

            var highlightText = _converter.Parse_OLD(null, _cultureInfo, "HighlightText");
            Assert.AreEqual(SystemColors.HighlightText, highlightText);

            var inactiveBorder = _converter.Parse_OLD(null, _cultureInfo, "InactiveBorder");
            Assert.AreEqual(SystemColors.InactiveBorder, inactiveBorder);

            var inactiveCaption = _converter.Parse_OLD(null, _cultureInfo, "InactiveCaption");
            Assert.AreEqual(SystemColors.InactiveCaption, inactiveCaption);

            var inactiveCaptionText = _converter.Parse_OLD(null, _cultureInfo, "InactiveCaptionText");
            Assert.AreEqual(SystemColors.InactiveCaptionText, inactiveCaptionText);

            // TODO: Check if SystemColors.Info is valid color for InfoBackground.
            var infoBackground = _converter.Parse_OLD(null, _cultureInfo, "InfoBackground");
            Assert.AreEqual(SystemColors.Info, infoBackground);

            var infoText = _converter.Parse_OLD(null, _cultureInfo, "InfoText");
            Assert.AreEqual(SystemColors.InfoText, infoText);

            var menu = _converter.Parse_OLD(null, _cultureInfo, "Menu");
            Assert.AreEqual(SystemColors.Menu, menu);

            var menuText = _converter.Parse_OLD(null, _cultureInfo, "MenuText");
            Assert.AreEqual(SystemColors.MenuText, menuText);

            var scrollbar = _converter.Parse_OLD(null, _cultureInfo, "ScrollBar");
            Assert.AreEqual(SystemColors.ScrollBar, scrollbar);

            // TODO: Check if SystemColors.ControlDarkDark is valid color for ThreeDDarkShadow.
            var threeDDarkShadow = _converter.Parse_OLD(null, _cultureInfo, "ThreeDDarkShadow");
            Assert.AreEqual(SystemColors.ControlDarkDark, threeDDarkShadow);

            // TODO: Check if SystemColors.Control is valid color for ThreeDFace.
            var threeDFace = _converter.Parse_OLD(null, _cultureInfo, "ThreeDFace");
            Assert.AreEqual(SystemColors.Control, threeDFace);

            // TODO: Check if SystemColors.ControlLight is valid color for ThreeDHighlight.
            var threeDHighlight = _converter.Parse_OLD(null, _cultureInfo, "ThreeDHighlight");
            Assert.AreEqual(SystemColors.ControlLight, threeDHighlight);

            // TODO: Check if SystemColors.ControlLightLight is valid color for ThreeDLightShadow.
            var threeDLightShadow = _converter.Parse_OLD(null, _cultureInfo, "ThreeDLightShadow");
            Assert.AreEqual(SystemColors.ControlLightLight, threeDLightShadow);

            // TODO: Check if SystemColors.?? is valid color for ThreeDShadow.
            // var threeDShadow = _converter.Parse_OLD(null, _cultureInfo, "ThreeDDarkShadow");
            // Assert.AreEqual(SystemColors.??, threeDShadow);

            var window = _converter.Parse_OLD(null, _cultureInfo, "Window");
            Assert.AreEqual(SystemColors.Window, window);

            var windowFrame = _converter.Parse_OLD(null, _cultureInfo, "WindowFrame");
            Assert.AreEqual(SystemColors.WindowFrame, windowFrame);

            var windowText = _converter.Parse_OLD(null, _cultureInfo, "WindowText");
            Assert.AreEqual(SystemColors.WindowText, windowText);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_hex_rgb()
        {
            var color1 = _converter.Parse_OLD(null, _cultureInfo, "#f00");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), color1);
            Assert.AreEqual(Color.Red, color1);

            var color2 = _converter.Parse_OLD(null, _cultureInfo, "#fb0");
            Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xBB, 0x00), color2);

            var color3 = _converter.Parse_OLD(null, _cultureInfo, "#fff");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), color3);
            Assert.AreEqual(Color.White, color3);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_hex_rrggbb()
        {
            var color1 = _converter.Parse_OLD(null, _cultureInfo, "#ff0000");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), color1);
            Assert.AreEqual(Color.Red, color1);

            var color2 = _converter.Parse_OLD(null, _cultureInfo, "#00ff00");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), color2);
            Assert.AreEqual(Color.Lime, color2);

            var color3 = _converter.Parse_OLD(null, _cultureInfo, "#0000ff");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), color3);
            Assert.AreEqual(Color.Blue, color3);

            var color4 = _converter.Parse_OLD(null, _cultureInfo, "#000000");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), color4);
            Assert.AreEqual(Color.Black, color4);

            var color5 = _converter.Parse_OLD(null, _cultureInfo, "#ffffff");
            // Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), color5);
            Assert.AreEqual(Color.White, color5);

            var color6 = _converter.Parse_OLD(null, _cultureInfo, "#ffbb00");
            Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xBB, 0x00), color6);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_rgb_integer_range()
        {
            var color1 = _converter.Parse_OLD(null, _cultureInfo, "rgb(255,0,0)");
            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color1);

            var color2 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0,255,0)");
            Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), color2);

            var color3 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0,0,255)");
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 255), color3);

            var color4 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0,0,0)");
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), color4);

            var color5 = _converter.Parse_OLD(null, _cultureInfo, "rgb(255,255,255)");
            Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), color5);

            // var color6 = _converter.Parse_OLD(null, _cultureInfo, "rgb(300,0,0)");
            // Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color6);

            // var color7 = _converter.Parse_OLD(null, _cultureInfo, "rgb(255,-10,0)");
            // Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color7);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_rgb_float_range()
        {
            var color1 = _converter.Parse_OLD(null, _cultureInfo, "rgb(100%, 0%, 0%)");
            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color1);
 
            var color2 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 100%, 0%)");
            Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), color2);

            var color3 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 0%, 100%)");
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 255), color3);

            var color4 = _converter.Parse_OLD(null, _cultureInfo, "rgb(100%, 100%, 100%)");
            Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), color4);

            var color5 = _converter.Parse_OLD(null, _cultureInfo, "rgb(0%, 0%, 0%)");
            Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), color5);

            // var color6 = _converter.Parse_OLD(null, _cultureInfo, "rgb(110%, 0%, 0%)");
            // Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color6);

            // var color7 = _converter.Parse_OLD(null, _cultureInfo, "rgb(110%, -10%, 0%)");
            // Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color7);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_rgb_hsl()
        {
             var color1 = _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 100%, 50%)");
             Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), color1);

             var color2 = _converter.Parse_OLD(null, _cultureInfo, "hsl(120, 100%, 50%)");
             Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), color2);

             var color3 = _converter.Parse_OLD(null, _cultureInfo, "hsl(240, 100%, 50%)");
             Assert.AreEqual(Color.FromArgb(255, 0, 0, 255), color3);

             var color4 = _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 0%, 100%)");
             Assert.AreEqual(Color.FromArgb(255, 255, 255, 255), color4);

             var color5 = _converter.Parse_OLD(null, _cultureInfo, "hsl(0, 0%, 0%)");
             Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), color5);

             var color6 = _converter.Parse_OLD(null, _cultureInfo, "hsl(359, 0%, 0%)");
             Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), color6);

             var color7 = _converter.Parse_OLD(null, _cultureInfo, "hsl(180, 50%, 50%)");
             Assert.AreEqual(Color.FromArgb(255, 64, 191, 191), color7);

             // var color8 = _converter.Parse_OLD(null, _cultureInfo, "hsl(400, 50%, 50%)");
             // Assert.AreEqual(Color.FromArgb(255, 191, 149, 64), color8);

             // var color9 = _converter.Parse_OLD(null, _cultureInfo, "hsl(180, -50%, 50%)");
             // Assert.AreEqual(Color.FromArgb(255, 128, 128, 128), color9);

             // var color10 = _converter.Parse_OLD(null, _cultureInfo, "hsl(90, 50%, -50%)");
             // Assert.AreEqual(Color.FromArgb(255, 0, 0, 0), color10);

             // var color11 = _converter.Parse_OLD(null, _cultureInfo, "hsl(-30, 50%, 50%)");
             // Assert.AreEqual(Color.FromArgb(255, 191, 64, 128), color11);
        }

        [Test]
        public void Parse_OLDReturnsValidColor_grey()
        {
            // gray

            var g1 = _converter.Parse_OLD(null, _cultureInfo, "grey");
            Assert.AreEqual(Color.Gray, g1);

            var g2 = _converter.Parse_OLD(null, _cultureInfo, "gray");
            Assert.AreEqual(Color.Gray, g2);

            var g3 = _converter.Parse_OLD(null, _cultureInfo, "Grey");
            Assert.AreEqual(Color.Gray, g3);

            var g4 = _converter.Parse_OLD(null, _cultureInfo, "Gray");
            Assert.AreEqual(Color.Gray, g4);

            // lightgray

            var lg1 = _converter.Parse_OLD(null, _cultureInfo, "lightgrey");
            Assert.AreEqual(Color.LightGray, lg1);

            var lg2 = _converter.Parse_OLD(null, _cultureInfo, "lightgray");
            Assert.AreEqual(Color.LightGray, lg2);

            var lg3 = _converter.Parse_OLD(null, _cultureInfo, "LightGrey");
            Assert.AreEqual(Color.LightGray, lg3);

            var lg4 = _converter.Parse_OLD(null, _cultureInfo, "LightGray");
            Assert.AreEqual(Color.LightGray, lg4);

            // darkslategray

            var dsg1 = _converter.Parse_OLD(null, _cultureInfo, "darkslategrey");
            Assert.AreEqual(Color.DarkSlateGray, dsg1);

            var dsg2 = _converter.Parse_OLD(null, _cultureInfo, "darkslategray");
            Assert.AreEqual(Color.DarkSlateGray, dsg2);

            var dsg3 = _converter.Parse_OLD(null, _cultureInfo, "DarkSlateGrey");
            Assert.AreEqual(Color.DarkSlateGray, dsg3);

            var dsg4 = _converter.Parse_OLD(null, _cultureInfo, "DarkSlateGray");
            Assert.AreEqual(Color.DarkSlateGray, dsg4);
        }
    }
}
