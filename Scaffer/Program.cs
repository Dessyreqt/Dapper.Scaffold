using System;
using System.Threading.Tasks;
using CommandLine;
using Scaffer.Domain;

namespace Scaffer;

public class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<ClassGenerationOptions>(args)
            .WithParsedAsync(
                async o =>
                {
                    IDatabaseReader databaseReader = o.Generator switch
                    {
                        "mssql" => new MssqlDatabaseReader(),
                        "postgres" => new PostgresDatabaseReader(),
                        _ => throw new ArgumentException("Invalid generator specified."),
                    };

                    var classGenerator = new ClassGenerator(o, databaseReader);
                    await classGenerator.GenerateAsync();
                });
    }
}
