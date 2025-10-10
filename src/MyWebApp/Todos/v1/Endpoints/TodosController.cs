using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Todos.v1.Dtos;

namespace MyWebApp.Todos.v1.Endpoints;

[ApiController]
[Route("api/1.0/todos")]
public class TodosController(TodoService todoService, LinkGenerator linkGenerator) : ControllerBase
{
    [HttpGet]
    public async Task<Ok<IEnumerable<TodoDto>>> GetAll()
    {
        var todos = await todoService.GetAllAsync();

        return TypedResults.Extensions.Ok(todos, todo => todo.ToDto());
    }


    [HttpGet("{id}", Name = "GetTodoAsync")]
    public async Task<Results<Ok<TodoDto>, NotFound>> GetTodoAsync([FromRoute] int id)
    {
        var todo = await todoService.GetAsync(id);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }

    [HttpPost]
    public async Task<Created<TodoDto>> CreateAsync([FromBody] CreateTodoDto dto)
    {
        var todo = await todoService.CreateAsync(dto);

        var todoLink = linkGenerator.GetUriByName(HttpContext, nameof(GetTodoAsync), new { id = todo.Id });

        return TypedResults.Created(todoLink, todo.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<Results<Ok<TodoDto>, NotFound>> PutAsync([FromRoute] int id, [FromBody] PutTodoDto dto)
    {
        var todo = await todoService.UpdateAsync(id, dto);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }


    [HttpPatch("{id}")]
    public async Task<Results<Ok<TodoDto>, NotFound>> PatchAsync([FromRoute] int id, [FromBody] PatchTodoDto dto)
    {
        var todo = await todoService.PatchAsync(id, dto);

        return TypedResults.Extensions.OkOrNotFound(todo, todo => todo.ToDto());
    }

    [HttpDelete]
    public async Task<NoContent> DeleteAsync([FromRoute] int id)
    {
        await todoService.DeleteAsync(id);

        return TypedResults.NoContent();
    }
}
