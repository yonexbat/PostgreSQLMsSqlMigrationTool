namespace CopyTableData;

public interface IColumnReader
{
    IList<DataBaseCol> GetDataBaseCols(string tableName);
}