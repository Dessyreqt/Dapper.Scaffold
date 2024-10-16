using CommandLine;

namespace Scaffer.Domain;

internal class ClassGenerationOptions
{
    [Option('g', "generator", HelpText = "The generator to use while generating (mssql, postgres). Defaults to mssql.", Default = "mssql")]
    public string Generator { get; set; } = string.Empty;

    [Option('t', "tableName", HelpText = "The table from which to create the class.")]
    public string? TableName { get; set; }

    [Option('c', "connectionString", HelpText = "The connection string to a SQL Server database. Required.", Required = true)]
    public string ConnectionString { get; set; } = string.Empty;

    [Option('p', "path", HelpText = "The path to which to save the class.")]
    public string? Path { get; set; }

    [Option('n', "namespace", HelpText = "The namespace in which the class will be created. Defaults to a name based on the database.")]
    public string? Namespace { get; set; }

    [Option('f', "force", HelpText = "Force the file to be created, overwriting an existing file if necessary.")]
    public bool Force { get; set; }

    [Option('e', "extensions", HelpText = "Create class extensions for CRUD operations.")]
    public bool Extensions { get; set; }

    [Option('d', "delete", HelpText = "Delete existing classes prior to generation")]
    public bool Delete { get; set; }
}
