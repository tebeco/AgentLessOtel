using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWebApp.Db.Models;

public class Todo : IEntityTypeConfiguration<Todo>
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required bool IsCompleted { get; set; }

    public void Configure(EntityTypeBuilder<Todo> builder)
        => builder.HasKey(builder => builder.Id);
}
