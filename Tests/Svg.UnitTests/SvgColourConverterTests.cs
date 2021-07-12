using System;
using System.Drawing;
using NUnit.Framework;

namespace Svg.UnitTests
{
    [TestFixture]
    public class SvgColourConverterTests
    {
        [Test]
        public void ParseReturnsValidColor_html_4()
        {
            var aqua = SvgColourConverter.Parse("aqua".AsSpan());
            Assert.AreEqual(Color.Aqua, aqua);

            var black = SvgColourConverter.Parse("black".AsSpan());
            Assert.AreEqual(Color.Black, black);

            var blue = SvgColourConverter.Parse("blue".AsSpan());
            Assert.AreEqual(Color.Blue, blue);

            var fuchsia = SvgColourConverter.Parse("fuchsia".AsSpan());
            Assert.AreEqual(Color.Fuchsia, fuchsia);

            var gray = SvgColourConverter.Parse("gray".AsSpan());
            Assert.AreEqual(Color.Gray, gray);

            var green = SvgColourConverter.Parse("green".AsSpan());
            Assert.AreEqual(Color.Green, green);

            var lime = SvgColourConverter.Parse("lime".AsSpan());
            Assert.AreEqual(Color.Lime, lime);

            var maroon = SvgColourConverter.Parse("maroon".AsSpan());
            Assert.AreEqual(Color.Maroon, maroon);

            var navy = SvgColourConverter.Parse("navy".AsSpan());
            Assert.AreEqual(Color.Navy, navy);

            var olive = SvgColourConverter.Parse("olive".AsSpan());
            Assert.AreEqual(Color.Olive, olive);

            var purple = SvgColourConverter.Parse("purple".AsSpan());
            Assert.AreEqual(Color.Purple, purple);

            var red = SvgColourConverter.Parse("red".AsSpan());
            Assert.AreEqual(Color.Red, red);

            var silver = SvgColourConverter.Parse("silver".AsSpan());
            Assert.AreEqual(Color.Silver, silver);

            var teal = SvgColourConverter.Parse("teal".AsSpan());
            Assert.AreEqual(Color.Teal, teal);

            var white = SvgColourConverter.Parse("white".AsSpan());
            Assert.AreEqual(Color.White, white);

            var yellow = SvgColourConverter.Parse("yellow".AsSpan());
            Assert.AreEqual(Color.Yellow, yellow);
        }

        [Test]
        public void ParseReturnsValidColor_system_colors()
        {
            var activeBorder = SvgColourConverter.Parse("ActiveBorder".AsSpan());
            Assert.AreEqual(SystemColors.ActiveBorder, activeBorder);

            var activeCaption = SvgColourConverter.Parse("ActiveCaption".AsSpan());
            Assert.AreEqual(SystemColors.ActiveCaption, activeCaption);

            var appWorkspace = SvgColourConverter.Parse("AppWorkspace".AsSpan());
            Assert.AreEqual(SystemColors.AppWorkspace, appWorkspace);

            // TODO: Check if SystemColors.Desktop is valid color for Background.
            var background = SvgColourConverter.Parse("Background".AsSpan());
            Assert.AreEqual(SystemColors.Desktop, background);

            var buttonFace = SvgColourConverter.Parse("ButtonFace".AsSpan());
            Assert.AreEqual(SystemColors.ButtonFace, buttonFace);

            var buttonHighlight = SvgColourConverter.Parse("ButtonHighlight".AsSpan());
            Assert.AreEqual(SystemColors.ControlLightLight, buttonHighlight);

            var buttonShadow = SvgColourConverter.Parse("ButtonShadow".AsSpan());
            Assert.AreEqual(SystemColors.ControlDark, buttonShadow);

            // TODO: Check if SystemColors.ControlText is valid color for ButtonText.
            var buttonText = SvgColourConverter.Parse("ButtonText".AsSpan());
            Assert.AreEqual(SystemColors.ControlText, buttonText);

            // TODO: Check if SystemColors.ActiveCaptionText is valid color for CaptionText.
            var captionText = SvgColourConverter.Parse("CaptionText".AsSpan());
            Assert.AreEqual(SystemColors.ActiveCaptionText, captionText);

            var grayText = SvgColourConverter.Parse("GrayText".AsSpan());
            Assert.AreEqual(SystemColors.GrayText, grayText);

            var highlight = SvgColourConverter.Parse("Highlight".AsSpan());
            Assert.AreEqual(SystemColors.Highlight, highlight);

            var highlightText = SvgColourConverter.Parse("HighlightText".AsSpan());
            Assert.AreEqual(SystemColors.HighlightText, highlightText);

            var inactiveBorder = SvgColourConverter.Parse("InactiveBorder".AsSpan());
            Assert.AreEqual(SystemColors.InactiveBorder, inactiveBorder);

            var inactiveCaption = SvgColourConverter.Parse("InactiveCaption".AsSpan());
            Assert.AreEqual(SystemColors.InactiveCaption, inactiveCaption);

            var inactiveCaptionText = SvgColourConverter.Parse("InactiveCaptionText".AsSpan());
            Assert.AreEqual(SystemColors.InactiveCaptionText, inactiveCaptionText);

            // TODO: Check if SystemColors.Info is valid color for InfoBackground.
            var infoBackground = SvgColourConverter.Parse("InfoBackground".AsSpan());
            Assert.AreEqual(SystemColors.Info, infoBackground);

            var infoText = SvgColourConverter.Parse("InfoText".AsSpan());
            Assert.AreEqual(SystemColors.InfoText, infoText);

            var menu = SvgColourConverter.Parse("Menu".AsSpan());
            Assert.AreEqual(SystemColors.Menu, menu);

            var menuText = SvgColourConverter.Parse("MenuText".AsSpan());
            Assert.AreEqual(SystemColors.MenuText, menuText);

            var scrollbar = SvgColourConverter.Parse("ScrollBar".AsSpan());
            Assert.AreEqual(SystemColors.ScrollBar, scrollbar);

            // TODO: Check if SystemColors.ControlDarkDark is valid color for ThreeDDarkShadow.
            var threeDDarkShadow = SvgColourConverter.Parse("ThreeDDarkShadow".AsSpan());
            Assert.AreEqual(SystemColors.ControlDarkDark, threeDDarkShadow);

            // TODO: Check if SystemColors.Control is valid color for ThreeDFace.
            var threeDFace = SvgColourConverter.Parse("ThreeDFace".AsSpan());
            Assert.AreEqual(SystemColors.Control, threeDFace);

            // TODO: Check if SystemColors.ControlLight is valid color for ThreeDHighlight.
            var threeDHighlight = SvgColourConverter.Parse("ThreeDHighlight".AsSpan());
            Assert.AreEqual(SystemColors.ControlLight, threeDHighlight);

            // TODO: Check if SystemColors.ControlLightLight is valid color for ThreeDLightShadow.
            var threeDLightShadow = SvgColourConverter.Parse("ThreeDLightShadow".AsSpan());
            Assert.AreEqual(SystemColors.ControlLightLight, threeDLightShadow);

            // TODO: Check if SystemColors.?? is valid color for ThreeDShadow.
            // var threeDShadow = SvgColourConverter.Parse("ThreeDDarkShadow".AsSpan());
            // Assert.AreEqual(SystemColors.??, threeDShadow);

            var window = SvgColourConverter.Parse("Window".AsSpan());
            Assert.AreEqual(SystemColors.Window, window);

            var windowFrame = SvgColourConverter.Parse("WindowFrame".AsSpan());
            Assert.AreEqual(SystemColors.WindowFrame, windowFrame);

            var windowText = SvgColourConverter.Parse("WindowText".AsSpan());
            Assert.AreEqual(SystemColors.WindowText, windowText);
        }

