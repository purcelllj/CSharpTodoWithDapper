using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CSharpTodoWithDapper.Data;
using CSharpTodoWithDapper.Services;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    // finish moving items to the service layer and remove _todoRepository from the constructor
    private readonly ITodoRepository _todoRepository;
    private readonly ITodoService _todoService;
    public static List<Todo> Todos = new List<Todo>();
    public TodoController(ILogger<TodoController> logger, ITodoRepository todoRepository, ITodoService todoService)
    {
        _logger = logger;
        _todoRepository = todoRepository;
        _todoService = todoService;
    }

    [HttpGet(Name = "GetAllTodos")]
    public async Task<List<Todo>> GetAsync()
    {
        // TODO: Add error handling
        var todos =  await _todoService.GetAllAsync();
        return todos; 
    }

    [HttpGet("search", Name = "Find todos")]
    public async Task<IActionResult> GetMatching([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Mising or invalid query");
        }
        var todos = await _todoRepository.GetAllTodosAsync();
            
        var matched = todos
            .Where(x => Regex.IsMatch(x.Description, query, RegexOptions.IgnoreCase))
            .OrderBy(x => x.Id)
            .ToList();

        if (matched.Count == 0)
        {
            return NotFound();
        }

        return Ok(matched);
    }

    [HttpPost(Name = "Add todo item")]
    public async Task<IActionResult> PostAsync(Todo todo)
    {
        await _todoRepository.AddTodoAsync(todo); 
        _logger.LogInformation($"Added todo item: {todo.Description}");
        return Ok("Todo successfully added.");
    }
    
    // implement put and delete
}