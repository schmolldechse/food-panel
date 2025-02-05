using Microsoft.EntityFrameworkCore;

namespace FoodPanel.Models;

[PrimaryKey(nameof(PostId), nameof(CreatorId))]
public class Rating
{
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
    
    public Guid CreatorId { get; set; }
    public virtual User Creator { get; set; }
    
    public string Message { get; set; }
}