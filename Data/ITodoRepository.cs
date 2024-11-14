namespace CSharpTodoWithDapper.Data
{
    public interface ITodoRepository
    {
        Task<Todo> AddTodoAsync(Todo todo);
        Task<Todo> GetTodoByIdAsync(int id);
        Task<List<Todo>> GetAllTodosAsync();
    }
}
