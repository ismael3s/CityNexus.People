using System.Data;
using CityNexus.People.Application.Abstractions;
using Npgsql;

namespace CityNexus.People.Infra.Database.Dapper;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
