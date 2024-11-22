using CSharpTodoWithDapper.Data.Models;
using CSharpTodoWithDapper.Models;
using Dapper;

namespace CSharpTodoWithDapper.Data
{
    public interface ITodoRepository
    {
        Task<TodoItem> AddTodoAsync(CreateTodoRequest todo);
        Task<List<TodoItem>> GetTodoByIdAsync(int id);
        Task<List<TodoItem>> GetAllTodosAsync();
        Task<List<TodoItem>> GetMatchingTodosAsync(string query);
        Task<TodoItem> UpdateTodoAsync(UpdateTodoRequest todo, int id);
        Task DeleteTodoAsync(int id);
    }
    public class TodoRepository : ITodoRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        public TodoRepository(DbConnectionFactory dbConnectionFactory, ILogger<TodoRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<TodoItem> AddTodoAsync(CreateTodoRequest todo)
        {
            var insertQuery = "INSERT INTO Todo (Description, Completed) VALUES (@Description, @Completed); SELECT last_insert_rowid();";
            var getTodoItemQuery = "SELECT * FROM Todo WHERE Id = @Id";
            
            using (var conn = _dbConnectionFactory.Connect())
            {
                var id = await conn.ExecuteScalarAsync<int>(insertQuery, new { Description = todo.Description, Completed = 0} );

                var getResult = await conn.QueryAsync<TodoItem>(getTodoItemQuery, new { Id = id });
                var createdTodo = getResult.SingleOrDefault();

                if (createdTodo == null)
                {
                    throw new InvalidOperationException(
                        "Something went wrong. Unable to retrieve the created TodoItem");
                }
                
                return createdTodo;  
            }
        }

        public async Task<List<TodoItem>> GetTodoByIdAsync(int id)
        {
            var comm = "SELECT * FROM Todo WHERE Id = @Id";
            using (var conn = _dbConnectionFactory.Connect())
            {
                var queryResult = await conn.QueryAsync<TodoItem>(comm, new { Id = id });
                return queryResult.ToList();
            }
        }

        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            var comm = "SELECT * FROM Todo";
            using (var conn = _dbConnectionFactory.Connect())
            {
                var queryResult = await conn.QueryAsync<TodoItem>(comm);
                return queryResult.ToList();
            }
        }

        public async Task<List<TodoItem>> GetMatchingTodosAsync(string query)
        {
            var comm = "SELECT * FROM Todo WHERE Description LIKE @Query;";
            using (var conn = _dbConnectionFactory.Connect())
            {
                var queryResult = await conn.QueryAsync<TodoItem>(comm, new { Query = $"%{query}%" });
                return queryResult.ToList();
            }
        }

        public async Task<TodoItem> UpdateTodoAsync(UpdateTodoRequest todo, int id)
        {
            var updateQuery = "UPDATE Todo SET Description = @Description, Completed = @Completed WHERE Id = @Id;";
            var selectQuery = "SELECT * FROM Todo WHERE Id = @Id;";
            using (var conn = _dbConnectionFactory.Connect())
            {
                await conn.ExecuteScalarAsync(updateQuery, new { todo.Description, todo.Completed, Id = id });
                var result = await conn.QuerySingleAsync<TodoItem>(selectQuery, new { Id = id});
                return result;
            }
        }

        public async Task DeleteTodoAsync(int id)
        {
            var deleteQuery = "DELETE FROM Todo WHERE Id = @Id;";
            using (var conn = _dbConnectionFactory.Connect())
            {
                await conn.ExecuteAsync(deleteQuery, new { Id = id });
            }
        }
    }
}
