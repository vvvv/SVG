using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Svg.UnitTests
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ImageComparisonTest
    {
        public TestContext TestContext { get; set; }

#if NETSTANDARD || NETCORE
        /// <summary>
        /// Compares SVG images against reference PNG images from the W3C SVG 1.1 test suite.
        /// This tests 158 out of 179 passing tests - the rest will not pass
        /// the test for several reasons.
        /// Note that with the current test there are still a lot of false positives,
        /// so this is not a definitive test for image equality yet.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(ImageTestDataSource), nameof(ImageTestDataSource.PassingTests))]
        public void CompareSvgImageWithReference(ImageTestDataSource.TestData testData)
        {
            // W3C Test Suites use external references to local fonts
            SvgDocument.ResolveExternalXmlEntites = ExternalType.Local;
            SvgDocument.ResolveExternalElements = ExternalType.Local;
            SvgDocument.ResolveExternalImages = ExternalType.Local;
            
            string basePath = testData.BasePath;
            string baseName = testData.BaseName;
            bool testSaveLoad = !baseName.StartsWith("#");
            if (!testSaveLoad)
            {
                baseName = baseName.Substring(1);
            }
            var svgPath = Path.Combine(basePath, "svg", baseName + ".svg");
            var pngPath = Path.Combine(basePath, "png", baseName + ".png");
            CompareSvgImageWithReferenceImpl(baseName, svgPath, pngPath, testSaveLoad);
        }
#else
        [Test]
        [TestCaseSource(typeof(ImageTestDataSource), "PassingTests")]
        public void CompareSvgImageWithReference(ImageTestDataSource.TestData testData)
        {
            // W3C Test Suites use external references to local fonts
            SvgDocument.ResolveExternalXmlEntites = ExternalType.Local;
            SvgDocument.ResolveExternalElements = ExternalType.Local;
            SvgDocument.ResolveExternalImages = ExternalType.Local;

            var basePath = testData.BasePath;
            while (!basePath.ToLower().EndsWith("svg"))
            {
                basePath = Path.GetDirectoryName(basePath);
            }
            basePath = Path.Combine(Path.Combine(basePath, "Tests"), "W3CTestSuite");
            var svgBasePath = Path.Combine(basePath, "svg");
            var baseName = testData.BaseName;
            bool testSaveLoad = !baseName.StartsWith("#");
            if (!testSaveLoad)
            {
                baseName = baseName.Substring(1);
            }
            var svgPath = Path.Combine(Path.Combine(basePath, "svg"), baseName + ".svg");
            var pngPath = Path.Combine(Path.Combine(basePath, "png"), baseName + ".png");
            CompareSvgImageWithReferenceImpl(baseName, svgPath, pngPath, testSaveLoad);
        }
#endif
        
        

        public static void CompareSvgImageWithReferenceImpl(string baseName,
            string svgPath, string pngPath, bool testSaveLoad)
        {
            var svgDoc = LoadSvgDocument(svgPath);
            Assert.IsNotNull(svgDoc);
            bool useFixedSize = !baseName.StartsWith("__");

            using (var pngImage = Image.FromFile(pngPath))
            {
                using (var svgImage = LoadSvgImage(svgDoc, useFixedSize))
                {
                    Assert.AreNotEqual(null, pngImage, "Failed to load " + pngPath);
                    Assert.AreNotEqual(null, svgImage, "Failed to load " + svgPath);
                    var difference = svgImage.PercentageDifference(pngImage);
                    Assert.IsTrue(difference < 0.05, baseName + ": Difference is " + (difference * 100.0).ToString() + "%");
                }
                if (!testSaveLoad)
                {
                    // for some images, save/load is still failing
                    return;
                }

                // test save/load
                using (var memStream = new MemoryStream())
                {
                    svgDoc.Write(memStream);
                    memStream.Position = 0;
                    var baseUri = svgDoc.BaseUri;
                    svgDoc = SvgDocument.Open<SvgDocument>(memStream);
                    svgDoc.BaseUri = baseUri;
                    using (var svgImage = LoadSvgImage(svgDoc, useFixedSize))
                    {
                        Assert.IsNotNull(svgImage);
                        var difference = svgImage.PercentageDifference(pngImage);
                        Assert.IsTrue(difference < 0.05, baseName + ": Difference is " + (difference * 100.0).ToString() + "%");
                    }
                }
            }
        }

        /// <summary>
        /// Enable this test to output the calculate percentage difference
        /// of all considered W3C tests.
        /// Can be used to enhance the difference calculation.
        /// </summary>
        // [TestFixture]
        public void RecordDiffForAllSvgImagesWithReference()
        {
#if NETSTANDARD || NETCORE
            var basePath = Path.Combine(ImageTestDataSource.SuiteTestsFolder, "W3CTestSuite");
#else
            var basePath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDirectory))); //TODO: Tthe get dir name was parsed from the testparams -> Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestRunDirectory)));
            basePath = Path.Combine(Path.Combine(basePath, "Tests"), "W3CTestSuite");
