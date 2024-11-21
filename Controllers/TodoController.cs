using CSharpTodoWithDapper.Data;
using Microsoft.AspNetCore.Mvc;
using CSharpTodoWithDapper.Models;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoRepository todoRepository, ILogger<TodoController> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var todos = await _todoRepository.GetAllTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }

    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (id < 1)
        {
            return BadRequest(new { error = $"Invalid Id of {id}."});
        }
        try
        {
            var todoQueryResult = await _todoRepository.GetTodoByIdAsync(id);
            if (todoQueryResult.Count < 1)
            {
                return NotFound(new { message = $"No todo found with the Id of {id}."});
            }
            return Ok(todoQueryResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }

    }

    [HttpGet("find", Name = "Query")]
    public async Task<IActionResult> GetMatching([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new { message = "Missing valid query." });
        }

        try
        {
            var matchedTodos = await _todoRepository.GetMatchingTodosAsync(query);

            if (matchedTodos.Count == 0)
            {
                return NotFound(new { message = "The query returned 0 results."});
            }

            return Ok(matchedTodos);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    [HttpPost(Name = "CreateTodo")]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        if (string.IsNullOrEmpty(todo.Description))
        {
            return BadRequest(new { messsage = "'Description' is a required field." });
        }
        try
        {
            var createdTodo = await _todoRepository.AddTodoAsync(todo);
            return CreatedAtAction("GetById", new { id = createdTodo.Id }, createdTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        } 
    }

    [HttpPut("{id}", Name = "UpdateTodo")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Todo? todo)
    {
        // Get existing by id
        var getByIdResult = await _todoRepository.GetTodoByIdAsync(id);
        if (getByIdResult.Count < 1)
        {
            return NotFound(new { message = $"No todo found with the Id of {id}."});
        }
        var todoToUpdate = getByIdResult.Single();
        
        var (existingDescription, existingCompletedStatus) = (
            todoToUpdate.Description, todoToUpdate.Completed);
         
        // Build update request 
        var description = !string.IsNullOrEmpty(todo.Description) ? todo.Description : existingDescription;
        var completed = !todo.Completed ? existingCompletedStatus : todo.Completed;
        var updatedTodoRequest = new Todo
        {
            Description = description,
            Completed = completed
        };
        
        // Send the update
        try
        {
            var updatedTodo = await _todoRepository.UpdateTodoAsync(updatedTodoRequest, id);
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
            return BadRequest(new { error = $"Invalid Id of {id}."});
        }
        try
        {
            await _todoRepository.DeleteTodoAsync(id);
            return Ok( new { message = $"Todo with id of {id} has been deleted successfully."});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        } 
    }

}