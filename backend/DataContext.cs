using FoodPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel;

public class DataContext(DbContextOptions options) : IdentityDbContext<User, UserRole, Guid>(options)
{
	public DbSet<Post> Posts { get; set; }
	public DbSet<Rating> Ratings { get; set; }
}