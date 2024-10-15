﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Scaffold.Domain.Enums;

//using System.Data.SqlClient;

namespace Dapper.Scaffold.Domain;

internal class ClassGenerator
{
    private readonly ClassGenerationOptions _options;
    private readonly IDatabaseReader _databaseReader;
    private GeneratedFile _generatedFile;

    public ClassGenerator(ClassGenerationOptions options, IDatabaseReader databaseReader)
    {
        _options = options;
        _databaseReader = databaseReader;
    }

    public async Task GenerateAsync()
    {
        var connectionString = _options.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new("Please specify a connection string using the -c parameter or by setting it in the appsettings.json");
        }

        using var connection = _databaseReader.CreateConnection(connectionString);

        var tableName = _options.TableName;
        var path = _options.Path ?? Directory.GetCurrentDirectory();
        var ns = _options.Namespace ?? "Project";
        var force = _options.Force;
        var generatedFiles = new List<GeneratedFile>();

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Creating directory \"{path}\"...");
            Directory.CreateDirectory(path);
        }

        if (_options.Delete)
        {
            // delete all code files in directory
            var files = Directory.GetFiles(path, "*.cs");

            foreach (var file in files)
            {
                Console.WriteLine($"Deleting file \"{file}\"...");
                File.Delete(file);
            }
        }

        if (tableName != null)
        {
            await WriteFileAsync(connection, path, tableName, force, ns);

            generatedFiles.Add(_generatedFile);
        }
        else
        {
            var tableNames = await _databaseReader.GetTableNamesAsync(connection);

            foreach (var table in tableNames)
            {
                await WriteFileAsync(connection, path, table, force, ns);

                generatedFiles.Add(_generatedFile);
            }
        }

        if (_options.Extensions)
        {
            await WriteExtensionsFileAsync(path, generatedFiles, force, connection.Database, ns);
        }
    }

    private async Task WriteExtensionsFileAsync(string path, List<GeneratedFile> generatedFiles, bool force, string databaseName, string ns)
    {
        var className = GetClassName(databaseName);
        var filename = Path.Combine(path, $"{className}ConnectionExtensions.g.cs");
        var firstClass = true;

        Console.WriteLine($"Writing file \"{filename}\"...");

        if (!force && File.Exists(filename))
        {
            throw new($"File \"{filename}\" already exists! Use -f to overwrite.");
        }

        var extensionsText = new StringBuilder();

        extensionsText.Append(GeneratedFile.GetAutoGeneratedComment());

        extensionsText.AppendLine("using System.Collections.Generic;");
        extensionsText.AppendLine("using System.Data;");
        extensionsText.AppendLine("using System.Linq;");
        extensionsText.AppendLine("using System.Threading.Tasks;");
        extensionsText.AppendLine("using Dapper;");
        extensionsText.AppendLine();
        extensionsText.AppendLine($"namespace {ns};");
        extensionsText.AppendLine();
        extensionsText.AppendLine($"public static class {className}ConnectionExtensions");
        extensionsText.AppendLine("{");

        foreach (var file in generatedFiles)
        {
            if (!firstClass)
            {
                extensionsText.AppendLine();
            }
            else
            {
                firstClass = false;
            }

            AppendClassExtensions(extensionsText, file);
        }

        extensionsText.AppendLine("}");

        await using var writer = new StreamWriter(filename, false);

        await writer.WriteAsync(extensionsText.ToString());

        Console.WriteLine($"File \"{filename}\" written.");
    }

    private void AppendClassExtensions(StringBuilder extensionsText, GeneratedFile file)
    {
        extensionsText.Append(file.GetListMethod());
        extensionsText.AppendLine();
        extensionsText.Append(file.GetInsertMethod());

        var hasIdMethod = !string.IsNullOrWhiteSpace(file.GetGetByIdMethod());

        if (hasIdMethod)
        {
            extensionsText.AppendLine();
            extensionsText.Append(file.GetGetByIdMethod());
            extensionsText.AppendLine();
            extensionsText.Append(file.GetSaveMethod());
            extensionsText.AppendLine();
            extensionsText.Append(file.GetUpdateMethod());
            extensionsText.AppendLine();
            extensionsText.Append(file.GetDeleteMethod());
        }
    }

    private async Task WriteFileAsync(IDbConnection connection, string path, string tableName, bool force, string ns)
    {
        var className = GetClassName(tableName);
        var filename = Path.Combine(path, $"{className}.g.cs");

        Console.WriteLine($"Writing file \"{filename}\"...");

        if (!force && File.Exists(filename))
        {
            throw new($"File \"{filename}\" already exists! Use -f to overwrite.");
        }

        await GenerateFileAsync(connection, ns, tableName);

        await using var writer = new StreamWriter(filename, false);

        await writer.WriteAsync(_generatedFile.ToString());

        Console.WriteLine($"File \"{filename}\" written.");
    }

    private async Task GenerateFileAsync(IDbConnection connection, string ns, string tableName)
    {
        _generatedFile = new(_databaseReader) { FileNamespace = ns, };

        var className = GetClassName(tableName);

        var tableColumns = await _databaseReader.GetTableColumnsAsync(connection, tableName);

        var mainClass = new GeneratedClass { Access = Access.Public, Name = className, TableName = tableName, };
        _generatedFile.AddClass(mainClass);

        mainClass.IdentityColumn = tableColumns.FirstOrDefault(x => x.IsIdentity)?.ColumnName;

        var generatedProperties = GetGeneratedProperties(tableColumns);

        foreach (var property in generatedProperties)
        {
            mainClass.AddProperty(property);
        }
    }

    private string GetClassName(string tableName)
    {
        var className = new StringBuilder();
        var capitalizeNext = true;

        foreach (var tableNameChar in tableName)
        {
            if (char.IsLetterOrDigit(tableNameChar))
            {
                if (capitalizeNext)
                {
                    className.Append(char.ToUpper(tableNameChar));
                    capitalizeNext = false;
                }
                else
                {
                    className.Append(tableNameChar);
                }
            }
            else
            {
                capitalizeNext = true;
            }
        }

        return className.ToString();
    }

    private List<GeneratedProperty> GetGeneratedProperties(List<TableColumn> tableColumns)
    {
        var generatedProperties = new List<GeneratedProperty>();

        foreach (var column in tableColumns)
        {
            var generatedProperty = new GeneratedProperty
            {
                Name = column.ColumnName, Access = Access.Public, Readonly = false, Type = GetCSharpType(column), HasDefault = column.HasDefault,
            };

            generatedProperties.Add(generatedProperty);
        }

        return generatedProperties;
    }

    private string GetCSharpType(TableColumn column)
    {
        var type = _databaseReader.GetCSharpType(column.ColumnType);
        _generatedFile.AddNamespaceForType(type);

        if (column.IsNullable)
        {
            type += "?";
        }

        return type;
    }
}
