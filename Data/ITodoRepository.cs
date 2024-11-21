﻿using CSharpTodoWithDapper.Models;

namespace CSharpTodoWithDapper.Data
{
    public interface ITodoRepository
    {
        Task<Todo> AddTodoAsync(Todo todo);
        Task<Todo> GetTodoByIdAsync(int id);
        Task<List<Todo>> GetAllTodosAsync();
        Task<Todo> UpdateTodoAsync(Todo todo, int id);
        Task DeleteTodoAsync(int id);
    }
}
