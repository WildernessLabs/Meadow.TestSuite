using Meadow;
using System;
using System.Threading;

namespace LongTermMapleServiceTest;

public class MemoryMonitor
{
    private int _lastCheck;
    private int _firstCheck;
    private long _sum;
    private int _count;

    public bool CollectOnReport { get; set; }
    public long MinMemory { get; private set; } = long.MaxValue;
    public long MaxMemory { get; private set; }
    public long CurrentMemory { get; private set; }
    public long MeanMemory => _count == 0 ? 0 : _sum / _count;
    public TimeSpan RunTime => TimeSpan.FromMilliseconds(Math.Abs(_lastCheck - _firstCheck));

    public MemoryMonitor()
        : this(TimeSpan.FromSeconds(30))
    {
    }

    public void Reset()
    {
        MinMemory = long.MinValue;
        MaxMemory = 0;
        CurrentMemory = 0;
        _sum = 0;
        _count = 0;
        _firstCheck = 0;
        _lastCheck = 0;
    }

    public MemoryMonitor(TimeSpan? reportPeriod, bool collectOnReport = false)
    {
        CollectOnReport = collectOnReport;

        if (reportPeriod != null && reportPeriod > TimeSpan.Zero)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(reportPeriod.Value);
                    ReportState(false);
                }
            })
            .Start();
        }
    }

    public void ReportState(bool? collect = null)
    {
        var collectThisCycle = collect ?? CollectOnReport;

        _lastCheck = Environment.TickCount;
        if (_firstCheck == 0) _firstCheck = _lastCheck;

        CurrentMemory = GC.GetTotalMemory(collectThisCycle);

        _count++;
        _sum += CurrentMemory;

        if (CurrentMemory < MinMemory)
        {
            MinMemory = CurrentMemory;
        }

        if (CurrentMemory > MaxMemory)
        {
            MaxMemory = CurrentMemory;
        }

        Resolver.Log.Info(ToString());
    }

    public override string ToString()
    {
        return $"MEM @{RunTime:hh\\:mm\\:ss}: current: {CurrentMemory / 1000}k | min: {MinMemory / 1000}k | max: {MaxMemory / 1000}k | mean: {MeanMemory / 1000}k";
    }

}
