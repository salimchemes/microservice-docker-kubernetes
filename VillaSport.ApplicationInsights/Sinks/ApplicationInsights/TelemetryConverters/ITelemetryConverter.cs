using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using System;
using System.Collections.Generic;
namespace VillaSport.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters
{
    public interface ITelemetryConverter
    {
        IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider);
    }
}
