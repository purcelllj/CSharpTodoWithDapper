namespace CSharpTodoWithDapper.Data
{
    public interface ITodoRepository
    {
        Task AddTodoAsync(Todo todo);
        Task<List<Todo>> GetAllTodosAsync();
    }
}
