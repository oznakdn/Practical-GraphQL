using GraphQL.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
    {
        
    }

    public DbSet<Project>Projects { get; set; }
    public DbSet<Member> Members { get; set; }

}
