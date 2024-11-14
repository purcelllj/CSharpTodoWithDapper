using CSharpTodoWithDapper.Data;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<Todo> FindByIdAsync(int id)
        {
            try
            {
                var todo = await _todoRepository.GetTodoByIdAsync(id);
                if (todo is null)
                {
                    throw new KeyNotFoundException($"Item with id '{id}' was not found");
                }
                return todo;
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
                throw new ArgumentNullException("Mising or invalid query");
            }

            var todos = await _todoRepository.GetAllTodosAsync();

            var matched = todos
                .Where(x => Regex.IsMatch(x.Description, query, RegexOptions.IgnoreCase))
                .OrderBy(x => x.Id)
                .ToList();

            return matched;
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            if(string.IsNullOrWhiteSpace(todo.Description))
            {
                throw new ArgumentNullException($"Missing required field {todo.Description}");
            }
            var createdTodo = await _todoRepository.AddTodoAsync(todo);
            return createdTodo; 
        }
    }
}
