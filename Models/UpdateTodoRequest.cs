using System.ComponentModel.DataAnnotations;

namespace CSharpTodoWithDapper.Models;

public class UpdateTodoRequest
{
   [Required] 
   public string Description { get; set; } = null!;

   [Required]
   public bool Completed { get; set; }
}