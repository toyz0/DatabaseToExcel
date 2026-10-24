using Helpers;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

public class Program
{
    //Nested inside Program so it does not clash with System.Data.DbType.
    public enum DbType { SqlServer, Oracle }

    //Change these values to point at your database.
    //Do not commit a connection string that contains a password; this repository is public.
    private static readonly DbType _dbType = DbType.SqlServer;
    //Set the connection string
    private static readonly string _connectionString = "YOUR_CONNECTION_STRING";
    //Set the file name
    private static readonly string _filename = "YOUR_FILE_NAME";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Start...");

        //Read SQL Command from file.
        string sqlCommand = await ReadSqlCommandAsync();
        
        //Query database to DataTable
        DataTable dt = await GetAsync(sqlCommand);
        
        //Export to Excel file.
        await ExportExcelAsync(dt, false, null);
        
        Console.WriteLine("Export Completed");
    }

    private static async Task<DataTable> GetAsync(string sqlCommand)
    {
        //The connection is closed when this method returns; the DataTable keeps the data.
        await using DbConnection conn = _dbType == DbType.Oracle
            ? new OracleConnection(_connectionString)
            : new SqlConnection(_connectionString);

        return await Database.QueryAsync(conn, sqlCommand, CommandType.Text);
    }

    //isAutoFitColumns widens columns to fit their content; password, when set, encrypts the workbook.
    private static async Task ExportExcelAsync(DataTable dt, bool isAutoFitColumns = false, string? password = null)
    {
        //Next to the .exe, so the output lands in the same place whichever folder the program is started from.
        string exportDir = Path.Combine(AppContext.BaseDirectory, "Export");
        Directory.CreateDirectory(exportDir);

        //Timestamped so each run keeps the previous exports.
        string filePath = Path.Combine(exportDir, $"{_filename}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

        Export export = new();
        await export.ExcelAsync(dt, filePath, isAutoFitColumns, password);
    }

    private static async Task<string> ReadSqlCommandAsync()
    {
        //Query.sql is copied next to the .exe on build.
        string file = Path.Combine(AppContext.BaseDirectory, "Query.sql");
        string sql = await File.ReadAllTextAsync(file);

        //Oracle rejects a trailing ';' with ORA-00911.
        return sql.Trim().TrimEnd(';');
    }
}