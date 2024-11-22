namespace CSharpTodoWithDapper.Data.Models;

public class TodoItem
{
    public int Id { get; init; }
    public string? Description { get; set; }
    public bool Completed { get; init; }
}