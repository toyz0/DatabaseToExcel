using Helpers;
using System.Data;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Start...");

        //Read SQL Command from file.
        string sqlCommand = await ReadSqlCommandAsync();
        
        //Query database to DataTable
        DataTable dt = await GetAsync(sqlCommand);
        
        //Export to Excel file.
        await ExportExcelAsync(dt);
        
        Console.WriteLine("Export Completed");
    }

    private static async Task<DataTable> GetAsync(string sqlCommand)
    {
        Helpers.Oracle db = new("<YOUR CONNECTION STRING>");
        
        //Able to use SQL Server
        //Helpers.SqlServer db = new("<YOUR CONNECTION STRING>");

        DataTable dt = await db.QueryAsync(sqlCommand, CommandType.Text);

        return dt;
    }

    private static async Task ExportExcelAsync(DataTable dt)
    {
        Export export = new();
        var excelBytes = await export.ExcelAsync(dt);

        if (!Directory.Exists("Export"))
            Directory.CreateDirectory("Export");

        //Change <YOUR_FILE_NAME> to your file name.
        string filePath = Path.Combine("Export", "<YOUR_FILE_NAME>.xlsx");
        await File.WriteAllBytesAsync(filePath, excelBytes);
    }

    private static async Task<string> ReadSqlCommandAsync()
    {
        string file = "Query.sql";
        string[] lines = await File.ReadAllLinesAsync(file);
        StringBuilder sql = new();
        foreach (var line in lines)
        {
            sql.AppendLine(line);
        }
        return sql.ToString();
    }
}