using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace NewPointWebApi.Data
{
    public class SqlDataAccess(IConfiguration config) : ISqlDataAccess
    {
        private readonly IConfiguration _config = config;
        public string ConnectionString { get; set; } = "defaultConnection";

        public SqlConnection OpenConnection()
        {
            string connectionString = _config.GetConnectionString(ConnectionString);
            SqlConnection conn = new(connectionString);
            return conn;
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionString);
            using IDbConnection conn = new SqlConnection(connectionString);
            var data = await conn.QueryAsync<T>(sql, parameters);
            return data.ToList();
        }

        public async Task SaveData<T>(string sql, T parameters)
        {
            try
            {
                string connectionString = _config.GetConnectionString(ConnectionString);
                using IDbConnection conn = new SqlConnection(connectionString);
                await conn.ExecuteAsync(sql, parameters);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error SQL Command:" + ex.Message);
                throw;
            }
        }

        public async Task SaveDataConn<T, U>(string sql, U parameters, string conn)
        {
            string connectionString = _config.GetConnectionString(conn);
            using IDbConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(sql, parameters);
        }

        public int TotalResult<U>(string sql, U paramters)
        {
            string connStr = _config.GetConnectionString(ConnectionString);
            using IDbConnection conn = new SqlConnection(connStr);
            var data = conn.ExecuteScalar<int>(sql, paramters);
            return data;

        }

    }
}
