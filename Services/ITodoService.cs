namespace CSharpTodoWithDapper.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetAllAsync();
        Task<List<Todo>> SearchAsync(string query);
    }
}
