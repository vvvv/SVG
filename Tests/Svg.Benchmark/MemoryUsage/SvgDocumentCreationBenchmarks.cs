using System;
using System.Xml;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgDocumentCreationBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void SvgDocument_new()
        {
            var result = new SvgDocument();
        }

        private static string EmptySvg = @"<svg xmlns='http://www.w3.org/2000/svg'></svg>";

        [Benchmark]
        public void SvgDocument_new_FromSvg_Empty()
        {
            var doc = SvgDocument.FromSvg<SvgDocument>(EmptySvg);
        }

        [Benchmark]
        public void SvgDocument_new_FromSvg_Empty_Fast()
        {
            SvgDocument.SkipGdiPlusCapabilityCheck = true;
            SvgDocument.DisableDtdProcessing = true;
            SvgDocument.PointsPerInch = 96;
            var doc = SvgDocument.FromSvg<SvgDocument>(EmptySvg);
            SvgDocument.DisableDtdProcessing = false;
            SvgDocument.SkipGdiPlusCapabilityCheck = false;
        }
#if false
        [Benchmark]
        public void Xml_new_XmlTextReader()
        {
            var svg = EmptySvg;

            using (var strReader = new System.IO.StringReader(svg))
            {
                var reader = new SvgTextReader(strReader, null);
            }
        }

        [Benchmark]
        public void Xml_new_XmlReader_Create()
        {
            var svg = EmptySvg;

            using (var strReader = new System.IO.StringReader(svg))
            {
                var reader = XmlReader.Create(strReader);
            }
        }

        [Benchmark]
        public void SvgDocument_new_FromSvg_Internals()
        {
            var svg = EmptySvg;

            if (string.IsNullOrEmpty(svg))
            {
                throw new ArgumentNullException("svg");
            }

            using (var strReader = new System.IO.StringReader(svg))
            {
                var reader = new SvgTextReader(strReader, null)
                {
                    XmlResolver = new SvgDtdResolver(),
                    WhitespaceHandling = WhitespaceHandling.Significant,
                    DtdProcessing = SvgDocument.DisableDtdProcessing ? DtdProcessing.Ignore : DtdProcessing.Parse,
                };

                var doc = SvgDocument.Open<SvgDocument>(reader);
            }
        }
#endif
    }
}
