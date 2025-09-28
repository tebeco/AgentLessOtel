using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.MongoTodos.v1.Dtos;

namespace MyWebApp.MongoTodos.v1.Endpoints;

[ApiController]
[Route("api/1.0/mongo-todos")]
public class MongoTodosController(MongoTodoService mongoTodoService) : ControllerBase
{
    [HttpGet]
    public async Task<Ok<IEnumerable<TodoDto>>> GetAll()
    {
        var todos = await mongoTodoService.GetAllAsync();

        return TypedResults.Extensions.Ok(todos, todo => todo.ToDto());
    }


    [HttpGet("{id}")]
    public async Task<Results<Ok<TodoDto>, NotFound>> GetAsync([FromRoute] string id)
    {
        var todo = await mongoTodoService.GetAsync(id);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }


    [HttpPost]
    public async Task<Ok<TodoDto>> CreateAsync([FromBody] CreateTodoDto dto)
    {
        var todo = await mongoTodoService.CreateAsync(dto);

        return TypedResults.Ok(todo.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<Results<Ok<TodoDto>, NotFound>> PutAsync([FromRoute] string id, [FromBody] PutTodoDto dto)
    {
        var todo = await mongoTodoService.UpdateAsync(id, dto);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }


    [HttpPatch("{id}")]
    public async Task<Results<Ok<TodoDto>, NotFound>> PatchAsync([FromRoute] string id, [FromBody] PatchTodoDto dto)
    {
        var todo = await mongoTodoService.PatchAsync(id, dto);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }

    [HttpDelete]
    public async Task<NoContent> DeleteAsync([FromRoute] string id)
    {
        await mongoTodoService.DeleteAsync(id);

        return TypedResults.NoContent();
    }
}
