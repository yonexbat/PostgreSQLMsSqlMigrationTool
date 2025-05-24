namespace CopyTableData.PostgreSql.Mappers;

public class DateTimeToDateMapper : IMapper
{
    public object? Convert(object? input)
    {
        if (input is not null && input is DateTime date)
        {
            return DateOnly.FromDateTime(date);
        }

        return input;
    }
}