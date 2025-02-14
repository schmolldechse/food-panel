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
		
		if (await context.Ratings.AnyAsync(post => post.PostId == ratingInDto.PostId && post.CreatorId == userId))
			return Conflict("You have already rated this post");

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
	public async Task<IActionResult> DeleteRating([Required] Guid ratingId)
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

		if (!await context.Ratings.AnyAsync(rating => rating.Id == ratingId))
			return NotFound("Rating does not exist");

		await context.Ratings.Where(rating => rating.CreatorId == userId && rating.Id == ratingId)
			.ExecuteDeleteAsync();
		await context.SaveChangesAsync();

		return Ok();
	}

	[HttpGet("getRatingsByPostId")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RatingOutDto[]))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetRatingsByPostId(
		[FromQuery] [Required] Guid postId
	)
	{
		var posts = await context.Posts.FindAsync(postId);
		if (posts == null) return NotFound("Post does not exist.");

		var ratings = await context.Ratings.Include(r => r.Creator)
			.Where(rating => rating.PostId == postId)
			.Select(rating => new RatingOutDto()
			{
				Id = rating.Id,
				CreatorId = rating.CreatorId,
				CreatorName = rating.Creator.Name,
				CreatorHandle = rating.Creator.UserHandle,
				Stars = rating.Stars,
				Message = rating.Message
			})
			.ToArrayAsync();
		return Ok(ratings);
	}

	[HttpGet("getAllRatings")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RatingOutDto[]))]
	public async Task<IActionResult> GetAllRatings()
	{
		var ratings = await context.Ratings.Include(r => r.Creator)
			.Select(rating => new RatingOutDto()
			{
				Id = rating.Id,
				CreatorId = rating.CreatorId,
				CreatorName = rating.Creator.Name,
				CreatorHandle = rating.Creator.UserHandle,
				Stars = rating.Stars,
				Message = rating.Message
			})
			.ToArrayAsync();
		return Ok(ratings);
	}
}