using AppUnderProfile.LibraryToProfile;
using Serilog;
using Serilog.Formatting.Json;

namespace AppUnderProfile.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                   .WriteTo.RollingFile(new JsonFormatter(), "profiling.json")
                   .CreateLogger()
                   .ForContext("Envirornment", "Live")
                   .ForContext("System", "MyApplication")
                   .ForContext("SubSystem", "MyService");

            var logExporter = new MiniProfilerLogExporter.MiniProfilerLogExporter(logger);

            var queryEngine = new QueryEngine(logExporter);

            queryEngine.Query();
        }
    }
}