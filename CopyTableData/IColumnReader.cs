namespace CopyTableData;

public interface IColumnReader
{
    IList<string> GetColumnNames(string tableName);
}