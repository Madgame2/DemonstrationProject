using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using System.Configuration;
using System.IO;

namespace DemonstrationProject.DB
{
    public static class DatabaseInitializer
    {
        private static string masterConnectionString = ConfigurationManager.ConnectionStrings["MasterConnection"].ConnectionString;
        private static string targetDbConnectionString = ConfigurationManager.ConnectionStrings["DemonstrationDB"].ConnectionString;
        private static string targetDBName = "DemonstrationDB";


        public static void Initialize()
        {
            if (!CheckDatabaseExists())
            {
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "Scripts", "CreateDatabaseAndTables.sql");
                CreateDatabaseAndTablesFromFile(masterConnectionString, scriptPath);
            }
        }


        private static bool CheckDatabaseExists()
        {
            using var connection = new SqlConnection(masterConnectionString);
            connection.Open();

            string query = $"SELECT db_id('{targetDBName}')";
            using var command  = new SqlCommand(query, connection);

            object result = command.ExecuteScalar();
            return result !=DBNull.Value && result!=null;
        }


        public static void CreateDatabaseAndTablesFromFile(string masterConnectionString, string scriptFilePath)
        {
            string script = File.ReadAllText(scriptFilePath);

            string[] commands = script.Split(new string[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO\n", "\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            using var connection = new SqlConnection(masterConnectionString);
            connection.Open();


            foreach (var commandText in commands)
            {
                using var command = new SqlCommand(commandText, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
