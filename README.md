# CityNexus.People

This project  is responsible for registering people into the nexus

## Technologies
- xUnit
- FluentAssertions
- TestContainers
- .NET 8
- Clean Architecture
- TDD
- MassTransit
- Outbox Pattern
- Entity Framework
- CSharpier
- Husky.NET

## Features

### Register a person into the nexus

To be able to register a person into the Nexus we need:
- FullName
- Cpf (Brazilian Document)
- Email

When the person is registered should:
1. Send a Welcome Email
2. Publish the event into the broker
