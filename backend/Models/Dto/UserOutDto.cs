namespace FoodPanel.Models.Dto;

public class UserOutDto
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string UserHandle { get; set; }

	public int PostCount { get; set; }
	public int RatingCount { get; set; }
	
	public double AverageRating { get; set; }
}