namespace FoodPanel.Models.Dto;

public class RatingInDto
{
	public Guid PostId { get; set; }
	
	public double Stars { get; set; }

	public string Message { get; set; }
}