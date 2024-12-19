using System.Data;

namespace SncSoftware.Extensions.Dapper.Factories;

public interface ISqlConnectionFactory
{
    Task<IDbConnection> OpenConnection(CancellationToken cancellationToken);
}