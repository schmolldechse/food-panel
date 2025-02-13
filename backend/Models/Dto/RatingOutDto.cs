namespace FoodPanel.Models.Dto;

public class RatingOutDto
{
    public Guid Id { get; set; }

    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; }
    public string CreatorHandle { get; set; }
	
    public string Message { get; set; }

    public double Stars { get; set; }
}