using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MongoDB.Driver;
using MyWebApp.MongoTodos.Entities;
using MyWebApp.MongoTodos.v1.Dtos;

namespace MyWebApp.MongoTodos;

public class MongoTodoService(HybridCache hybridCache, IMongoCollection<Todo> todosCollection)
{
    public async Task<IEnumerable<Todo>> GetAllAsync()
        => await todosCollection.Find(FilterDefinition<Todo>.Empty).ToListAsync();

    public async Task<Todo?> GetAsync(string id)
        => await hybridCache.GetOrCreateAsync(
            $"/todo/{id}",
            async cancel => await GetFromSourceAsync(id));

    public async Task<Todo?> GetFromSourceAsync(string id)
        => await todosCollection.Find(todo => todo.Id == id).FirstOrDefaultAsync();

    public async Task<Todo> CreateAsync(CreateTodoDto dto)
    {
        var todo = new Todo
        {
            Id = Guid.CreateVersion7().ToString(),
            Title = dto.Title,
            IsCompleted = dto.IsCompleted
        };

        await todosCollection.InsertOneAsync(todo);

        await hybridCache.SetAsync($"/todo/{todo.Id}", todo);

        return todo;
    }

    public async Task<Todo?> UpdateAsync(string id, PutTodoDto dto)
    {
        var result = await todosCollection.FindOneAndUpdateAsync(
            todo => todo.Id == id,
            Builders<Todo>
                .Update
                .Set(t => t.Title, dto.Title)
                .Set(t => t.IsCompleted, dto.IsCompleted));

        await hybridCache.RemoveAsync($"/todo/{id}");

        return result;
    }

    public async Task<Todo?> PatchAsync(string id, PatchTodoDto dto)
    {
        var updateFilter = Builders<Todo>.Update;
        var updateFilters = new List<UpdateDefinition<Todo>>();

        if (dto.Title is not null)
        {
            updateFilters.Add(updateFilter.Set(t => t.Title, dto.Title));
        }

        if (dto.IsCompleted is not null)
        {
            updateFilters.Add(updateFilter.Set(t => t.IsCompleted, dto.IsCompleted));
        }

        var result = await todosCollection.FindOneAndUpdateAsync(
            todo => todo.Id == id,
            updateFilter.Combine(updateFilters));

        await hybridCache.RemoveAsync($"/todo/{id}");

        return await GetFromSourceAsync(id);
    }

    public async Task<long> DeleteAsync(string id)
    {
        var result = await todosCollection.DeleteOneAsync(todo => todo.Id == id);

        await hybridCache.RemoveAsync($"/todo/{id}");

        return result.DeletedCount;
    }
}
