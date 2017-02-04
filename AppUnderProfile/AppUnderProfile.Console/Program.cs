using System;
using AppUnderProfile.LibraryToProfile;
using Serilog;
using Serilog.Formatting.Json;
using StackExchange.Profiling;
using System.IO;

namespace AppUnderProfile.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniProfiler.Settings.ProfilerProvider = new SingletonProfilerProvider();

            var miniProfiler = MiniProfiler.Start();
            var queryEngine = new QueryEngine(miniProfiler);
            queryEngine.Query();

            MiniProfiler.Stop();

            EnsureDirectoriesCreated();
            WriteToSerilog(miniProfiler);
            WriteToJsonFile(miniProfiler);
        }

        static void EnsureDirectoriesCreated()
        {
            var directories = new[]
            {
                "Results",
                "Results\\Serilog",
                "Results\\Json"
            };

            foreach(var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        static void WriteToJsonFile(MiniProfiler miniProfiler)
        {
            var guid = Guid.NewGuid();
            File.WriteAllText($"Results\\Json\\{guid}.json", MiniProfiler.ToJson(miniProfiler));
        }

        static void WriteToSerilog(MiniProfiler miniProfiler)
        {
            var logger = new LoggerConfiguration()
               .WriteTo.RollingFile(new JsonFormatter(), "Results\\Serilog\\profile.json")
               .CreateLogger()
               .ForContext("Envirornment", "Live")
               .ForContext("System", "MyApplication")
               .ForContext("SubSystem", "MyService");

            var logExporter = new MiniProfilerLogExporter.MiniProfilerLogExporter(logger);

            logExporter.WriteToLogger(miniProfiler);
        }
    }
}