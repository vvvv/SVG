using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Environments;

namespace Svg.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
            var types = typeof(Program)
                .Assembly
                .GetExportedTypes()
                .Where(r => r != typeof(Program))
                .OrderBy(r => r.Name);

			var job = Job.Default;
            var config = new ManualConfig();

            config.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            config.AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
            config.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            config.AddValidator(JitOptimizationsValidator.DontFailOnError);
            //config.AddJob(job.WithRuntime(ClrRuntime.Net461));
            //config.AddJob(job.WithRuntime(CoreRuntime.Core21));
            config.AddJob(job.WithRuntime(CoreRuntime.Core31));
            //config.AddJob(job.WithRuntime(CoreRuntime.Core50));
            config.AddDiagnoser(MemoryDiagnoser.Default);
            config.AddColumn(StatisticColumn.OperationsPerSecond);
            config.AddColumn(RankColumn.Arabic);

            var switcher = new BenchmarkSwitcher(types.ToArray());
            switcher.Run(args, config);
        }
    }
}
