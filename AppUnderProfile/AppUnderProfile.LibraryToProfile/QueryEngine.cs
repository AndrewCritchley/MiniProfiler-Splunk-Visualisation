using StackExchange.Profiling;
using System;
using System.IO;
using System.Threading;

namespace AppUnderProfile.LibraryToProfile
{
    /// <summary>
    /// Simulates a query engine with multiple steps to profile.
    /// </summary>
    public class QueryEngine
    {
        private static readonly Random _random = new Random();
        private readonly MiniProfiler _miniProfiler;

        public QueryEngine(MiniProfiler miniProfiler)
        {
            _miniProfiler = miniProfiler;
        }

        public void Query()
        {
            using (_miniProfiler.Step("Step 1"))
            {
                Step1();
            }
            using (_miniProfiler.Step("Step 2"))
            {
                Step2();
            }
            using (_miniProfiler.Step("Step 3"))
            {
                Step3();
            }
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
