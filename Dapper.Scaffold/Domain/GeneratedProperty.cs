using Dapper.Scaffold.Domain.Enums;

namespace Dapper.Scaffold.Domain;

public class GeneratedProperty : GeneratedObject
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Access Access { get; set; }
    public bool Readonly { get; set; }
    public bool HasDefault { get; set; }

    public override string ToString()
    {
        _output = new();

        AppendLine($"{AccessText(Access)} {Type} {Name} {{ get;{(Readonly ? string.Empty : " set;")} }}{(Type == "string" ? " = string.Empty;" : string.Empty)}");

        return _output.ToString();
    }
}
