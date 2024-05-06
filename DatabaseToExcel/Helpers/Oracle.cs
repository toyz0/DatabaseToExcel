using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Helpers
{
    public class Oracle
    {
        private readonly string _conn;
        public Oracle(string conn)
        {
            _conn = conn;
        }

        public async Task<DataTable> QueryAsync(string sqlCommand, CommandType commandType, params OracleParameter[] parameters)
        {
            DataTable dt = new();
            using OracleConnection conn = new(_conn);
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using OracleCommand cmd = new(sqlCommand, conn);
            cmd.CommandType = commandType;
            if (parameters.Length > 0)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
            }

            using OracleDataAdapter adapter = new(cmd);
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
