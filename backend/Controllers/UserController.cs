using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(DataContext db) : ControllerBase
{

	[HttpGet("{userId:guid}")]
	public async Task<IActionResult> GetUser(Guid userId)
	{
		if (!await db.Users.AnyAsync(user => user.Id == userId))
		{
			return NotFound("User was not found");
		}

		var user = (await db.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId))!;
		
		return Ok(new UserOutDto()
		{
			Id = user.Id,
			Name = user.Name,
			UserHandle = user.UserHandle,
			PostCount = user.Posts.Count,
			RatingCount = user.Ratings.Count,
			AverageRating = user.Ratings.Select(r => r.Stars).Average()
		});
	}
	
}