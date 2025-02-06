using System.ComponentModel.DataAnnotations;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RatingsController(ILogger<RatingsController> logger, DataContext context) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> CreateRating([FromBody] [Required] RatingInDto ratingInDto)
	{
		if (!await context.Users.AnyAsync(user => user.Id == ratingInDto.UserId))
			return NotFound("You need to be logged in to create a rating");

		if (!await context.Posts.AnyAsync(post => post.Id == ratingInDto.PostId))
			return BadRequest("Post does not exist");

		if (string.IsNullOrWhiteSpace(ratingInDto.Message)) return BadRequest("Message cannot be empty");

		var newRating = new Rating
		{
			PostId = ratingInDto.PostId,
			CreatorId = ratingInDto.UserId,
			Message = ratingInDto.Message
		};

		await context.Ratings.AddAsync(newRating);
		await context.SaveChangesAsync();

		return Created();
	}


	[HttpDelete]
	public async Task<IActionResult> DeleteRating(
		[FromBody] [Required] Guid userId,
		[Required] Guid postId
	)
	{
		if (!await context.Users.AnyAsync(user => user.Id == userId))
			return NotFound("You need to be logged in to create a rating");

		if (!await context.Posts.AnyAsync(post => post.Id == postId))
			return BadRequest("Post does not exist");

		await context.Ratings.Where(rating => rating.CreatorId == userId && rating.PostId == postId)
			.ExecuteDeleteAsync();
		await context.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("getRatingsByPostId")]
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
	public async Task<IActionResult> GetAllRatings()
	{
		var ratings = await context.Ratings.ToArrayAsync();
		return Ok(ratings);
	}
}