        [Test]
        public void ParseReturnsValidColor_hex_rgb()
        {
            var color1 = SvgColourConverter.Parse("#f00".AsSpan());
            Assert.AreEqual(Color.Red, color1);

            var color2 = SvgColourConverter.Parse("#fb0".AsSpan());
            Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xBB, 0x00), color2);

            var color3 = SvgColourConverter.Parse("#fff".AsSpan());
            Assert.AreEqual(Color.White, color3);
        }

        [Test]
        public void ParseReturnsValidColor_hex_rrggbb()
        {
            var color1 = SvgColourConverter.Parse("#ff0000".AsSpan());
            Assert.AreEqual(Color.Red, color1);

            var color2 = SvgColourConverter.Parse("#00ff00".AsSpan());
            Assert.AreEqual(Color.Lime, color2);

            var color3 = SvgColourConverter.Parse("#0000ff".AsSpan());
            Assert.AreEqual(Color.Blue, color3);

            var color4 = SvgColourConverter.Parse("#000000".AsSpan());
            Assert.AreEqual(Color.Black, color4);

            var color5 = SvgColourConverter.Parse("#ffffff".AsSpan());
            Assert.AreEqual(Color.White, color5);

            var color6 = SvgColourConverter.Parse("#ffbb00".AsSpan());
            Assert.AreEqual(Color.FromArgb(0xFF, 0xFF, 0xBB, 0x00), color6);
        }

        [Test]
        public void ParseReturnsValidColor_rgb_integer_range()
        {
            var color1 = SvgColourConverter.Parse("rgb(255,0,0)".AsSpan());
            Assert.AreEqual(Color.Red, color1);

            var color2 = SvgColourConverter.Parse("rgb(0,255,0)".AsSpan());
            Assert.AreEqual(Color.Lime, color2);

            var color3 = SvgColourConverter.Parse("rgb(0,0,255)".AsSpan());
            Assert.AreEqual(Color.Blue, color3);

            var color4 = SvgColourConverter.Parse("rgb(0,0,0)".AsSpan());
            Assert.AreEqual(Color.Black, color4);

            var color5 = SvgColourConverter.Parse("rgb(255,255,255)".AsSpan());
            Assert.AreEqual(Color.White, color5);

            var color6 = SvgColourConverter.Parse("rgb(300,0,0)".AsSpan());
            Assert.AreEqual(Color.Red, color6);

            var color7 = SvgColourConverter.Parse("rgb(255,-10,0)".AsSpan());
            Assert.AreEqual(Color.Red, color7);
        }

        [Test]
        public void ParseReturnsValidColor_rgb_float_range()
        {
            var color1 = SvgColourConverter.Parse("rgb(100%, 0%, 0%)".AsSpan());
            Assert.AreEqual(Color.Red, color1);
 
            var color2 = SvgColourConverter.Parse("rgb(0%, 100%, 0%)".AsSpan());
            Assert.AreEqual(Color.Lime, color2);

            var color3 = SvgColourConverter.Parse("rgb(0%, 0%, 100%)".AsSpan());
            Assert.AreEqual(Color.Blue, color3);

            var color4 = SvgColourConverter.Parse("rgb(100%, 100%, 100%)".AsSpan());
            Assert.AreEqual(Color.White, color4);

            var color5 = SvgColourConverter.Parse("rgb(0%, 0%, 0%)".AsSpan());
            Assert.AreEqual(Color.Black, color5);

            var color6 = SvgColourConverter.Parse("rgb(110%, 0%, 0%)".AsSpan());
            Assert.AreEqual(Color.Red, color6);

            var color7 = SvgColourConverter.Parse("rgb(110%, -10%, 0%)".AsSpan());
            Assert.AreEqual(Color.Red, color7);
        }

        [Test]
        public void ParseReturnsValidColor_rgb_hsl()
        {
             var color1 = SvgColourConverter.Parse("hsl(0, 100%, 50%)".AsSpan());
             Assert.AreEqual(Color.Red, color1);

             var color2 = SvgColourConverter.Parse("hsl(120, 100%, 50%)".AsSpan());
             Assert.AreEqual(Color.Lime, color2);

             var color3 = SvgColourConverter.Parse("hsl(240, 100%, 50%)".AsSpan());
             Assert.AreEqual(Color.Blue, color3);

             var color4 = SvgColourConverter.Parse("hsl(0, 0%, 100%)".AsSpan());
             Assert.AreEqual(Color.White, color4);

             var color5 = SvgColourConverter.Parse("hsl(0, 0%, 0%)".AsSpan());
             Assert.AreEqual(Color.Black, color5);

             var color6 = SvgColourConverter.Parse("hsl(359, 0%, 0%)".AsSpan());
             Assert.AreEqual(Color.Black, color6);

             var color7 = SvgColourConverter.Parse("hsl(180, 50%, 50%)".AsSpan());
             Assert.AreEqual(Color.FromArgb(255, 64, 191, 191), color7);

             var color8 = SvgColourConverter.Parse("hsl(400, 50%, 50%)".AsSpan());
             Assert.AreEqual(Color.FromArgb(255, 191, 149, 64), color8);

             var color9 = SvgColourConverter.Parse("hsl(180, -50%, 50%)".AsSpan());
             Assert.AreEqual(Color.Gray, color9);

             var color10 = SvgColourConverter.Parse("hsl(90, 50%, -50%)".AsSpan());
             Assert.AreEqual(Color.Black, color10);

             var color11 = SvgColourConverter.Parse("hsl(-30, 50%, 50%)".AsSpan());
             Assert.AreEqual(Color.FromArgb(255, 191, 64, 128), color11);
        }

        [Test]
        public void ParseReturnsValidColor_grey()
        {
            // gray

            var g1 = SvgColourConverter.Parse("grey".AsSpan());
            Assert.AreEqual(Color.Gray, g1);

            var g2 = SvgColourConverter.Parse("gray".AsSpan());
            Assert.AreEqual(Color.Gray, g2);

            var g3 = SvgColourConverter.Parse("Grey".AsSpan());
            Assert.AreEqual(Color.Gray, g3);

            var g4 = SvgColourConverter.Parse("Gray".AsSpan());
            Assert.AreEqual(Color.Gray, g4);

            // lightgray

            var lg1 = SvgColourConverter.Parse("lightgrey".AsSpan());
            Assert.AreEqual(Color.LightGray, lg1);

            var lg2 = SvgColourConverter.Parse("lightgray".AsSpan());
            Assert.AreEqual(Color.LightGray, lg2);

            var lg3 = SvgColourConverter.Parse("LightGrey".AsSpan());
            Assert.AreEqual(Color.LightGray, lg3);

            var lg4 = SvgColourConverter.Parse("LightGray".AsSpan());
            Assert.AreEqual(Color.LightGray, lg4);

            // darkslategray

            var dsg1 = SvgColourConverter.Parse("darkslategrey".AsSpan());
            Assert.AreEqual(Color.DarkSlateGray, dsg1);

            var dsg2 = SvgColourConverter.Parse("darkslategray".AsSpan());
            Assert.AreEqual(Color.DarkSlateGray, dsg2);

            var dsg3 = SvgColourConverter.Parse("DarkSlateGrey".AsSpan());
            Assert.AreEqual(Color.DarkSlateGray, dsg3);

            var dsg4 = SvgColourConverter.Parse("DarkSlateGray".AsSpan());
            Assert.AreEqual(Color.DarkSlateGray, dsg4);
        }
    }
}
