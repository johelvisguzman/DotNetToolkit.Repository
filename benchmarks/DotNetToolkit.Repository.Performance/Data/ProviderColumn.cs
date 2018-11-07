using BenchmarkDotNet.Columns;

namespace DotNetToolkit.Repository.Performance.Data
{
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;
    using System.Linq;

    public class ProviderColumn : IColumn
    {
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            return benchmarkCase.Parameters.Items
                       .FirstOrDefault(item => item.Name == this.ColumnName)
                       ?.ToDisplayText() ?? "?";
        }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, ISummaryStyle style)
        {
            return GetValue(summary, benchmarkCase);
        }

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
        {
            return false;
        }

        public bool IsAvailable(Summary summary)
        {
            return true;
        }

        public string Id { get; } = nameof(ProviderColumn);

        public string ColumnName { get; } = "Provider";
        public bool AlwaysShow { get; } = true;
        public ColumnCategory Category { get; } = ColumnCategory.Job;

        public int PriorityInCategory { get; } = -10;

        public bool IsNumeric { get; } = false;

        public UnitType UnitType { get; } = UnitType.Dimensionless;

        public string Legend { get; } = "The context provider type being tested";
    }
}
