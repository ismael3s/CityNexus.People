using CityNexus.People.Application.Extensions;
using CityNexus.People.Application.People.Commands.RegisterPerson;
using CityNexus.People.Application.People.Queries.FindPeople;
using CityNexus.People.Infra.Database.EF;
using CityNexus.People.Infra.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging(_ => { });
builder.Services.AddSwaggerGen().AddInfra(configuration).AddApplication();

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
