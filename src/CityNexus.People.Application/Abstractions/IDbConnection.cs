using System.Data;

namespace CityNexus.People.Application.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
