namespace CopyTableData;

public interface IBinaryReaderWriter
{
    IEnumerable<BinaryReadItem> GetBinaries(string tableName, string? idColumn, string binaryColumn);

    void WriteBinary(string tableName, string? idColumn, string binaryColumn, BinaryReadItem item);
}