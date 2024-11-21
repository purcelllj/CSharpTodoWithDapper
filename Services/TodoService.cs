using CSharpTodoWithDapper.Data;
using CSharpTodoWithDapper.Models;
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
                return todo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<Todo>> SearchAsync(string query)
        {
            var todos = await _todoRepository.GetAllTodosAsync();

            var matched = todos
                .Where(x => Regex.IsMatch(x.Description, query, RegexOptions.IgnoreCase))
                .OrderBy(x => x.Id)
                .ToList();

            return matched;
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            var createdTodo = await _todoRepository.AddTodoAsync(todo);
            return createdTodo;
        }

        public async Task<Todo> UpdateAsync(Todo todo, int id)
        {
            try
            {
                var updatedTodo = await _todoRepository.UpdateTodoAsync(todo, id);
                return updatedTodo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _todoRepository.DeleteTodoAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
