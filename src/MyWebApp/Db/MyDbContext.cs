using Microsoft.EntityFrameworkCore;
using MyWebApp.Db.Models;

namespace MyWebApp.Db;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }
}
