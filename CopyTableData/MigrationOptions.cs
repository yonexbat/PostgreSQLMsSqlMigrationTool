namespace CopyTableData;

public class MigrationOptions
{
    public const string Migration = "Migration";

    public string Name { get; set; } = string.Empty;

    public string SourceDbTech { get; set; } = string.Empty;

    public string DestinationDbTech { get; set; } = string.Empty;

    public IList<string> PreScripts { get; set; } = new List<string>();

    public IList<string> PostScripts { get; set; } = new List<string>();

    public IList<MigrationItem> MigrationItems { get; set; } = new List<MigrationItem>();
}