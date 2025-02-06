namespace FoodPanel.Models.Dto;

public class PostInDto
{
	public Guid CreatorId { get; set; }

	public string Title { get; set; }
	public string Message { get; set; }

	public IFormFile Image { get; set; }
}