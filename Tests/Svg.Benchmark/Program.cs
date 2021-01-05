using System;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Environments;
using Svg;

// dotnet run -c Release -f netcoreapp3.1 -- -f '*'

namespace Benchmark.Portable.Xaml
{
    public class Benchmarks
    {
        [Benchmark]
        public void SvgPathBuilder_Parse()
        {
            SvgPathBuilder.Parse(@"m -129.83,103.065 c 0.503,6.048 1.491,12.617 3.23,15.736 0,0 -3.6,12.4 5.2,25.6 0,0 -0.4,7.2 1.2,10.4 0,0 4,8.4 8.8,9.2 3.884,0.647 12.607,3.716 22.468,5.12 0,0 17.132,14.08 13.932,26.88 0,0 -0.4,16.4 -4,18 0,0 11.6,-11.2 2,5.6 l -4.4,18.8 c 0,0 25.6,-21.6 10,-3.2 l -10,26 c 0,0 19.6,-18.4 12.4,-10 l -3.2,8.8 c 0,0 43.2,-27.2 12.4,2.4 0,0 8,-3.6 12.4,-0.8 0,0 6.8,-1.2 6,0.4 0,0 -20.8,10.4 -24.4,28.8 0,0 8.4,-10 5.2,0.8 l 0.4,11.6 c 0,0 4,-21.6 3.6,16 0,0 19.2,-18 7.6,2.8 l 0,16.8 c 0,0 15.2,-16.4 8.8,-3.6 0,0 10,-8.8 6,6.4 0,0 -0.8,10.4 3.6,-0.8 0,0 16,-30.6 10,-4.4 0,0 -0.8,19.2 4,4.4 0,0 0.4,10.4 9.6,17.6 0,0 -1.2,-50.8 11.6,-14.8 l 4,16.4 c 0,0 2.8,-9.2 2.4,-14.4 0,0 14.8,-16.4 8,8 0,0 15.2,-22.8 12,-9.6 0,0 -7.6,16 -6,20.8 0,0 16.8,-34.8 18,-36.4 0,0 -2,42.401 8.8,6.4 0,0 5.6,12 2.8,16.4 0,0 8,-8 7.2,-11.2 0,0 4.6,-8.2 7.4,5.4 0,0 1.8,9.4 3.4,6.2 0,0 4,24.001 5.2,1.2 0,0 1.6,-13.6 -5.6,-25.2 0,0 0.8,-3.2 -2,-7.2 0,0 13.6,21.6 6.4,-7.2 0,0 11.201,8 12.401,8 0,0 -13.601,-23.2 -4.801,-18.4 0,0 -5.2,-10.4 12.801,1.6 0,0 -16.001,-16 1.6,-6.4 0,0 8,6.4 0.4,-3.6 0,0 -14.401,-16 7.6,2 0,0 11.6,16.4 12.4,19.2 0,0 -10,-29.2 -14.4,-32 0,0 8.4,-36.4 49.6,-20.8 0,0 6.8,17.2 11.2,-1.2 0,0 12.8,-6.4 24,21.2 0,0 4,-13.6 3.2,-16.4 0,0 6.8,1.2 6,0 0,0 13.2,4.4 14.4,3.6 0,0 6.8,6.8 7.2,3.2 0,0 9.2,2.8 7.2,-0.8 0,0 8.8,15.6 9.2,19.2 l 2.4,-14 2,2.8 c 0,0 1.6,-7.6 0.8,-8.8 -0.8,-1.2 20,6.8 24.8,27.6 l 2,8.4 c 0,0 6,-14.8 4.4,-18.8 0,0 5.2,0.8 5.6,5.2 0,0 4,-23.2 -0.8,-29.2 0,0 4.4,-0.8 5.6,2.8 l 0,-7.2 c 0,0 7.2,0.8 7.2,-1.6 0,0 4.4,-4 6.4,0.8 0,0 -12.4,-35.2 6,-16 0,0 7.2,10.8 3.6,-8 -3.6,-18.8 -7.6,-20.4 -2.8,-20.8 0,0 0.8,-3.6 -1.2,-5.2 -2,-1.6 1.2,0 1.2,0 0,0 4.8,4 -0.4,-18 0,0 6.4,1.6 -5.6,-27.6 0,0 2.8,-2.4 -1.2,-10.8 0,0 8,4.4 10.8,2.8 0,0 -0.4,-1.6 -3.6,-5.6 0,0 -21.6,-54.801 -1.2,-32.8 0,0 11.85,13.55 5.45,-9.25 0,0 -9.111,-24.01 -8.334,-28.306 l -429.547,23.02 z");
        }

        private Stream Open(string name) => typeof(Program).Assembly.GetManifestResourceStream($"Svg.Benchmark.{name}");

        [Benchmark]
        public void AJ_Digital_Camera()
        {
            using var stream = Open("__AJ_Digital_Camera.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }

        [Benchmark]
        public void Issue_134_01()
        {
            using var stream = Open("__issue-134-01.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }

        [Benchmark]
        public void Tiger()
        {
            using var stream = Open("__tiger.svg");
            SvgDocument.Open<SvgDocument>(stream);    
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var types = typeof(Program)
                .Assembly
                .GetExportedTypes()
                .Where(r => r != typeof(Program))
                .OrderBy(r => r.Name);

			//var job = Job.ShortRun;
			var job = Job.Default;
            var config = new ManualConfig();

            config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            config.AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
            config.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            config.AddValidator(JitOptimizationsValidator.DontFailOnError);
            config.AddJob(job.WithRuntime(CoreRuntime.Core31));
            //config.AddJob(job.WithRuntime(CoreRuntime.Core22));
            //config.AddJob(job.WithRuntime(ClrRuntime.Net461));
            config.AddDiagnoser(MemoryDiagnoser.Default);
            config.AddColumn(StatisticColumn.OperationsPerSecond);
            config.AddColumn(RankColumn.Arabic);

            var switcher = new BenchmarkSwitcher(types.ToArray());
            switcher.Run(args, config);
        }
    }
}
