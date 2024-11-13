using CSharpTodoWithDapper.Data;
using System.Text.RegularExpressions;

namespace CSharpTodoWithDapper.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            try
            {
                var todos = await _todoRepository.GetAllTodosAsync();
                return todos.OrderBy(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<Todo>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Mising or invalid query");
            }

            var todos = await _todoRepository.GetAllTodosAsync();

            var matched = todos
                .Where(x => Regex.IsMatch(x.Description, query, RegexOptions.IgnoreCase))
                .OrderBy(x => x.Id)
                .ToList();

            return matched;
        }

    }
}
