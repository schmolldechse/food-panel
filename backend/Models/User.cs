using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FoodPanel.Models;

public class User : IdentityUser<Guid>
{
	[MaxLength(255)] public string Name { get; set; }

	[MaxLength(32)] public string UserHandle { get; set; }

	public ICollection<Post> Posts { get; set; }
	public ICollection<Rating> Ratings { get; set; }
}