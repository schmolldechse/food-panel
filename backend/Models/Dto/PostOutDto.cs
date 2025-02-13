namespace FoodPanel.Models.Dto;

public class PostOutDto
{
	public Guid Id { get; set; }

	public Guid CreatorId { get; set; }
	public string CreatorName { get; set; }
	public string CreatorHandle { get; set; }
	
	public string Title { get; set; }
	public string Message { get; set; }

	public int CommentAmount { get; set; }
	public double AverageRating { get; set; }
}