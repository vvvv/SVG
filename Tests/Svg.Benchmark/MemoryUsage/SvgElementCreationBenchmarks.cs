#if true
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgElementCreationBenchmarks
    {
        [Benchmark]
        public void SvgElement_new_EmptyClass()
        {
            var result = new EmptyClass();
        }

        [Benchmark(Baseline = true)]
        public void SvgElement_new_SvgElementEmpty()
        {
            var result = new SvgElementEmpty();
        }
    }
}
#endif