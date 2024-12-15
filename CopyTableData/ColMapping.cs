namespace CopyTableData;

public class ColMapping
{
    public string SourceColName { get; set; } = null!;

    public string DestinationColName { get; set; } = null!;
    
    public string? SourceColType { get; set; }
    
    public string? DestinationColType { get; set; }
}