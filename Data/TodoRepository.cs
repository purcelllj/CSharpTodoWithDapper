using Dapper;
using System.Data;

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
            using (IDbConnection conn = _dbConnectionFactory.Connect())
            {
                var completed = todo.Completed ? 1 : 0;
                var comm = $"INSERT INTO Todo(Description, Completed) VALUES ('{todo.Description}', {completed})";
                await conn.ExecuteAsync(comm);
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
