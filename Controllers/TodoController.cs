using CSharpTodoWithDapper.Data;
using CSharpTodoWithDapper.Data.Models;
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

    [HttpGet(Name = "GetTodos")]
    public async Task<IActionResult> GetAsync([FromQuery] string? query)
    {
        // if query param exists, look for matching todos
        if (!string.IsNullOrWhiteSpace(query))
        {
            try
            {
                var result = await _todoRepository.GetMatchingTodosAsync(query);
                if (result.Count == 0)
                {
                    return NotFound(new { message = "The query returned 0 results." });
                }

                var matchedTodos = result.Select(x => new Todo
                    { Id = x.Id, Description = x.Description, Completed = x.Completed }).ToList();
                return Ok(matchedTodos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem("An unexpected error occurred.");
            }
        }

        // get all todos
        try
        {
            var todos = await _todoRepository.GetAllTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem("An unexpected error occurred.");
        }
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (id < 1)
        {
            return BadRequest(new { error = $"Invalid Id of {id}." });
        }

        try
        {
            var todoQueryResult = await _todoRepository.GetTodoByIdAsync(id);
            if (todoQueryResult.Count < 1)
            {
                return NotFound(new { message = $"No todo found with the Id of {id}." });
            }

            var mappedTodos = todoQueryResult
                .Select(x => new Todo { Id = x.Id, Description = x.Description, Completed = x.Completed }).ToList();
            return Ok(mappedTodos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem("An unexpected error occurred.");
        }
    }

    [HttpPost(Name = "CreateTodo")]
    public async Task<IActionResult> PostAsync(CreateTodoRequest todo)
    {
        try
        {
            var result = await _todoRepository.AddTodoAsync(todo);
            var createdTodo = new Todo
                { Id = result.Id, Description = result.Description, Completed = result.Completed };
            return CreatedAtAction("GetById", new { id = createdTodo.Id }, createdTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem("An unexpected error occurred.");
        }
    }

    [HttpPut("{id}", Name = "UpdateTodo")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateTodoRequest todo)
    {
        // Send the update
        try
        {
            var result = await _todoRepository.UpdateTodoAsync(todo, id);
            var updatedTodo = new Todo { Id = id, Description = result.Description, Completed = result.Completed };
            return Ok(updatedTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("{id}", Name = "DeleteTodo")]
    public async Task<IActionResult> DeleteTodoAsync([FromRoute] int id)
    {
        if (id < 1)
        {
            return BadRequest(new { error = $"Invalid Id of {id}." });
        }

        try
        {
            await _todoRepository.DeleteTodoAsync(id);
            return Ok(new { message = $"Todo with id of {id} has been deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }
}