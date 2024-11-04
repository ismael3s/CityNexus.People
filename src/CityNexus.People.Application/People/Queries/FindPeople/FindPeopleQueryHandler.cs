using CityNexus.People.Application.Abstractions;
using Dapper;

namespace CityNexus.People.Application.People.Queries.FindPeople;

public sealed class FindPeopleQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
{
    public sealed record Input(int PageNumber = 1, int PageSize = 10);

    public sealed class Output
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = default!;

        public string Email { get; init; } = default!;

        public DateTime CreatedAt { get; init; } = default;
    }

    public async Task<Pagination<Output>> Handle(
        Input input,
        CancellationToken cancellationToken = default
    )
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        var skip = (input.PageNumber - 1) * input.PageSize;
        var take = input.PageSize > 50 ? 50 : input.PageSize;
        var query = $"""
            SELECT
                id, name, email, created_at 
            FROM person
            WHERE 1=1
            ORDER BY id ASC
            OFFSET {skip}
            LIMIT {take}
            """;
        var result = await connection.QueryAsync<Output>(query, cancellationToken);
        var count = await connection.ExecuteScalarAsync<int>(
            "SELECT count(*) from person",
            cancellationToken
        );
        return Pagination<Output>.Create(count, input.PageNumber, take, result.ToList());
    }
}
