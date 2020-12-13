using System;
using Serilog;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using ShadowStartMenu.Menu;
using System.Collections.Generic;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.DependencyInjection;

namespace ShadowStartMenu
{
    public class Initialization
    {
        public const string LOG_OUTPUT_FORMAT = "[{Timestamp:HH:mm:ss} | {Level:u3}] {LooseSource} {Message:lj}{NewLine}{Exception}";

        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IMenuSource, UmbraMenuSource>();
        }

        public static void Logging(string contentRoot, IServiceProvider provider)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.With<LooseSourceEnricher>()
                .WriteTo.Console(outputTemplate: LOG_OUTPUT_FORMAT, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(Path.Combine(contentRoot, "Logs", "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LOG_OUTPUT_FORMAT,
                    rollOnFileSizeLimit: true)
                .WriteTo.Sink(provider.GetRequiredService<Log>())
                .CreateLogger();
        }

        public class LooseSourceEnricher : ILogEventEnricher
        {
            private readonly Dictionary<string, LogEventProperty> _propertyCache = new Dictionary<string, LogEventProperty>();
            private static readonly string looseSourceName = "LooseSource";
            private static readonly string sourceName = "SourceContext";

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (logEvent.Properties.TryGetValue(sourceName, out LogEventPropertyValue? value) && value != null)
                {
                    var subs = value.ToString().Split('.');
                    if (subs.Length > 0)
                    {
                        var sub = $"[{subs[^1].Replace("\"", "")}]";
                        if (!_propertyCache.TryGetValue(sub, out LogEventProperty? property))
                        {
                            property = propertyFactory.CreateProperty(looseSourceName, sub);
                            _propertyCache.TryAdd(sub, property);
                        }
                        logEvent.AddPropertyIfAbsent(property);
                    }
                }
            }
        }
    }
}