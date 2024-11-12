using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CSharpTodoWithDapper.Data;

namespace CSharpTodoWithDapper.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoRepository _todoRepository;
    public static List<Todo> Todos = new List<Todo>();
    public TodoController(ILogger<TodoController> logger, TodoRepository todoRepository)
    {
        _logger = logger;
        _todoRepository = todoRepository;
    }

    [HttpGet(Name = "GetAllTodos")]
    public async Task<List<Todo>> GetAsync()
    {
        var todos = await _todoRepository.GetAllTodosAsync();
        return todos.OrderBy(x => x.Id).ToList();
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
        return Ok("Todo successfully added.");
    }

}