using System.Data;
using Dapper;

namespace CityNexus.People.Infra.Database.Dapper;

internal sealed class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
    }

    public override DateTime Parse(object value)
    {
        return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
    }
}
