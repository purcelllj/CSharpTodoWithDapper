using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Linq;

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

        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            try
            {
                var insertQuery = "INSERT INTO Todo(Description, Completed) VALUES (@Description, @Completed); SELECT last_insert_rowid();";
                var getTodoQuery = "SELECT * FROM Todo WHERE Id = @Id";
                using (IDbConnection conn = _dbConnectionFactory.Connect())
                {
                    var id = await conn.ExecuteScalarAsync<int>(insertQuery, todo);
                    var getResult = await conn.ExecuteScalarAsync<Todo>(getTodoQuery, new { Id = id});
                    if (getResult == null)
                    {
                        throw new Exception();
                    }
                    var createdTodo = new Todo { Id = getResult.Id, Description = getResult.Description, Completed = getResult.Completed };

                    return createdTodo;
                }
            }
            catch (SQLiteException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Todo> GetTodoByIdAsync(int id)
        {
            try
            {
                var comm = "SELECT * FROM Todo WHERE Id = @Id";
                using (IDbConnection conn = _dbConnectionFactory.Connect())
                {
                    var queryResult = await conn.QuerySingleAsync<Todo>(comm, new { Id = id });
                    return queryResult;
                }
            }
            catch (SQLiteException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception)
            {
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
            catch (SQLiteException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
