using System.ComponentModel.DataAnnotations;

namespace FoodPanel.Models;

public class User
{
	[Key] public Guid Id { get; set; }

	[MaxLength(255)] public string Name { get; set; }

	[MaxLength(32)] public string UserHandle { get; set; }

	public ICollection<Post> Posts { get; set; }
	public ICollection<Rating> Ratings { get; set; }
}