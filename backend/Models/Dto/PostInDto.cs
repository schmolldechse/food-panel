namespace FoodPanel.Models.Dto;

public class PostInDto
{
	public string Title { get; set; }
	public string Message { get; set; }

	public IFormFile Image { get; set; }
}