using System.Data;

namespace DapperApplication;

public interface ISqlConnectionFactory
{
    IDbConnection GetConnection();
}