using CityNexus.People.Domain.Abstractions;

namespace CityNexus.People.Domain.People.Events;

public sealed record RegisteredPersonDomainEvent(Guid Id) : IDomainEvent;
