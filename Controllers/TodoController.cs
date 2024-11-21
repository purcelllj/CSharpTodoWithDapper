using CSharpTodoWithDapper.Helpers;
using Microsoft.AspNetCore.Mvc;
using CSharpTodoWithDapper.Services;
using CSharpTodoWithDapper.Models;
using static CSharpTodoWithDapper.Controllers.TodoRoutes;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    // finish moving items to the service layer and remove _todoRepository from the constructor
    private readonly ITodoService _todoService;
    private ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var todos = await _todoService.GetAllAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }

    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (id < 1)
        {
            return BadRequest(new ApiResponse("Bad Request", $"Invalid Id of {id}."));
        }
        try
        {
            var todo = await _todoService.FindByIdAsync(id);
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }

    }

    [HttpGet("find", Name = "Query")]
    public async Task<IActionResult> GetMatching([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Missing valid query.");
        }

        try
        {
            var matchedTodos = await _todoService.SearchAsync(query);

            if (matchedTodos.Count == 0)
            {
                return NotFound(new ApiResponse("Not Found", "The query returned 0 results."));
            }

            return Ok(matchedTodos);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost(Name = "CreateTodo")]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        var isValidCheck = TodoValidator.ValidateTodo(todo);
        if (!isValidCheck.IsValid)
        {
            return BadRequest(isValidCheck.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
        }
        var createdTodo = await _todoService.CreateAsync(todo);
        return CreatedAtAction(GetById, new { id = createdTodo.Id }, createdTodo);
    }

    [HttpPut("{id}", Name = "UpdateTodo")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Todo todo)
    {
        // Get existing by id
        var getByIdResult = await _todoService.FindByIdAsync(id);
        if (getByIdResult == null)
        {
            return NotFound($"No todo found by with the id of {id}");
        }

        var (existingDescription, existingCompletedStatus) = (
            getByIdResult.Description, getByIdResult.Completed);
   
        // Build update request 
        var updatedTodoRequest = new Todo
        {
            Description = 
                todo.Description != null && todo.Description != existingDescription ? 
                    todo.Description : existingDescription,
            Completed = 
                todo.Completed != existingCompletedStatus ? 
                    todo.Completed : existingCompletedStatus 
        };
        
        // Send the update
        try
        {
            var updatedTodo = await _todoService.UpdateAsync(updatedTodoRequest, id);
            return Ok(updatedTodo);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{id}", Name = "DeleteTodo")]
    public async Task<IActionResult> DeleteTodoAsync([FromRoute]int id)
    {
        if (id < 1)
        {
            return BadRequest(new ApiResponse("Bad Request", $"Invalid Id of {id}."));
        }
        try
        {
            await _todoService.DeleteAsync(id);
            return Ok(new ApiResponse("OK", $"Todo with id of {id} has been deleted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        } 
    }

}

public static class TodoRoutes
{
    public const string GetById = "GetById";
    public const string GetAll = "GetAll";
    public const string Query = "Query";
    public const string CreateTodo = "CreateTodo";
    public const string UpdateTodo = "UpdateTodo";
    public const string DeleteTodo = "DeleteTodo";
}