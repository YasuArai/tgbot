using System.Data.SqlClient;

namespace Db_create
{
	class Db_create
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand();
                command.CommandText = "CREATE DATABASE memory";
                command.Connection = connection;
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("База данных создана");
            }
            string connectionString2 = "Server=(localdb)\\mssqllocaldb;Database=memory;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString2))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand();
                command.CommandText = "CREATE TABLE creply (Id INT PRIMARY KEY IDENTITY, chatId BIGINT NOT NULL, q NVARCHAR(100) NOT NULL, re NVARCHAR(100) NOT NULL)";
                command.Connection = connection;
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("Таблица creply создана");
            }
        }
    }
}