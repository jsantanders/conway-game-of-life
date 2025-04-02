using ConwayGameOfLife.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameOfLife.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Board> Boards => Set<Board>();
}