using NUnit.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Svg.UnitTests
{
    /// <summary>
    /// Test class to test rendering masks (see issue 482).
    /// </summary>
    [TestFixture]
    public class MaskingTests : SvgTestHelper
    {
        [Test]
        public void RenderTestFileFromIssue482()
        {
            var document = OpenSvg(GetXMLDocFromResource(GetFullResourceString("Issue482_MasksNotRendered.TestSvgMasks.svg")));

            var renderedDocument = document.Draw();
            var expectedImage = GetBitmapFromResource("Issue482_MasksNotRendered.TestSvgMasks.png");

            float equalPercentage;
            Bitmap difference;

            ImagesAreEqual(renderedDocument, expectedImage, 1, out equalPercentage, out difference);

            Assert.Greater(equalPercentage, 99);
        }

        [Test]
        public void RenderVariousElementsDefaultSize()
        {
            var document = OpenSvg(GetXMLDocFromResource(GetFullResourceString("Issue482_MasksNotRendered.VariousElements.svg")));

            var renderedDocument = document.Draw();
            var expectedImage = GetBitmapFromResource("Issue482_MasksNotRendered.VariousElements.png");

            float equalPercentage;
            Bitmap difference;

            ImagesAreEqual(renderedDocument, expectedImage, 1, out equalPercentage, out difference);

            Assert.Greater(equalPercentage, 99);
        }

        [Test]
        public void RenderVariousElementsUpscaled()
        {
            var document = OpenSvg(GetXMLDocFromResource(GetFullResourceString("Issue482_MasksNotRendered.VariousElements.svg")));

            var renderedDocument = document.Draw(1440, 2560);
            var expectedImage = GetBitmapFromResource("Issue482_MasksNotRendered.VariousElements_1440x2560.png");

            float equalPercentage;
            Bitmap difference;

            ImagesAreEqual(renderedDocument, expectedImage, 1, out equalPercentage, out difference);

            Assert.Greater(equalPercentage, 99);
        }

        [Test]
        public void RenderPcb()
        {
            var document = OpenSvg(GetXMLDocFromResource(GetFullResourceString("Issue482_MasksNotRendered.PCB.svg")));

            var renderedDocument = document.Draw(1440, 2560);

            var expectedImage = GetBitmapFromResource("Issue482_MasksNotRendered.PCB.png");

            float equalPercentage;
            Bitmap difference;

            ImagesAreEqual(renderedDocument, expectedImage, 1, out equalPercentage, out difference);

            Assert.Greater(equalPercentage, 99);
        }
    }
}