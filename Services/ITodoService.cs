namespace CSharpTodoWithDapper.Services
{
    public interface ITodoService
    {
        Task<List<Todo>> GetAllAsync();
    }
}
