using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace SmartGestionApp.Data
{
    public static class DatabaseManager
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartGestión.db");

        public static SqliteConnection GetConnection()
        {
            var connectionString = $"Data Source={dbPath}";
            return new SqliteConnection(connectionString);
        }
    }
}
