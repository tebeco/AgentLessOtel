namespace MyWebApp.MongoTodos.Entities;

public class Todo
{
    public required string Id { get; set; }

    public required string Title { get; set; }

    public required bool IsCompleted { get; set; }
}
