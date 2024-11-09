using CityNexus.People.Domain.Abstractions;

namespace CityNexus.People.Domain.People.Events;

public sealed record NotificationVariables(string Name, string Value);

public record RegisteredPersonDomainEvent(Guid Id, List<NotificationVariables> Variables)
    : IDomainEvent;
