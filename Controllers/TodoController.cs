using Microsoft.AspNetCore.Mvc;
using CSharpTodoWithDapper.Data;
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
    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet(Name = "GetAll")]
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

    [HttpGet("{id}", Name = "GetById")]
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

    [HttpGet("find", Name = "Query")]
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

    [HttpPost(Name = "CreateTodo")]
    public async Task<IActionResult> PostAsync(Todo todo)
    { 
        var createdTodo = await _todoService.CreateAsync(todo);
        return CreatedAtAction(GetById, new { id = createdTodo.Id }, createdTodo);
    }

}

public static class TodoRoutes
{
    public const string GetById = "GetById";
    public const string GetAll = "GetAll";
    public const string Query = "Query";
    public const string CreateTodo = "CreateTodo";
}