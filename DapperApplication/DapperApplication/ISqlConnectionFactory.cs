using System.Data;

namespace DapperApplication;

public interface ISqlConnectionFactory
{
    Task<IDbConnection> OpenConnection(CancellationToken cancellationToken);
}