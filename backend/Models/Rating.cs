using System.ComponentModel.DataAnnotations;

namespace FoodPanel.Models;

public class Rating
{
	[Key] public Guid Id { get; set; }

	public Guid PostId { get; set; }
	public virtual Post Post { get; set; }

	public Guid CreatorId { get; set; }
	public virtual User Creator { get; set; }

	public double Stars { get; set; }
	public string Message { get; set; }
}