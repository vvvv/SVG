using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgElementFactoryBenchmarks
    {
        private SvgElement CreateElement_TypeOf<T>(bool fragmentIsDocument) where T : SvgDocument, new()
        {
            return fragmentIsDocument
                ? typeof(T) == typeof(SvgDocument) ? new SvgDocument() : new T()
                : new SvgFragment();
        }

        private SvgElement CreateElement_NoTypeOf<T>(bool fragmentIsDocument) where T : SvgDocument, new()
        {
            return (fragmentIsDocument) ? new T() : new SvgFragment();
        }

        [Benchmark]
        public void CreateElement_TypeOf_false()
        {
            CreateElement_TypeOf<SvgDocument>(false);
        }

        [Benchmark]
        public void CreateElement_TypeOf_true()
        {
            CreateElement_TypeOf<SvgDocument>(true);
        }
        
        [Benchmark]
        public void CreateElement_NoTypeOf_false()
        {
            CreateElement_NoTypeOf<SvgDocument>(false);
        }

        [Benchmark]
        public void CreateElement_NoTypeOf_true()
        {
            CreateElement_NoTypeOf<SvgDocument>(true);
        }
    }
}