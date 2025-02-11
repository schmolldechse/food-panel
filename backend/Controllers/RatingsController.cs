using System.ComponentModel.DataAnnotations;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RatingsController(ILogger<RatingsController> logger, DataContext context, UserManager<User> userManager)
	: ControllerBase
{
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> CreateRating([FromBody] [Required] RatingInDto ratingInDto)
	{
		if (!Guid.TryParse(userManager.GetUserId(this.User), out var userId))
		{
			return Unauthorized();
		}

		// Use Include to eagerly load Posts and Ratings
		var user = await context.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return Unauthorized();

		if (!await context.Posts.AnyAsync(post => post.Id == ratingInDto.PostId))
			return BadRequest("Post does not exist");

		if (ratingInDto.Stars is > 5 or < .5) return BadRequest("Stars must be between 0.5 and 5");
		if (ratingInDto.Stars % .5 != 0) return BadRequest("Stars must be between 0 and 5");

		if (string.IsNullOrWhiteSpace(ratingInDto.Message)) return BadRequest("Message cannot be empty");

		var newRating = new Rating
		{
			Id = Guid.NewGuid(),
			PostId = ratingInDto.PostId,
			CreatorId = user.Id,
			Stars = ratingInDto.Stars,
			Message = ratingInDto.Message
		};

		await context.Ratings.AddAsync(newRating);
		await context.SaveChangesAsync();

		return Created();
	}

	[HttpDelete]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> DeleteRating([Required] Guid postId)
	{
		if (!Guid.TryParse(userManager.GetUserId(this.User), out var userId))
		{
			return Unauthorized();
		}

		// Use Include to eagerly load Posts and Ratings
		var user = await context.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return Unauthorized();

		if (!await context.Posts.AnyAsync(post => post.Id == postId))
			return NotFound("Post does not exist");

		await context.Ratings.Where(rating => rating.CreatorId == userId && rating.PostId == postId)
			.ExecuteDeleteAsync();
		await context.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("getRatingsByPostId")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Rating[]))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetRatingsByPostId(
		[FromQuery] [Required] Guid postId
	)
	{
		var posts = await context.Posts.FindAsync(postId);
		if (posts == null) return NotFound("Post does not exist.");

		var ratings = await context.Ratings.Where(rating => rating.PostId == postId).ToArrayAsync();
		return Ok(ratings);
	}

	[HttpGet("getAllRatings")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Rating[]))]
	public async Task<IActionResult> GetAllRatings()
	{
		var ratings = await context.Ratings.ToArrayAsync();
		return Ok(ratings);
	}
}