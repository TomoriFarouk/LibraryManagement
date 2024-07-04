using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryManagement.Infrastructure.Data
{
    public class DbConnector
    {
        private readonly IConfiguration _configuration;

        public DbConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string _connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqliteConnection(_connectionString);
        }
    }
}

