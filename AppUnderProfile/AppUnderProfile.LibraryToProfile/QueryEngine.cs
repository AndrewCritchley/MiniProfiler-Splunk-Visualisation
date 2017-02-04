using StackExchange.Profiling;
using System;
using System.Threading;

namespace AppUnderProfile.LibraryToProfile
{
    /// <summary>
    /// Simulates a query engine with multiple steps to profile.
    /// </summary>
    public class QueryEngine
    {
        private static readonly Random _random = new Random();
        private readonly MiniProfilerLogExporter.MiniProfilerLogExporter _miniProfilerLogExporter;

        public QueryEngine(MiniProfilerLogExporter.MiniProfilerLogExporter miniProfilerLogExporter)
        {
            _miniProfilerLogExporter = miniProfilerLogExporter;
        }

        public void Query()
        {
            MiniProfiler.Settings.ProfilerProvider = new SingletonProfilerProvider();

            var mp = MiniProfiler.Start();

            using (mp.Step("Step 1"))
            {
                Step1();
            }
            using (mp.Step("Step 2"))
            {
                Step2();
            }
            using (mp.Step("Step 3"))
            {
                Step3();
            }

            MiniProfiler.Stop();
            _miniProfilerLogExporter.WriteToLogger(mp);
        }

        private void SleepForRandomDuration()
        {
            var miliseconds = (1000 + _random.Next(-300, 300)) * _random.Next(1, 7);
            Console.WriteLine("Sleeping for {0} miliseconds", miliseconds);
            Thread.Sleep(miliseconds);
        }

        private void Step1() { SleepForRandomDuration(); }

        private void Step2() { SleepForRandomDuration(); }

        private void Step3() { SleepForRandomDuration(); }
    }
}
