using System.Data;
using System.Data.SQLite;

namespace CSharpTodoWithDapper.Data
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connect()
        {
            return new SQLiteConnection(_connectionString);
        }
    }

}
