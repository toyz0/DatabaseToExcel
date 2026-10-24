using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace Helpers
{
    public static class Database
    {
        //Works with any ADO.NET provider (SqlConnection, OracleConnection, ...). The caller owns and disposes the connection.
        public static async Task<DataTable> QueryAsync(DbConnection conn, string sqlCommand, CommandType commandType, params DbParameter[] parameters)
        {
            await conn.OpenAsync();

            await using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlCommand;
            cmd.CommandType = commandType;
            //Exports are often long-running; SqlCommand would otherwise time out after 30 seconds.
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddRange(parameters);

            await using DbDataReader reader = await cmd.ExecuteReaderAsync();

            //Oracle NUMBER can hold more digits than .NET decimal; truncate instead of throwing InvalidCastException.
            if (reader is OracleDataReader oracleReader)
                oracleReader.SuppressGetDecimalInvalidCastException = true;

            DataTable dt = new();
            dt.Load(reader);
            return dt;
        }
    }
}
