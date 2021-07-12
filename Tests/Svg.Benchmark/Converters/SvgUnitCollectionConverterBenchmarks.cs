using System;
using BenchmarkDotNet.Attributes;

namespace Svg.Benchmark
{
    public class SvgUnitCollectionConverterBenchmarks
    {
        [Benchmark]
        public void SvgUnitCollectionConverter_Parse()
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
    }
}
