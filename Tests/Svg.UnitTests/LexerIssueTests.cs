using NUnit.Framework;

namespace Svg.UnitTests
{
    /// <summary>
    /// Some complexer stylesheets seemed to crash the lexer and result in errors (#399)
    /// Found several issues in the lexer and parser of the SVG file. 
    /// 
    /// This test class will test against these issues to prevent the bug from reoccuring
    /// </summary>
    [TestFixture]
    public class LexerIssueTests : SvgTestHelper
    {
        private const string ResourceStringEmptyDTagFile = "Issue399_LexerIssue.EmptyDTag.svg";

        private const string ResourceStringLexerTemplate = "Issue399_LexerIssue.LexerTestTemplate.svg";
        private const string LexerTemplateReplaceToken = "/*[REPLACE]*/";

        /// <summary>
        /// We encountered issues in the example file that were caused by an empty d tag in some of the elements
        /// </summary>
        [Test]
        public void Lexer_FileWithEmptyDAttribute_Success()
        {
            var xml = GetXMLDocFromResource(GetFullResourceString(ResourceStringEmptyDTagFile));
            var doc = OpenSvg(xml);
        }

        /// <summary>
        /// Stylesheet lexer fails if there is an important after a hex value, this tests the 3 and 6 lenght variants in the file
        /// </summary>
        [Test]
        [TestCase("border-top: 1px solid #333 !important;")] //Important should be valid on 3 digit hex
        [TestCase("border-top: 1px solid #009c46 !important;")] //Important should be valid on 6 digit hex
        [TestCase("border-bottom: 1px solid #009c46  ;")] //Whitespace should not break the parser
        public void Lexer_ImportantAfterHex_Success(string testString)
        {
            GenerateLexerTestFile(testString);
        }

        /// <summary>
        ///Reference test if there is an important after a non-hex value (should never fail) and on hex without an important
        /// </summary>
        [Test]
        [TestCase("border-top: 1px solid #009c46;")] //Hex is working, if no !important suffixing it (#399)
        [TestCase("border-top: 1px solid red !important;")] //Important is not failing on non-hex value
        public void Lexer_NoImportantAndImportantAfterNonHex_Success(string testString)
        {
            GenerateLexerTestFile(testString);
        }

        [Test]
        public void Lexer_FileWithValidHex_ColorTagIsIssued()
        {
            // valid colors
            var doc = GenerateLexerTestFile("fill: #ff0000; stroke: #ffff00");
            var path = doc.GetElementById<SvgPath>("path1");
            Assert.AreEqual(System.Drawing.Color.Red, ((SvgColourServer)path.Fill).Colour);
            Assert.AreEqual(System.Drawing.Color.Yellow, ((SvgColourServer)path.Stroke).Colour);
        }

        [Test]
        public void Lexer_FileWithInvalidHex_ColorTagOnlyReverstToDefaultForTheInvalidTag()
        {
            // invalid/valid color combinations - default color is used for each invalid color
            var doc = GenerateLexerTestFile("fill: #ff00; stroke: #00ff00");
            var path = doc.GetElementById<SvgPath>("path1");
            // default fill color is Black
            Assert.AreEqual(System.Drawing.Color.Black, ((SvgColourServer)path.Fill).Colour);
            Assert.AreEqual(System.Drawing.Color.Lime, ((SvgColourServer)path.Stroke).Colour);
        }

        [Test]
        public void Lexer_FileWithInvalidHex_ColorTagResolvesToTransparent()
        {
            var doc = GenerateLexerTestFile("fill: #fff; stroke: 005577");
            var path = doc.GetElementById<SvgPath>("path1");
            Assert.AreEqual(System.Drawing.Color.White, ((SvgColourServer)path.Fill).Colour);
            // default stroke color is transparent
            Assert.AreEqual(SvgPaintServer.NotSet, path.Stroke);
        }

        [Test]
        public void Lexer_FileWithInvalidHex_EmptyColorTagIsIgnored()
        {
            var doc = GenerateLexerTestFile("fill:; stroke: #00557;");
            var path = doc.GetElementById<SvgPath>("path1");
            Assert.AreEqual(System.Drawing.Color.Black, ((SvgColourServer)path.Fill).Colour);
            Assert.IsNull(path.Stroke);
        }

        /// <summary>
        /// Load the lexer template and replace the token with a test value and try generating the svg
        /// </summary>
        /// <param name="testContent">Content to test in the template</param>
        /// <returns></returns>
        private SvgDocument GenerateLexerTestFile(string testContent)
        {
            var str = GetResourceXmlDocAsString(GetFullResourceString(ResourceStringLexerTemplate));
            str = str.Replace(LexerTemplateReplaceToken, testContent);
            var xml = GetXMLDocFromString(str);
            var doc = OpenSvg(xml);
            if(doc == null) { Assert.Fail("Expected a document result, but got null"); }
            return doc;
        }

    }
}
