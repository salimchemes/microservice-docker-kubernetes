using VillaSport.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace VillaSport.ApplicationInsights
{ 
    public static class TelemetryConverter
    {
        public static ITelemetryConverter Traces => new TraceTelemetryConverter();

        public static ITelemetryConverter Events => new EventTelemetryConverter();
    }
}
