namespace MyWebApp.Todos.v1.Dtos;

public record TodoDto(int Id, string Title, bool IsCompleted);

public record CreateTodoDto(string Title, bool IsCompleted);

public record PutTodoDto(string Title, bool IsCompleted);

public record PatchTodoDto(string? Title, bool? IsCompleted);

public static class TodoDtoExtensions
{
    public static TodoDto ToDto(this Db.Models.Todo todo)
        => new(todo.Id, todo.Title, todo.IsCompleted);
}
