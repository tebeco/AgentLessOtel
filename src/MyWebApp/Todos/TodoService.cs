using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MyWebApp.Db;
using MyWebApp.Db.Models;
using MyWebApp.Todos.v1.Dtos;

namespace MyWebApp.Todos;

public class TodoService(HybridCache hybridCache, MyDbContext myDbContext)
{
    public async Task<IEnumerable<Todo>> GetAllAsync()
        => await myDbContext.Todos.ToListAsync();

    public async Task<Todo?> GetAsync(int id)
        => await hybridCache.GetOrCreateAsync(
            $"/todo/{id}",
            async cancel => await GetFromSourceAsync(id));

    public async Task<Todo?> GetFromSourceAsync(int id)
        => await myDbContext.Todos.FirstOrDefaultAsync(todo => todo.Id == id);

    public async Task<Todo> CreateAsync(CreateTodoDto dto)
    {
        var todo = new Todo
        {
            Title = dto.Title,
            IsCompleted = dto.IsCompleted
        };

        myDbContext.Todos.Add(todo);
        await myDbContext.SaveChangesAsync();

        await hybridCache.SetAsync($"/todo/{todo.Id}", todo);

        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, PutTodoDto dto)
    {
        var todo = await myDbContext
            .Todos
            .AsTracking()
            .FirstOrDefaultAsync(todo => todo.Id == id);

        if (todo is null)
        {
            return null;
        }

        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;

        await myDbContext.SaveChangesAsync();

        await hybridCache.RemoveAsync($"/todo/{id}");

        return todo;
    }

    public async Task<Todo?> PatchAsync(int id, PatchTodoDto dto)
    {
        var todo = await myDbContext
            .Todos
            .AsTracking()
            .FirstOrDefaultAsync(todo => todo.Id == id);

        if (todo is null)
        {
            return null;
        }

        if (dto.Title is not null)
        {
            todo.Title = dto.Title;
        }

        if (dto.IsCompleted is not null)
        {
            todo.IsCompleted = dto.IsCompleted.Value;
        }

        await myDbContext.SaveChangesAsync();

        await hybridCache.RemoveAsync($"/todo/{id}");

        return todo;
    }

    public async Task<int> DeleteAsync(int id)
    {
        var affectedRowCount = await myDbContext.Todos.Where(todo => todo.Id == id).ExecuteDeleteAsync();

        await hybridCache.RemoveAsync($"/todo/{id}");

        return affectedRowCount;
    }
}
