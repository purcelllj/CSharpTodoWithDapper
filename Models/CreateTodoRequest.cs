using System.ComponentModel.DataAnnotations;

namespace CSharpTodoWithDapper.Models;

public class CreateTodoRequest
{
    [Required]
    public string Description { get; set; } = null!;
}