using System.ComponentModel.DataAnnotations;

namespace FoodPanel.Models;

public class Post
{
	[Key] public Guid Id { get; set; }

	public Guid CreatorId { get; set; }
	public virtual User Creator { get; set; }

	[MaxLength(128)] public string Title { get; set; }

	public string Message { get; set; }
}