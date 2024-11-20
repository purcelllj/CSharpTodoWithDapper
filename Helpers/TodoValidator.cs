using CSharpTodoWithDapper.Models;
using FluentValidation;
using FluentValidation.Results;

namespace CSharpTodoWithDapper.Helpers
{
    public class TodoValidator: AbstractValidator<Todo>
    {
        public TodoValidator()
        {
            RuleFor(todo => todo.Description).NotEmpty();
        }
        
        public static ValidationResult ValidateTodo(Todo todo)
        {
            var validator = new TodoValidator();
            var result = validator.Validate(todo);
            return result;
        }
    }
    
    
}