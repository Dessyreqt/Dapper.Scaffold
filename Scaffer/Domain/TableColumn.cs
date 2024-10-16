namespace Scaffer.Domain;

public class TableColumn
{
    private string _columnName = string.Empty;

    public string ColumnName
    {
        get => _columnName.Replace(' ', '_').Replace('/', '_');
        set => _columnName = value;
    }

    public string ColumnType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsIdentity { get; set; }
    public bool HasDefault { get; set; }
}
