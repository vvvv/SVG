using BenchmarkDotNet.Attributes;
using System.IO;

namespace Svg.Benchmark
{
    public class SvgDocumentBenchmarks
    {
        private Stream Open(string name) => typeof(Program).Assembly.GetManifestResourceStream($"Svg.Benchmark.Assets.{name}");

        [Benchmark]
        public void SvgDocument_Open_AJ_Digital_Camera()
        {
            using var stream = Open("__AJ_Digital_Camera.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }

        [Benchmark]
        public void SvgDocument_Open_Issue_134_01()
        {
            using var stream = Open("__issue-134-01.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }

        [Benchmark]
        public void SvgDocument_Open_Tiger()
        {
            using var stream = Open("__tiger.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }
    }
}
