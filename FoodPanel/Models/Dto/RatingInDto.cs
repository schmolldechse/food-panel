namespace FoodPanel.Models.Dto;

public class RatingInDto
{
    
    public Guid PostId { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Message { get; set; }
}