using CityNexus.People.Application.Extensions;
using CityNexus.People.Application.People.Commands.RegisterPerson;
using CityNexus.People.Application.People.Queries.FindPeople;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddSwaggerGen().AddInfra(configuration).AddApplication();
builder.Logging.AddOpenTelemetry(o =>
{
    o.IncludeScopes = true;
    o.IncludeFormattedMessage = true;
    o.SetResourceBuilder(
            ResourceBuilder
                .CreateDefault()
                .AddService(
                    configuration.GetValue<string>("Telemetry:ServiceName", "CityNexus.People.Api"),
                    serviceVersion: configuration.GetValue<string>(
                        "Telemetry:ServiceVersion",
                        "0.0.1"
                    )
                )
        )
        .AddConsoleExporter()
        .AddOtlpExporter(exporter =>
        {
            exporter.Endpoint = new Uri(
                configuration.GetValue<string>(
                    "Telemetry:ExporterUrl",
                    "http://localhost:5341/ingest/otlp/v1/logs"
                )
            );
            exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
app.UseHttpsRedirection();

app.MapGet(
    "/people",
    async (FindPeopleQueryHandler queryHandler, CancellationToken cancellationToken) =>
    {
        var result = await queryHandler.Handle(new(), cancellationToken);
        return Results.Ok(result);
    }
);

app.MapPost(
    "/people",
    async (
        RegisterPersonCommand.Input input,
        RegisterPersonCommand command,
        CancellationToken cancellationToken
    ) =>
    {
        await command.Handle(input, cancellationToken);
        return Results.NoContent();
    }
);

app.Run();
