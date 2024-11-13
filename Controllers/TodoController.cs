using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CSharpTodoWithDapper.Data;
using CSharpTodoWithDapper.Services;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    // finish moving items to the service layer and remove _todoRepository from the constructor
    private readonly ITodoRepository _todoRepository;
    private readonly ITodoService _todoService;
    public static List<Todo> Todos = new List<Todo>();
    public TodoController(ITodoRepository todoRepository, ITodoService todoService)
    {
        _todoRepository = todoRepository;
        _todoService = todoService;
    }

    [HttpGet(Name = "GetAllTodos")]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var todos =  await _todoService.GetAllAsync();
            return Ok(todos);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
         
    }

    [HttpGet("search", Name = "Find todos")]
    public async Task<IActionResult> GetMatching([FromQuery] string query)
    {
        var matchedTodos = await _todoService.SearchAsync(query);
        
        if (matchedTodos.Count == 0)
        {
            return NotFound();
        }

        return Ok(matchedTodos);
    }

    [HttpPost(Name = "Add todo item")]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        await _todoRepository.AddTodoAsync(todo); 
        return Ok("Todo successfully added.");
    }
    
    // implement put and delete
}