using System;

namespace MiniProfilerLogExporter.Models
{
    internal abstract class MiniProfilerJsonEntryBase
    {
        public DateTime Timestamp { get; set; }
        public Guid TransactionId { get; set; }
        public string Transaction { get; set; }
        public EntryStatus Status { get; set; }
    }

    internal class MiniProfilerJsonProfilerEntry : MiniProfilerJsonEntryBase
    {

    }

    internal class MiniProfilerJsonStepEntry : MiniProfilerJsonEntryBase
    {
        public decimal? Duration { get; set; }
        public string Action { get; set; }
    }
}
