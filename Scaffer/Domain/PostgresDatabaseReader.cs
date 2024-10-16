using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Scaffer.Domain;

public class PostgresDatabaseReader : IDatabaseReader
{
    public IDbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }

    public string GetSelectAllQuery(string tableName)
    {
        return $"SELECT * FROM \\\"{tableName}\\\";";
    }

    public string GetSelectWhereQuery(string tableName)
    {
        return $"SELECT * FROM \\\"{tableName}\\\" WHERE {{whereClause}};";
    }

    public string GetSelectByIdQuery(string tableName, string identityColumnName)
    {
        return $"SELECT * FROM \\\"{tableName}\\\" WHERE \\\"{identityColumnName}\\\" = @{identityColumnName};";
    }

    public string GetUpdateQuery(string tableName, string identityColumnName, IEnumerable<string> writeColumns)
    {
        var setColumns = string.Join(", ", writeColumns.Select(columnName => $"\\\"{columnName}\\\" = @{columnName}"));
        return $"UPDATE \\\"{tableName}\\\" SET {setColumns} WHERE \\\"{identityColumnName}\\\" = @{identityColumnName};";
    }

    public string GetDeleteQuery(string tableName, string identityColumnName)
    {
        return $"DELETE FROM \\\"{tableName}\\\" WHERE \\\"{identityColumnName}\\\" = @{identityColumnName};";
    }

    public string GetBasicInsertQuery(string tableName, IEnumerable<string> writeColumns, IEnumerable<string> readColumns)
    {
        var outputString = readColumns.Any() ? $" RETURNING {string.Join(", ", readColumns.Select(columnName => $"\\\"{columnName}\\\""))}" : string.Empty;
        var insertColumns = string.Join(", ", writeColumns.Select(columnName => $"\\\"{columnName}\\\""));
        var valueParameters = string.Join(", ", writeColumns.Select(columnName => $"@{columnName}"));

        return $"INSERT INTO \\\"{tableName}\\\" ({insertColumns}) VALUES ({valueParameters}){outputString};";
    }

    public string GetAdvancedInsertQueryOutputText()
    {
        return " RETURNING {string.Join(\", \", outputColumns.Select(columnName => $\"\\\"{columnName}\\\"\"))}";
    }

    public string GetAdvancedInsertQuery(string tableName)
    {
        return
            $"INSERT INTO \\\"{tableName}\\\" ({{string.Join(\", \", insertColumns.Select(columnName => $\"\\\"{{columnName}}\\\"\"))}}) VALUES ({{string.Join(\", \", insertColumns.Select(columnName => $\"@{{columnName}}\"))}}){{outputText}};";
    }

    public string GetCSharpType(string dbType)
    {
        switch (dbType.ToLower())
        {
            case "bigint":
            case "int8":
            case "serial8":
                return "long";
            case "bytea":
                return "byte[]";
            case "boolean":
            case "bool":
                return "bool";
            case "char":
            case "character":
            case "character varying":
            case "varchar":
            case "text":
                return "string";
            case "date":
            case "timestamp":
            case "timestamp without time zone":
            case "timestamp with time zone":
                return "DateTime";
            case "timestamptz":
                return "DateTimeOffset";
            case "numeric":
            case "decimal":
            case "money":
                return "decimal";
            case "double precision":
            case "float8":
                return "double";
            case "integer":
            case "int":
            case "int4":
            case "serial":
            case "serial4":
                return "int";
            case "real":
            case "float4":
                return "float";
            case "smallint":
            case "int2":
            case "smallserial":
            case "serial2":
                return "short";
            case "time":
            case "time without time zone":
            case "time with time zone":
                return "TimeSpan";
            case "uuid":
                return "Guid";
            case "xml":
                return "XDocument";
        }

        return $"UNKNOWN_{dbType}";
    }

    public async Task<List<TableColumn>> GetTableColumnsAsync(IDbConnection connection, string tableName)
    {
        var tableColumnQuery = """
            SELECT 
                col.column_name AS "ColumnName",
                col.data_type AS "ColumnType",
                (CASE WHEN col.is_nullable = 'YES' THEN true ELSE false END) AS "IsNullable",
                (CASE WHEN pg_get_serial_sequence(format('%I.%I', col.table_schema, col.table_name), col.column_name) IS NOT NULL THEN TRUE ELSE FALSE END) AS "IsIdentity",
                (CASE WHEN col.column_default IS NOT NULL THEN TRUE ELSE FALSE END) AS "HasDefault"
            FROM 
            	information_schema.columns col
            WHERE 
            	col.table_name = @tableName
            ORDER BY 
            	col.ordinal_position
            """;

        var tableColumns = (await connection.QueryAsync<TableColumn>(tableColumnQuery, new { tableName, })).ToList();

        return tableColumns;
    }

    public async Task<List<string>> GetTableNamesAsync(IDbConnection connection)
    {
        var tableQuery = """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_type = 'BASE TABLE'
                AND table_schema = 'public'
            ORDER BY table_name
            """;

        var tableNames = (await connection.QueryAsync<string>(tableQuery)).ToList();

        return tableNames;
    }
}
