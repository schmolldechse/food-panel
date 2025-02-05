using FoodPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}