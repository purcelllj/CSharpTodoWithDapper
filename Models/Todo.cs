namespace CSharpTodoWithDapper;

public class Todo
{
    public int Id { get; set; }
    public string Description { get; set; } = "";

    public bool Completed { get; set; } = false;
}
