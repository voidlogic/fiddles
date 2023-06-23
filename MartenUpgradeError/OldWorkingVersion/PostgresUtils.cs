using Npgsql;

namespace OldWorkingVersion
{
    public static class PostgresUtils
    {
        public static void ExecuteNpgsqlCommandFromAdminDatabase(string command, string connectionStringBase, string adminDatabaseName)
        {
            using (var conn = new NpgsqlConnection($"{connectionStringBase};Database={adminDatabaseName}"))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = command;
                    var result = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}
