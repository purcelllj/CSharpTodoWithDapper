using CSharpTodoWithDapper.Data;

namespace CSharpTodoWithDapper.Services
{
    public class TodoService: ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            try
            {
                var todos = await _todoRepository.GetAllTodosAsync();
                return todos.OrderBy(x => x.Id).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("There was a problem retrieving todos.");
            }

        }

    }
}
