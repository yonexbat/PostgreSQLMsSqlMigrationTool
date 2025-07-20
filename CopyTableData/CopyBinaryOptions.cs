namespace CopyTableData;

public class CopyBinaryOptions
{
    public string DestinationTable { get; set; } = string.Empty;
    public string DestinationIdColumn { get; set; } = string.Empty;
    public string DestinationBinaryColumn { get; set; } = string.Empty;
    public string SourceTable { get; set; } = string.Empty;
    public string SourceIdColumn { get; set; } = string.Empty;
    public string SourceBinaryColumn { get; set; } = string.Empty;
}