using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Scaffer.Domain;

public interface IDatabaseReader
{
    IDbConnection CreateConnection(string connectionString);
    string GetSelectAllQuery(string tableName);
    string GetSelectWhereQuery(string tableName);
    string GetSelectByIdQuery(string tableName, string identityColumnName);
    string GetUpdateQuery(string tableName, string identityColumnName, IEnumerable<string> writeColumns);
    string GetDeleteQuery(string tableName, string identityColumnName);
    string GetBasicInsertQuery(string tableName, IEnumerable<string> writeColumns, IEnumerable<string> readColumns);
    string GetAdvancedInsertQueryOutputText();
    string GetAdvancedInsertQuery(string tableName);
    string GetCSharpType(string dbType);
    Task<List<TableColumn>> GetTableColumnsAsync(IDbConnection connection, string tableName);
    Task<List<string>> GetTableNamesAsync(IDbConnection connection);
}
