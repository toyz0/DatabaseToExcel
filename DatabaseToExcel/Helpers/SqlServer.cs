using Microsoft.Data.SqlClient;
using System.Data;


namespace Helpers
{
    public class SqlServer
    {
        private readonly string _conn;
        public SqlServer(string conn)
        {
            _conn = conn;
        }

        public async Task<DataTable> QueryAsync(string sqlCommand, CommandType commandType, params SqlParameter[] parameters)
        {
            DataTable dt = new();
            using SqlConnection conn = new(_conn);
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using SqlCommand cmd = new(sqlCommand, conn);
            cmd.CommandType = commandType;
            if (parameters.Length > 0)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
            }

            using SqlDataAdapter adapter = new(cmd);
            adapter.Fill(dt);

            if (conn.State == ConnectionState.Open)
            {
                await conn.CloseAsync();
                await conn.DisposeAsync();
            }

            return dt;
        }
    }
}
