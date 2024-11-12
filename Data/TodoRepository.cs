using Dapper;
using System.Data;

namespace CSharpTodoWithDapper.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public TodoRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
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
            using (IDbConnection conn = _dbConnectionFactory.Connect())
            {
                var comm = "SELECT * FROM Todo";
                var queryResult = await conn.QueryAsync<Todo>(comm);

                return queryResult.ToList(); 
            }
        }
    }
}
