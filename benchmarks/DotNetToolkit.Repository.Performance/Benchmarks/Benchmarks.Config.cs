namespace DotNetToolkit.Repository.Performance.Benchmarks
{
    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Exporters.Csv;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Loggers;
    using BenchmarkDotNet.Order;
    using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
    using Data;

    public class Config : ManualConfig
    {
        public Config()
        {
            AddLogger(ConsoleLogger.Default);
            AddExporter(CsvExporter.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddExporter(HtmlExporter.Default);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddColumn(new ProviderColumn());
            AddColumn(TargetMethodColumn.Method);
            AddColumn(StatisticColumn.Mean);
            AddColumn(BaselineRatioColumn.RatioMean);
            AddColumn(RankColumn.Arabic);
            AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory);
            AddJob(Job.Dry
                .WithLaunchCount(1)
                .WithWarmupCount(1)
                .WithIterationCount(1)
                .WithGcForce(true)
                .WithToolchain(InProcessNoEmitToolchain.Instance)
            );

            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
            Options = ConfigOptions.JoinSummary;
        }
    }
}
