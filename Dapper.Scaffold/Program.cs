namespace Dapper.Scaffold;

using System.Threading.Tasks;
using CommandLine;
using Dapper.Scaffold.Domain;

public class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<ClassGenerationOptions>(args)
            .WithParsedAsync(
                async o =>
                {
                    var classGenerator = new ClassGenerator(o);
                    await classGenerator.GenerateAsync();
                });
    }
}
