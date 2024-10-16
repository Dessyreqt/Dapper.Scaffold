using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Scaffer.Domain;

public class MssqlDatabaseReader : IDatabaseReader
{
    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    public string GetSelectAllQuery(string tableName)
    {
        return $"SELECT * FROM [{tableName}];";
    }

    public string GetSelectWhereQuery(string tableName)
    {
        return $"SELECT * FROM [{tableName}] WHERE {{whereClause}};";
    }

    public string GetSelectByIdQuery(string tableName, string identityColumnName)
    {
        return $"SELECT * FROM [{tableName}] WHERE [{identityColumnName}] = @{identityColumnName};";
    }

    public string GetUpdateQuery(string tableName, string identityColumnName, IEnumerable<string> writeColumns)
    {
        var setColumns = string.Join(", ", writeColumns.Select(columnName => $"[{columnName}] = @{columnName}"));
        return $"UPDATE [{tableName}] SET {setColumns} WHERE [{identityColumnName}] = @{identityColumnName};";
    }

    public string GetDeleteQuery(string tableName, string identityColumnName)
    {
        return $"DELETE FROM [{tableName}] WHERE [{identityColumnName}] = @{identityColumnName};";
    }

    public string GetBasicInsertQuery(string tableName, IEnumerable<string> writeColumns, IEnumerable<string> readColumns)
    {
        var outputString = readColumns.Any() ? $"OUTPUT {string.Join(", ", readColumns.Select(columnName => $"INSERTED.[{columnName}]"))} " : string.Empty;
        var insertColumns = string.Join(", ", writeColumns.Select(columnName => $"[{columnName}]"));
        var valueParameters = string.Join(", ", writeColumns.Select(columnName => $"@{columnName}"));

        return $"INSERT INTO [{tableName}] ({insertColumns}) {outputString}VALUES ({valueParameters});";
    }

    public string GetAdvancedInsertQueryOutputText()
    {
        return "OUTPUT {string.Join(\", \", outputColumns.Select(columnName => $\"INSERTED.[{columnName}]\"))} ";
    }

    public string GetAdvancedInsertQuery(string tableName)
    {
        return
            $"INSERT INTO [{tableName}] ({{string.Join(\", \", insertColumns.Select(columnName => $\"[{{columnName}}]\"))}}) {{outputText}}VALUES ({{string.Join(\", \", insertColumns.Select(columnName => $\"@{{columnName}}\"))}});";
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
            case "xml":
                return "XDocument";
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
                AND TABLE_NAME NOT IN (
                    SELECT [name]
                    FROM [sys].[tables] tbl
                    WHERE is_ms_shipped = 1
                        OR (
                            SELECT COUNT(*)
                            FROM [sys].[extended_properties]
                            WHERE [major_id] = tbl.object_id
                                AND [minor_id] = 0
                                AND [class] = 1
                                AND [name] = 'microsoft_database_tools_support'
                        ) > 0
                )
            ORDER BY TABLE_NAME
            """;

        var tableNames = (await connection.QueryAsync<string>(tableQuery)).ToList();

        return tableNames;
    }
}