#endif
            string[] lines = File.ReadAllLines(@"..\..\..\..\Tests\Svg.UnitTests\all.csv");
            foreach (var baseName in lines)
            {
                var svgPath = Path.Combine(Path.Combine(basePath, "svg"), baseName + ".svg");
                var pngPath = Path.Combine(Path.Combine(basePath, "png"), baseName + ".png");
                if (File.Exists(pngPath) && File.Exists(svgPath))
                {
                    var svgDoc = LoadSvgDocument(svgPath);
                    bool useFixedSize = !baseName.StartsWith("__");
                    using (var pngImage = Image.FromFile(pngPath))
                    using (var svgImage = LoadSvgImage(svgDoc, useFixedSize))
                    {
                        var difference = svgImage.PercentageDifference(pngImage);
                        Console.WriteLine(baseName + " " + (difference * 100.0).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Load the SVG image the same way as in the SVGW3CTestRunner.
        /// </summary>
        private static Bitmap LoadSvgImage(SvgDocument svgDoc, bool usedFixedSize)
        {
            Bitmap svgImage = null;
            try
            {
                if (usedFixedSize)
                {
                    svgImage = new Bitmap(480, 360);
                    svgDoc.Draw(svgImage);
                }
                else
                {
                    svgImage = svgDoc.Draw();
                }
            }
            catch (Exception)
            {
                svgImage?.Dispose();
                throw;
            }
            return svgImage;
        }

        private static SvgDocument LoadSvgDocument(string svgPath)
        {
            return SvgDocument.Open(svgPath);
        }
    }

    /// <summary>
    /// Taken from https://web.archive.org/web/20130111215043/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    /// and slightly modified.
    /// Image width and height, default threshold and handling of alpha values have been adapted.
    /// </summary>
    static class ExtensionMethods
    {
        private static readonly int ImageWidth = 64;
        private static readonly int ImageHeight = 64;

        public static float PercentageDifference(this Image img1, Image img2, byte threshold = 10)
        {
            byte[,] differences = img1.GetDifferences(img2);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / (float)(differences.GetLength(0) * differences.GetLength(1));
        }

        public static Bitmap Resize(this Image originalImage, int newWidth, int newHeight)
        {
            if (originalImage.Width > originalImage.Height)
                newWidth = originalImage.Width * newHeight / originalImage.Height;
            else
                newHeight = originalImage.Height * newWidth / originalImage.Width;

            var smallVersion = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, smallVersion.Width, smallVersion.Height);
            }

            return smallVersion;
        }

        public static byte[,] GetGrayScaleValues(this Bitmap img)
        {
            byte[,] grayScale = new byte[img.Width, img.Height];

            for (int y = 0; y < grayScale.GetLength(1); y++)
            {
                for (int x = 0; x < grayScale.GetLength(0); x++)
                {
                    var alpha = img.GetPixel(x, y).A;
                    var gray = img.GetPixel(x, y).R;
                    grayScale[x, y] = (byte)Math.Abs(gray * alpha / 255);
                }
            }
            return grayScale;
        }

        // the colormatrix needed to grayscale an image
        static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        public static Bitmap GetGrayScaleVersion(this Bitmap original)
        {
            // create a blank bitmap the same size as original
            // https://web.archive.org/web/20130111215043/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
            var newBitmap = new Bitmap(original.Width, original.Height);

            // get a graphics object from the new image
            using (var g = Graphics.FromImage(newBitmap))
            // create some image attributes
            using (var attributes = new ImageAttributes())
            {
                // set the color matrix attribute
                attributes.SetColorMatrix(ColorMatrix);

                // draw the original image on the new image
                // using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return newBitmap;
        }

        public static byte[,] GetDifferences(this Image img1, Image img2)
        {
            using (var resizedThisOne = img1.Resize(ImageWidth, ImageHeight))
            using (var thisOne = resizedThisOne.GetGrayScaleVersion())
            using (var resizedTheOtherOne = img2.Resize(ImageWidth, ImageHeight))
            using (var theOtherOne = resizedTheOtherOne.GetGrayScaleVersion())
            {
                byte[,] differences = new byte[thisOne.Width, thisOne.Height];
                byte[,] firstGray = thisOne.GetGrayScaleValues();
                byte[,] secondGray = theOtherOne.GetGrayScaleValues();

                for (int y = 0; y < differences.GetLength(1); y++)
                {
                    for (int x = 0; x < differences.GetLength(0); x++)
                    {
                        differences[x, y] = (byte)Math.Abs(firstGray[x, y] - secondGray[x, y]);
                    }
                }
                return differences;
            }
        }
    }

    /// <summary>
    /// Helper class to read the datasource for the image tests.
    /// The datasource will read the embedded resource with the Tests and will pass a TestData class with the data to test.
    /// </summary>
    public class ImageTestDataSource
    {
        public class TestData
        {
            public string BasePath { get; set; }
            public string BaseName { get; set; }
            public override string ToString()
            {
                return $"TestDataSource - {BaseName}";
            }
        }

        public static IEnumerable<TestData> PassingTests()
        {
            var basePath = SuiteTestsFolder;
            var testSuite = Path.Combine(basePath, "W3CTestSuite");
            var rows = new ImageTestDataSource().LoadRowsFromResourceCsv().Skip(1); // Skip header row
            foreach (var row in rows)
                yield return new TestData() { BasePath = testSuite, BaseName = row };
        }

        private const string ResourceIdentifier = "PassingTests.csv";

        /// <summary>
        /// Read the rows from the resource
        /// </summary>
        /// <returns></returns>
        private string[] LoadRowsFromResourceCsv()
        {
            var assembly = typeof(ImageTestDataSource).Assembly;
            string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(r => r.IndexOf(ResourceIdentifier) > -1);
            if (resourceName == null) throw new Exception($"Cannot find data resource: {ResourceIdentifier}");
            var res = string.Empty;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                res = reader.ReadToEnd();
            }

            return res.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string _basePath = null;

        /// <summary>
        /// Determine the folder the testsuite is running in.
        /// </summary>
        public static string SuiteTestsFolder
        {
            get
            {
                if (_basePath != null) return _basePath;
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                while (!basePath.ToLower().EndsWith("svg"))
                {
                    basePath = Path.GetDirectoryName(basePath);
                }

                _basePath = Path.Combine(basePath, "Tests");
                return _basePath;
            }
        }
    }
}
