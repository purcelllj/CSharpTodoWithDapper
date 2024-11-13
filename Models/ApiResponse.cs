namespace CSharpTodoWithDapper.Data
{
    public class ApiResponse
    {
        public string Status { get; }
        public string Message { get; }
        public ApiResponse(string status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
