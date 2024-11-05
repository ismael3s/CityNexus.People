namespace CityNexus.People.Infra.Configuration;

public class TelemetryConfigurationOption
{
    public string ServiceName { get; set; } = "CityNexus.People.Api";
    public string ServiceVersion { get; set; } = "0.0.1";
    public string ExporterUrl { get; set; } = "http://localhost:5341/ingest/otlp/v1/logs";
}
