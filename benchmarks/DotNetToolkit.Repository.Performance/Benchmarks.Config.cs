namespace DotNetToolkit.Repository.Performance
{
    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Exporters.Csv;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Order;
    using BenchmarkDotNet.Toolchains.InProcess;
    using Data;

    public class Config : ManualConfig
    {
        public Config()
        {
            Add(ConsoleLogger.Default);
            Add(CsvExporter.Default);
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);
            Add(new MemoryDiagnoser());
            Add(new ProviderColumn());
            Add(TargetMethodColumn.Method);
            Add(StatisticColumn.Mean);
            Add(BaselineRatioColumn.RatioMean);
            Add(RankColumn.Arabic);
            Add(BenchmarkLogicalGroupRule.ByCategory);
            Add(Job.Dry
                .WithLaunchCount(1)
                .WithWarmupCount(1)
                .WithIterationCount(1)
                .WithGcForce(true)
                .With(InProcessToolchain.Instance)
            );

            Set(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));

            SummaryPerType = false;
        }
    }
}
