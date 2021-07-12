using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class ToStringBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void Float_ToString()
        {
            123.456f.ToString(CultureInfo.InvariantCulture);
            789.01f.ToString(CultureInfo.InvariantCulture);
            (-45.01f).ToString(CultureInfo.InvariantCulture);
            31.045f.ToString(CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void Float_ToSvgString()
        {
            123.456f.ToSvgString();
            789.01f.ToSvgString();
            (-45.01f).ToSvgString();
            31.045f.ToSvgString();
        }
    }
}
