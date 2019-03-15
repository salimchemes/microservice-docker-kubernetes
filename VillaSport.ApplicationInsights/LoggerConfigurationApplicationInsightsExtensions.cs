using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using VillaSport.ApplicationInsights.Sinks.ApplicationInsights;
using VillaSport.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace VillaSport.ApplicationInsights
{
    /// <summary>
    /// Adds the WriteTo.ApplicationInsights() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationApplicationInsightsExtensions
    {
        /// <summary>
        /// Adds a Serilog sink that writes <see cref="LogEvent">log events</see> to Microsoft Application Insights 
        /// using a custom <see cref="ITelemetry"/> converter / constructor.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="telemetryConfiguration">Required Application Insights configuration settings.</param>
        /// <param name="telemetryConverter">Required telemetry converter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration ApplicationInsights(
            this LoggerSinkConfiguration loggerConfiguration,
            TelemetryConfiguration telemetryConfiguration,
            ITelemetryConverter telemetryConverter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var client = new TelemetryClient(telemetryConfiguration ?? TelemetryConfiguration.Active);

            return loggerConfiguration.Sink(new ApplicationInsightsSink(client, telemetryConverter), restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a Serilog sink that writes <see cref="LogEvent">log events</see> to Microsoft Application Insights 
        /// using a custom <see cref="ITelemetry"/> converter / constructor.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="telemetryClient">Required Application Insights telemetry client.</param>
        /// <param name="telemetryConverter">Required telemetry converter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration ApplicationInsights(
            this LoggerSinkConfiguration loggerConfiguration,
            TelemetryClient telemetryClient,
            ITelemetryConverter telemetryConverter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            return loggerConfiguration.Sink(new ApplicationInsightsSink(telemetryClient, telemetryConverter), restrictedToMinimumLevel);
        }


        /// <summary>
        /// Adds a Serilog sink that writes <see cref="LogEvent">log events</see> to Microsoft Application Insights 
        /// using a custom <see cref="ITelemetry"/> converter / constructor. Only use in rare cases when your application doesn't
        /// have already constructed AI telemetry configuration, which is extremely rare.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="instrumentationKey">Required Application Insights key.</param>
        /// <param name="telemetryConverter">Required telemetry converter.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <returns></returns>
        public static LoggerConfiguration ApplicationInsights(
            this LoggerSinkConfiguration loggerConfiguration,
            string instrumentationKey,
            ITelemetryConverter telemetryConverter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var client = new TelemetryClient();

            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                client.InstrumentationKey = instrumentationKey;
            }

            return loggerConfiguration.Sink(new ApplicationInsightsSink(client, telemetryConverter), restrictedToMinimumLevel);
        }
    }
}
