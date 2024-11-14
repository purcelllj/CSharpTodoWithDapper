using Microsoft.AspNetCore.Mvc;
using CSharpTodoWithDapper.Data;
using CSharpTodoWithDapper.Services;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    // finish moving items to the service layer and remove _todoRepository from the constructor
    private readonly ITodoService _todoService;
    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet(Name = "GetAllTodos")]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var todos = await _todoService.GetAllAsync();
            return Ok(todos);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }

    }

    [HttpGet("{id}", Name = "GetTodoById")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var todo = await _todoService.FindByIdAsync(id);
            return Ok(todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }

    }
    
    [HttpGet("search", Name = "Find todos")]
    public async Task<IActionResult> GetMatching([FromQuery] string query)
    {
        try
        {
            var matchedTodos = await _todoService.SearchAsync(query);

            if (matchedTodos.Count == 0)
            {
                return NotFound(new ApiResponse("Not Found", "The query returned 0 results."));
            }

            return Ok(matchedTodos);

        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost(Name = "Add todo item")]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        try
        {
            var createdTodo = await _todoService.CreateAsync(todo);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdTodo.Id }, createdTodo);
                //new ApiResponse("OK", $"Created todo with description '{todo.Description}'."));
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new ApiResponse("Bad Request", ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    // implement put and delete
}