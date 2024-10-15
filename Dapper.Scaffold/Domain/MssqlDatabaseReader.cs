using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Scaffold.Domain;

public class MssqlDatabaseReader : IDatabaseReader
{
    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    public string GetCSharpType(string dbType)
    {
        switch (dbType.ToLower())
        {
            case "bigint":
            case "timestamp":
                return "long";
            case "binary":
            case "image":
            case "varbinary":
                return "byte[]";
            case "bit":
                return "bool";
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "text":
            case "varchar":
                return "string";
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                return "DateTime";
            case "datetimeoffset":
                return "DateTimeOffset";
            case "decimal":
            case "money":
            case "numeric":
            case "smallmoney":
                return "decimal";
            case "float":
                return "double";
            case "int":
                return "int";
            case "real":
                return "float";
            case "smallint":
                return "short";
            case "time":
                return "TimeSpan";
            case "tinyint":
                return "byte";
            case "uniqueidentifier":
                return "Guid";
        }

        return $"UNKNOWN_{dbType}";
    }

    public async Task<List<TableColumn>> GetTableColumnsAsync(IDbConnection connection, string tableName)
    {
        var tableColumnQuery = """
            SELECT col.[name] ColumnName
                ,typ.[name] ColumnType
                ,col.is_nullable IsNullable
                ,col.is_identity IsIdentity
                ,CASE WHEN col.default_object_id = 0 THEN 0 ELSE 1 END HasDefault
            FROM sys.columns col
            JOIN sys.types typ ON col.system_type_id = typ.system_type_id
                AND col.user_type_id = typ.user_type_id
            WHERE object_id = object_id(@tableName)
            ORDER BY column_id
            """;

        var tableColumns = (await connection.QueryAsync<TableColumn>(tableColumnQuery, new { tableName, })).ToList();

        return tableColumns;
    }

    public async Task<List<string>> GetTableNamesAsync(IDbConnection connection)
    {
        var tableQuery = """
            SELECT TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
                AND TABLE_SCHEMA = 'dbo'
            ORDER BY TABLE_NAME
            """;

        var tableNames = (await connection.QueryAsync<string>(tableQuery)).ToList();

        return tableNames;
    }
}
