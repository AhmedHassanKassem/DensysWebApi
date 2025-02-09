using Microsoft.Data.SqlClient;

namespace NewPointWebApi.Data
{
    public interface ISqlDataAccess
    {
        string ConnectionString { get; set; }

        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        SqlConnection OpenConnection();
        Task SaveData<T>(string sql, T parameters);
        Task SaveDataConn<T, U>(string sql, U parameters, string conn);
        int TotalResult<U>(string sql, U paramters);
    }
}