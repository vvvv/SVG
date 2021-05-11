using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace Svg.UnitTests
{
    public abstract class SvgTestHelper
    {
        /// <summary>
        /// Test file path.
        /// </summary>
        [Obsolete("Try not to use the file loader, please use the resource loader to ensure working of tests on all systems")]
        protected virtual string TestFile
        {
            get
            {
                const string msg = "Test file not overridden.";
                Assert.Inconclusive(msg);
                throw new NotImplementedException(msg);
            }
        }

        /// <summary>
        /// Full Unit Test resource string for test file. 
        /// </summary>
        /// <remarks>
        /// For the full Unit Test resource string you can use <see cref="GetFullResourceString(string)"/>.
        /// </remarks>
        protected virtual string TestResource
        {
            get
            {
                const string msg = "Test resource not overridden.";
                Assert.Inconclusive(msg);
                throw new NotImplementedException(msg);
            }
        }

        /// <summary>
        /// Expected size of svg file after drawing.
        /// </summary>
        protected virtual int ExpectedSize
        {
            get
            {
                const string msg = "Expected size not overridden.";
                Assert.Inconclusive(msg);
                throw new NotImplementedException(msg);
            }
        }

        /// <summary>
        /// Get full Unit Test resource string.
        /// </summary>
        /// <param name="resourcePath">Resource path.</param>
        /// <returns>Full resource string.</returns>
        /// <example>
        /// var s = GetFullResourceString("Issue204_PrivateFont.Text.svg");
        /// // s content: "Svg.UnitTests.Resources.Issue204_PrivateFont.Text.svg"
        /// </example>
        protected virtual string GetFullResourceString(string resourcePath)
        {
            const string DefaultResourcesDir = "Resources";
            return string.Format("{0}.{1}.{2}",
                this.GetType().Assembly.GetName().Name,
                DefaultResourcesDir,
                resourcePath);
        }

        /// <summary>
        /// Get embedded resource as stream from Unit Test resources.
        /// </summary>
        /// <param name="fullResourceString">Full Unit Test resource string.</param>
        /// <returns>Embedded resource data steam.</returns>
        /// <remarks>Do not forget to close, dispose the stream.</remarks>
        protected virtual Stream GetResourceStream(string fullResourceString)
        {
            Trace.WriteLine("Get resource data.");
            var s = this.GetType().Assembly.GetManifestResourceStream(fullResourceString);
            if (s == null)
                Assert.Fail("Unable to find embedded resource", fullResourceString);
            Trace.WriteLine("Done getting resource data.");
            return s;
        }

        /// <summary>
        /// Get embedded resource as byte array from Unit Test resources.
        /// </summary>
        /// <param name="fullResourceString">Full Unit Test resource string.</param>
        /// <returns>Embedded resource data bytes.</returns>
        protected virtual byte[] GetResourceBytes(string fullResourceString)
        {
            using (var s = GetResourceStream(fullResourceString))
            {
                var resource = new byte[s.Length];
                s.Read(resource, 0, (int)s.Length);
                return resource;
            }
        }

        /// <summary>
        /// Get embedded resource as xml document from Unit Test resources.
        /// </summary>
        /// <param name="fullResourceString">Full Unit Test resource string.</param>
        /// <returns>Embedded resource data xml document.</returns>
        protected virtual XmlDocument GetResourceXmlDoc(string fullResourceString)
        {
            using (var s = GetResourceStream(fullResourceString))
            {
                Trace.WriteLine("Load XmlDocument from resource data.");
                var xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = new SvgDtdResolver();
                xmlDoc.Load(s);
                Trace.WriteLine("Done XmlDocument loading from resource data.");
                return xmlDoc;
            }
        }

        /// <summary>
        /// Get embedded resource as string from Unit Test resources.
        /// </summary>
        /// <param name="fullResourceString">Full Unit Test resource string.</param>
        /// <returns>Embedded resource data xml as string.</returns>
        protected virtual string GetResourceXmlDocAsString(string fullResourceString)
        {
            using (var s = GetResourceStream(fullResourceString))
            {
                Trace.WriteLine("Load XmlDocument content from resource data.");
                using (var reader = new StreamReader(s, Encoding.UTF8))
                {
                    string value = reader.ReadToEnd();
                    Trace.WriteLine("Done XmlDocument content loading from resource data.");
                    return value;
                }
            }
        }

        /// <summary>
        /// Get xml document from <see cref="TestFile"/>.
        /// </summary>
        /// <returns>File data as xml document.</returns>
        [Obsolete("Try not to use the file loader, please use the resource loader to ensure working of tests on all systems")]
        protected virtual XmlDocument GetXMLDocFromFile()
        {
            return GetXMLDocFromFile(TestFile);
        }

        /// <summary>
        /// Get xml document from file.
        /// </summary>
        /// <param name="file">File to load.</param>
        /// <returns>File data as xml document.</returns>
        [Obsolete("Try not to use the file loader, please use the resource loader to ensure working of tests on all systems")]
        protected virtual XmlDocument GetXMLDocFromFile(string file)
        {
            if (!File.Exists(file))
                Assert.Fail("Test file missing.", file);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(file));
            return xmlDoc;
        }

        /// <summary>
        /// Get the xml document from an input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual XmlDocument GetXMLDocFromString(string input)
        {
            Trace.WriteLine("Load XmlDocument from input data.");
            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = new SvgDtdResolver();
            xmlDoc.LoadXml(input);
            Trace.WriteLine("Done XmlDocument loading from resource data.");
            return xmlDoc;
        }

        /// <summary>
        /// Get xml document from <see cref="TestResource"/>.
        /// </summary>
        /// <returns>Resource data as xml document.</returns>
        protected virtual XmlDocument GetXMLDocFromResource()
        {
            return GetResourceXmlDoc(TestResource);
        }

        /// <summary>
        /// Get xml document from resource.
        /// </summary>
        /// <param name="fullResourceString">Full Unit Test resource string.</param>
        /// <returns>Resource data as xml document.</returns>
        protected virtual XmlDocument GetXMLDocFromResource(string fullResourceString)
        {
            return GetResourceXmlDoc(fullResourceString);
        }

        /// <summary>
        /// Gets a bitmap from resource.
        /// </summary>
        /// <param name="resourcePath">Resource path.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        protected virtual Bitmap GetBitmapFromResource(string resourcePath)
        {
            var fullPath = GetFullResourceString(resourcePath);
            using (var stream = GetResourceStream(fullPath))
            {
                return new Bitmap(Image.FromStream(stream));
            }
        }

        /// <summary>
        /// Load, draw and check svg file.
        /// </summary>
        /// <param name="xml">Svg file data.</param>
        protected virtual void LoadSvg(XmlDocument xml)
        {
            Trace.WriteLine("SvgDocument open xml.");
            var svgDoc = OpenSvg(xml);
            Trace.WriteLine("Done SvgDocument open xml.");

            Trace.WriteLine("Draw svg.");
            var img = DrawSvg(svgDoc);
            Trace.WriteLine("Done drawing.");

            CheckSvg(svgDoc, img);
        }

        /// <summary>
        /// Open SVG document from XML document.
        /// </summary>
        /// <param name="xml">XML document.</param>
        /// <returns>Open SVG document.</returns>
        protected virtual SvgDocument OpenSvg(XmlDocument xml)
        {
            return SvgDocument.Open(xml);
        }

        /// <summary>
        /// Draw SVG.
        /// </summary>
        /// <param name="svgDoc">SVG document to draw.</param>
        /// <returns>SVG as image.</returns>
        protected virtual Image DrawSvg(SvgDocument svgDoc)
        {
            return svgDoc.Draw();
        }

        /// <summary>
        /// Check svg file data.
        /// </summary>
        /// <param name="svgDoc">Svg document.</param>
        /// <param name="img">Image of svg file from <paramref name="svgDoc"/>.</param>
        protected virtual void CheckSvg(SvgDocument svgDoc, Image img)
        {
            using (var ms = new MemoryStream())
            {
                img?.Save(ms, ImageFormat.Png);
                ms.Flush();
                Assert.IsTrue(ms.Length >= ExpectedSize, $"Svg file size {ms.Length} does not match expected minimum size (expected {ExpectedSize}).");
            }
        }

        /// <summary>
        /// Compare Images.
        /// </summary>
        /// <param name="img1">Image 1.</param>
        /// <param name="img2">Image 2.</param>
        /// <returns>If images are completely equal: true; otherwise: false</returns>
        protected virtual bool ImagesAreEqual(Bitmap img1, Bitmap img2)
        {
            float imgEqualPercentage; // To ignore.
            return ImagesAreEqual(img1, img2, out imgEqualPercentage);
        }

        /// <summary>
        /// Compare Images.
        /// </summary>
        /// <param name="img1">Image 1.</param>
        /// <param name="img2">Image 2.</param>
        /// <param name="imgEqualPercentage">Image equal value in percentage. 0.0% == completely unequal. 100.0% == completely equal.</param>
        /// <returns>If images are completely equal: true; otherwise: false</returns>
        protected virtual bool ImagesAreEqual(Bitmap img1, Bitmap img2, out float imgEqualPercentage)
        {
            Bitmap imgDiff; // To ignore.
            return ImagesAreEqual(img1, img2, 0, out imgEqualPercentage, out imgDiff);
        }

        /// <summary>
        /// Compare Images.
        /// </summary>
        /// <param name="image1">Image 1.</param>
        /// <param name="image2">Image 2.</param>
        /// <param name="allowedDifference">The difference allowed for each component (R, G, B, A). This is required to be set experimentally as the renders might be slightly different for each target - probably due to different anti-aliasing algorithms or settings.</param>
        /// <param name="imgEqualPercentage">Image equal value in percentage. 0.0% == completely unequal. 100.0% == completely equal.</param>
        /// <param name="diffImage">Image with red pixel where <paramref name="image1"/> and <paramref name="image2"/> are unequal.</param>
        /// <returns>If images are completely equal: true; otherwise: false</returns>
        protected virtual bool ImagesAreEqual(Bitmap image1, Bitmap image2, int allowedDifference, out float imgEqualPercentage, out Bitmap diffImage)
        {
            // Defaults.
            var diffColor = Color.Red;

            // Reset.
            imgEqualPercentage = 0;
            diffImage = null;

            // Requirements.
            if (image1 == null)
                return false;
            if (image2 == null)
                return false;
            if (image1.Size.Width < 1 && image1.Height < 1)
                return false;
            if (!image1.Size.Equals(image2.Size))
                return false;

            diffImage = new Bitmap(image1.Width, image1.Height, PixelFormat.Format32bppArgb);

            var image1Data = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var image2Data = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var diffImageData = diffImage.LockBits(new Rectangle(0, 0, diffImage.Width, diffImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var image1Bytes = new byte[image1Data.Stride * image1.Height];
            var image2Bytes = new byte[image2Data.Stride * image2.Height];
            var diffImageBytes = new byte[diffImageData.Stride * diffImage.Height];

            Marshal.Copy(image1Data.Scan0, image1Bytes, 0, image1Bytes.Length);
            Marshal.Copy(image2Data.Scan0, image2Bytes, 0, image2Bytes.Length);
            Marshal.Copy(diffImageData.Scan0, diffImageBytes, 0, diffImageBytes.Length);

            int diffPixelCount = 0;

            for (var pixelIndex = 0; pixelIndex < image1Bytes.Length; pixelIndex += 4)
            {
                for (var componentIndex = 0; componentIndex < 4; componentIndex++)
                {
                    if (Math.Abs(image1Bytes[pixelIndex + componentIndex] - image2Bytes[pixelIndex + componentIndex]) <= allowedDifference)
                    {
                        continue;
                    }

                    diffPixelCount++;

                    diffImageBytes[pixelIndex] = diffColor.B;
                    diffImageBytes[pixelIndex + 1] = diffColor.G;
                    diffImageBytes[pixelIndex + 2] = diffColor.R;
                    diffImageBytes[pixelIndex + 3] = diffColor.A;
                }
            }

            Marshal.Copy(diffImageBytes, 0, diffImageData.Scan0, diffImageBytes.Length);

            image1.UnlockBits(image1Data);
            image2.UnlockBits(image2Data);
            diffImage.UnlockBits(diffImageData);

            // Calculate percentage.
            int totalPixelCount = image1.Width * image1.Height;
            var imgDiffFactor = ((float)diffPixelCount / totalPixelCount);
            imgEqualPercentage = 100 - imgDiffFactor * 100;

            return (imgDiffFactor == 0f);
        }
    }
}
