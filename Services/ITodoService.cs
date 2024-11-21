using CSharpTodoWithDapper.Models;

namespace CSharpTodoWithDapper.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetAllAsync();
        Task<List<Todo>> SearchAsync(string query);
        Task<Todo> FindByIdAsync(int id);
        Task<Todo> CreateAsync(Todo todo);
        Task<Todo> UpdateAsync(Todo todo, int id);
    }
}
