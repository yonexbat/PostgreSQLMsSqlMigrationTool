namespace CopyTableData;

public class MigrationItem
{
    public string SourceTableName { get; set; } = string.Empty;

    public string DestinationTableName { get; set; } = string.Empty;

    public IList<ColMapping> ColMappings { get; set; } = new List<ColMapping>();
}