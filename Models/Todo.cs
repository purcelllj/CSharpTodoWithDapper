using System.ComponentModel.DataAnnotations;

namespace CSharpTodoWithDapper.Models;

public class Todo
{
    public int Id { get; init; }
    public string? Description { get; set; }
    public bool Completed { get; init; }
}
