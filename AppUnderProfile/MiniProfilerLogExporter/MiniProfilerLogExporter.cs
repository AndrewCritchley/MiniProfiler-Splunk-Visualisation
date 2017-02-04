using MiniProfilerLogExporter.Models;
using Serilog;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniProfilerLogExporter
{
    /// <summary>
    /// Writes timings as part of a mini profiler session to serilog.
    /// Loosely based on https://kzhendev.wordpress.com/2015/05/18/measuring-application-performance-with-mini-profiler-and-splunk/
    /// </summary>
    public class MiniProfilerLogExporter
    {
        private readonly ILogger _logger;

        public MiniProfilerLogExporter(ILogger logger)
        {
            _logger = logger;
        }

        private void WriteLog(MiniProfilerJsonEntryBase entry)
        {
            _logger.Information("Trace: {@trace}", entry);
        }

        public void WriteToLogger(MiniProfiler profiler)
        {
            var timings = new Stack<Timing>();

            if (profiler.Root.HasChildren)
            {
                var children = profiler.Root.Children;
                for (var i = children.Count - 1; i >= 0; i--) timings.Push(children[i]);
            }
            else
            {
                timings.Push(profiler.Root);
            }

            var start = new MiniProfilerJsonProfilerEntry()
            {
                Timestamp = profiler.Started,
                TransactionId = profiler.Root.Id,
                Transaction = profiler.Root.Name,
                Status = EntryStatus.Started
            };

            WriteLog(start);

            decimal runningDuration = 0;

            while (timings.Count > 0)
            {
                var timing = timings.Pop();
                var name = timing.Name;

                decimal customTimingDuration = 0;

                if (timing.HasCustomTimings)
                {
                    foreach (var item in timing.CustomTimings)
                    {
                        customTimingDuration = item.Value.Sum(v => v.DurationMilliseconds ?? 0);

                        decimal customOffset = runningDuration;

                        foreach (var item2 in item.Value)
                        {
                            customOffset += item2.DurationMilliseconds ?? 0;

                            var customTimingEntry = new MiniProfilerJsonStepEntry()
                            {
                                Timestamp = profiler.Started.AddMilliseconds(Convert.ToDouble(customOffset)),
                                TransactionId = profiler.Root.Id,
                                Transaction = profiler.Root.Name,
                                Action = string.Format("{0}/{1}", name, item.Key),
                                Duration = item2.DurationMilliseconds,
                                Status = EntryStatus.Processing
                            };

                            WriteLog(customTimingEntry);
                        }
                    }
                }

                var offset = Convert.ToDecimal(timing.DurationWithoutChildrenMilliseconds - customTimingDuration);

                var entry = new MiniProfilerJsonStepEntry()
                {
                    Timestamp = profiler.Started.AddMilliseconds(Convert.ToDouble(runningDuration)),
                    TransactionId = profiler.Root.Id,
                    Action = name,
                    Duration = offset,
                    Status = EntryStatus.Processing,
                    Transaction = profiler.Root.Name
                };

                WriteLog(entry);

                if (timing.HasChildren)
                {
                    var children = timing.Children;
                    for (var i = children.Count - 1; i >= 0; i--) timings.Push(children[i]);
                }

                runningDuration += timing.DurationWithoutChildrenMilliseconds + customTimingDuration;
            }

            var end = new MiniProfilerJsonProfilerEntry()
            {
                Timestamp = profiler.Started.AddMilliseconds(Convert.ToDouble(runningDuration)),
                TransactionId = profiler.Root.Id,
                Transaction = profiler.Root.Name,
                Status = EntryStatus.Finished
            };

            WriteLog(end);
        }
    }
}
