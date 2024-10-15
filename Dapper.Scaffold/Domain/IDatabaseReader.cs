using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Scaffold.Domain;

public interface IDatabaseReader
{
    IDbConnection CreateConnection(string connectionString);
    string GetCSharpType(string dbType);
    Task<List<TableColumn>> GetTableColumnsAsync(IDbConnection connection, string tableName);
    Task<List<string>> GetTableNamesAsync(IDbConnection connection);
}
