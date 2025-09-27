using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWebApp.Db;

public class AgileDbContext : DbContext
{
    public required DbSet<Workitem> Workitems { get; set; }
}

public class Workitem : IEntityTypeConfiguration<Workitem>
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required User CreatedBy { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset LastModif{ get; set; }

    public required User? AssignedTo { get; set; }

    public WorkitemStatus Status { get; set; }

    public void Configure(EntityTypeBuilder<Workitem> builder)
    {
        
    }
}

public class User
{
    public int Id { get; set; }

    public required string Name { get; set; }
}


public enum WorkitemStatus
{
    New,
    Ready,
    InProgress,
    Done,
    Tested,
    Closed
}