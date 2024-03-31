using HoaHoeHoaSoi.Model;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Helpers
{
    public static class HoaDBContext
    {
        static IConfigurationRoot _config;
        static HoaDBContext()
        {
            _config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        }

        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
    }
}
