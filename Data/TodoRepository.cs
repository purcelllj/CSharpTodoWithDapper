using CSharpTodoWithDapper.Models;
using Dapper;
using System.Data;
using System.Data.SQLite;

namespace CSharpTodoWithDapper.Data
{
    public interface ITodoRepository
    {
        Task<Todo> AddTodoAsync(Todo todo);
        Task<List<Todo>> GetTodoByIdAsync(int id);
        Task<List<Todo>> GetAllTodosAsync();
        Task<List<Todo>> GetMatchingTodosAsync(string query);
        Task<Todo> UpdateTodoAsync(Todo todo, int id);
        Task DeleteTodoAsync(int id);
    }
    public class TodoRepository : ITodoRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<TodoRepository> _logger;

        public TodoRepository(DbConnectionFactory dbConnectionFactory, ILogger<TodoRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            var insertQuery = "INSERT INTO Todo(Description, Completed) VALUES (@Description, @Completed); SELECT last_insert_rowid();";
            var getTodoQuery = "SELECT * FROM Todo WHERE Id = @Id";
            try
            {
                using (IDbConnection conn = _dbConnectionFactory.Connect())
                {
                    var id = await conn.ExecuteScalarAsync<int>(insertQuery, todo);

                    var getResult = await conn.QueryAsync<Todo>(getTodoQuery, new { Id = id });
                    var createdTodo = getResult.SingleOrDefault(x => x.Id == id);

                    if (createdTodo == null)
                    {
                        throw new InvalidOperationException(
                            "Something went wrong. Unable to retrieve the created todo");
                    }
                    
                    return createdTodo;  
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException(
                    $"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }

        public async Task<List<Todo>> GetTodoByIdAsync(int id)
        {
            try
            {
                var comm = "SELECT * FROM Todo WHERE Id = @Id";
                using (var conn = _dbConnectionFactory.Connect())
                {
                    var queryResult = await conn.QueryAsync<Todo>(comm, new { Id = id });
                    return queryResult.ToList();
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException($"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }

        public async Task<List<Todo>> GetAllTodosAsync()
        {
            try
            {
                var comm = "SELECT * FROM Todo";
                using (var conn = _dbConnectionFactory.Connect())
                {
                    var queryResult = await conn.QueryAsync<Todo>(comm);
                    return queryResult.ToList();
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException(
                    $"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }

        public async Task<List<Todo>> GetMatchingTodosAsync(string query)
        {
            try
            {
                var comm = "SELECT * FROM Todo WHERE Description LIKE @Query;";
                using (var conn = _dbConnectionFactory.Connect())
                {
                    var queryResult = await conn.QueryAsync<Todo>(comm, new { Query = $"%{query}%" });
                    return queryResult.ToList();
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException(
                    $"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo, int id)
        {
            try
            {
                var updateQuery = "UPDATE Todo SET Description = @Description, Completed = @Completed WHERE Id = @Id;";
                var selectQuery = "SELECT * FROM Todo WHERE Id = @Id;";
                using (var conn = _dbConnectionFactory.Connect())
                {
                    await conn.ExecuteScalarAsync(updateQuery, new { todo.Description, todo.Completed, Id = id });
                    var result = await conn.QuerySingleAsync<Todo>(selectQuery, new { Id = id});
                    return result;
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException($"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }

        public async Task DeleteTodoAsync(int id)
        {
            try
            {
                var deleteQuery = "DELETE FROM Todo WHERE Id = @Id;";
                using (var conn = _dbConnectionFactory.Connect())
                {
                    await conn.ExecuteAsync(deleteQuery, new { Id = id });
                }
            }
            catch (SQLiteException ex)
            {
                throw new ApplicationException($"There was a problem with one of the database queries attempted.\nMessage: {ex.Message}");
            }
        }
    }
}
