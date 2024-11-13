using Dapper;
using System.Data;
using System.Data.SQLite;

namespace CSharpTodoWithDapper.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<TodoRepository> _logger;

        public TodoRepository(DbConnectionFactory dbConnectionFactory, ILogger<TodoRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task AddTodoAsync(Todo todo)
        {
            try
            {
                var insertQuery = "INSERT INTO Todo(Description, Completed) VALUES (@Description, @Completed)";
                using (IDbConnection conn = _dbConnectionFactory.Connect())
                {
                    await conn.ExecuteAsync(insertQuery, todo);
                }
            }
            catch (SQLiteException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
       }

        public async Task<List<Todo>> GetAllTodosAsync()
        {
            try
            {
                using (IDbConnection conn = _dbConnectionFactory.Connect())
                {
                    var comm = "SELECT * FROM Todo";
                    var queryResult = await conn.QueryAsync<Todo>(comm);

                    return queryResult.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
