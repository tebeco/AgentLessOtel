namespace MyWebApp.MongoTodos.v1.Dtos;

public record TodoDto(string Id, string Title, bool IsCompleted);

public record CreateTodoDto(string Title, bool IsCompleted);

public record PutTodoDto(string Title, bool IsCompleted);

public record PatchTodoDto(string? Title, bool? IsCompleted);

public static class TodoDtoExtensions
{
    public static TodoDto ToDto(this Entities.Todo todo)
        => new(todo.Id, todo.Title, todo.IsCompleted);
}
