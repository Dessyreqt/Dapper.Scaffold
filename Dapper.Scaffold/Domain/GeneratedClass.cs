using System.Collections.Generic;
using Dapper.Scaffold.Domain.Enums;

namespace Dapper.Scaffold.Domain;

public class GeneratedClass : GeneratedObject
{
    public List<GeneratedProperty> Properties { get; } = new();
    public string Name { get; set; }
    public string TableName { get; set; }
    public Access Access { get; set; }
    public string IdentityColumn { get; set; }

    public void AddProperty(GeneratedProperty generatedProperty)
    {
        generatedProperty.BaseIndentation = BaseIndentation + 4;
        Properties.Add(generatedProperty);
    }

    public override string ToString()
    {
        _output = new();

        AppendLine($"{AccessText(Access)} class {Name}");
        AppendLine("{");
        _output.Append(string.Join(string.Empty, Properties));
        AppendLine("}");

        return _output.ToString();
    }
}
