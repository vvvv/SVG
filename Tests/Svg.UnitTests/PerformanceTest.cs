using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Svg.UnitTests
{
    [TestFixture]
    public class PerformanceTest 
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = typeof(PerformanceTest).Assembly.Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Load All W3CSvg Unit tests
        /// On My Machine this Unit Test went from 22 seconds down to 10 seconds. I set the Timeout
        /// to 25 seconds to catch Performance regressions On the Build Server it needs 9 seconds
        /// So I can move down the timeout to 15 seconds
        /// </summary>
        [Test]
        public void LoadAllW3CSvg()
        {
            var svgPath = Path.Combine(AssemblyDirectory, "..", "..", "..", "..", "W3CTestSuite", "svg");
            var files = Directory.GetFiles(svgPath, "*.svg");
            foreach (var file in files)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        using (var stream = File.OpenRead(file))
                        {
                            if (Path.GetExtension(file) == ".svgz")
                            {
                                var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                                SvgDocument.Open<SvgDocument>(gzipStream);    
                            }
                            else
                            {
                                SvgDocument.Open<SvgDocument>(stream);    
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception in svg file: {file} with Message: {e.Message}  {Environment.NewLine} Content: {Environment.NewLine} {File.ReadAllText(file)} ");
                    throw;
                }
            }
        }

        [Test]
        public void UnitConverterProfiler()
        {
            SvgUnitConverter.Parse("1pt".AsSpan());
            SvgUnitConverter.Parse("1.25px".AsSpan());
            SvgUnitConverter.Parse("1pc".AsSpan());
            SvgUnitConverter.Parse("15px".AsSpan());
            SvgUnitConverter.Parse("1mm".AsSpan());
            SvgUnitConverter.Parse("3.543307px".AsSpan());
            SvgUnitConverter.Parse("1cm".AsSpan());
            SvgUnitConverter.Parse("35.43307px".AsSpan());
            SvgUnitConverter.Parse("1in".AsSpan());
            SvgUnitConverter.Parse("90px".AsSpan());
            SvgUnitConverter.Parse("15em".AsSpan());
            SvgUnitConverter.Parse("0.2822222mm".AsSpan());
            SvgUnitConverter.Parse("3990".AsSpan());
            SvgUnitConverter.Parse("1990".AsSpan());
            SvgUnitConverter.Parse("-50".AsSpan());
            SvgUnitConverter.Parse(".4in".AsSpan());
            SvgUnitConverter.Parse(".25em".AsSpan());
            SvgUnitConverter.Parse("10%".AsSpan());
            SvgUnitConverter.Parse("1%".AsSpan());
            SvgUnitConverter.Parse("0%".AsSpan());
            SvgUnitConverter.Parse("100%".AsSpan());
            SvgUnitConverter.Parse("1.2em".AsSpan());
            SvgUnitConverter.Parse("medium".AsSpan());
            SvgUnitConverter.Parse("x-small".AsSpan());
            SvgUnitConverter.Parse("xx-large".AsSpan());
            SvgUnitConverter.Parse("657.45".AsSpan());
            SvgUnitConverter.Parse("12.5".AsSpan());
            SvgUnitConverter.Parse("0".AsSpan());
            SvgUnitConverter.Parse("12".AsSpan());
        }

        [Test]
        public void UnitCollectionConverterProfiler()
        {
            SvgUnitCollectionConverter.Parse(@"
                                               1pt
                                               1.25px
                                               1pc
                                               15px
                                               1mm
                                               3.543307px
                                               1cm
                                               35.43307px
                                               1in
                                               90px
                                               15em
                                               0.2822222mm
                                               3990
                                               1990
                                               -50
                                               .4in
                                               .25em
                                               10%
                                               1%
                                               0%
                                               100%
                                               1.2em
                                               medium
                                               x-small
                                               xx-large
                                               657.45
                                               12.5
                                               0
                                               12".AsSpan());
        }

        private static string EmptySvg = @"<svg xmlns='http://www.w3.org/2000/svg'></svg>";

        [Test]
        public void SvgDocumentFromSvgEmpty()
        {
            var doc = SvgDocument.FromSvg<SvgDocument>(EmptySvg);
        }
        
        [Test]
        public void SvgDocumentFromSvgEmptySkipGdiPlusCapabilityCheck()
        {
            SvgDocument.SkipGdiPlusCapabilityCheck = true;
            SvgDocument.DisableDtdProcessing = true;
            SvgDocument.PointsPerInch = 96;
            var doc = SvgDocument.FromSvg<SvgDocument>(EmptySvg);
            SvgDocument.DisableDtdProcessing = false;
            SvgDocument.SkipGdiPlusCapabilityCheck = false;
        }
    }
}
