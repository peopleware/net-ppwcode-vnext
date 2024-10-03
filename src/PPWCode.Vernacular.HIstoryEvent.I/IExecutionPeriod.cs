using PPWCode.Util.Time.I;

namespace PPWCode.Vernacular.HistoryEvent.I;

public interface IExecutionPeriod<TExecutionPeriod, TExecution>
    where TExecutionPeriod : IPeriod<TExecution>
    where TExecution : struct, IComparable<TExecution>, IEquatable<TExecution>
{
    TExecutionPeriod? ExecutionPeriod { get; set;  }
}
