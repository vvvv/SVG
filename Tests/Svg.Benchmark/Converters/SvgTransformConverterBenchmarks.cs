using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using Svg;
using Svg.Transforms;

namespace Svg.Benchmark
{
    public class SvgTransformConverterBenchmarks
    {
        [Benchmark]
        public void SvgTransformConverter_Parse_Matrix_1()
        {
            SvgTransformConverter.Parse("matrix(252,0,0,252,7560,11340)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Matrix_2()
        {
            SvgTransformConverter.Parse("matrix(-4.37114e-08,1,-1,-4.37114e-08,181,409.496)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Matrix_3()
        {
            SvgTransformConverter.Parse("matrix(0.74811711,0.48689734,-0.42145482,0.93331568,324.55155,94.282562)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Matrix_4()
        {
            SvgTransformConverter.Parse("matrix(0.879978,0.475015,-0.475015,0.879978,120.2732,-136.2899)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Rotate_Translate()
        {
            SvgTransformConverter.Parse("rotate(180), translate(-50, 0)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Translate_Rotate()
        {
            SvgTransformConverter.Parse("translate(9, 241) rotate(-90)".AsSpan());
        }

        [Benchmark]
        public void SvgTransformConverter_Parse_Matrix_Rotate_Scale()
        {
            SvgTransformConverter.Parse("rotate(180 2.5 2.5) scale(0.7142857142857143,0.7142857142857143)".AsSpan());
        }
    }
}
