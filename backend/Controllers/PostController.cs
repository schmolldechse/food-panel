using System.ComponentModel.DataAnnotations;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostController(IMinioClient minio, DataContext db, UserManager<User> userManager) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostOutDto[]))]
	public async Task<IActionResult> GetAllPosts()
	{
		var posts = await db.Posts
			.Include(p => p.Creator)
			.Include(p => p.Ratings) // if Ratings is a navigation property
			.Select(post => new PostOutDto
			{
				Id = post.Id,
				CreatorId = post.CreatorId,
				CreatorName = post.Creator.Name,
				Title = post.Title,
				Message = post.Message,
				CommentAmount = post.Ratings.Count,
				AverageRating = post.Ratings.Count != 0 ? post.Ratings.Average(r => r.Stars) : 0
			})
			.ToListAsync();

		return Ok(posts);
	}

	[HttpGet("{userId:guid}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostOutDto[]))]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPostByUser(Guid userId)
	{
		if (!await db.Users.AnyAsync(p => p.Id == userId)) return NotFound("User was not found");

		var posts = await db.Posts
			.Where(post => post.CreatorId == userId)
			.Include(p => p.Creator)
			.Include(p => p.Ratings) // if Ratings is a navigation property
			.Select(post => new PostOutDto
			{
				Id = post.Id,
				CreatorId = post.CreatorId,
				CreatorName = post.Creator.Name,
				Title = post.Title,
				Message = post.Message,
				CommentAmount = post.Ratings.Count,
				AverageRating = post.Ratings.Count != 0 ? post.Ratings.Average(r => r.Stars) : 0
			})
			.ToListAsync();

		return Ok(posts);
	}

	[HttpPost]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Post([FromForm, Required] PostInDto data)
	{
		if (!Guid.TryParse(userManager.GetUserId(this.User), out var userId))
		{
			return Unauthorized();
		}

		// Use Include to eagerly load Posts and Ratings
		var user = await db.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return Unauthorized();

		var postId = Guid.NewGuid();

		using var outputStream = new MemoryStream();

		// Convert image to png
		using (var imageFactory = new ImageFactory())
		{
			imageFactory.Load(data.Image.OpenReadStream())
				.Format(new PngFormat())
				.Save(outputStream);
		}

		await minio.PutObjectAsync(new PutObjectArgs()
			.WithBucket("foodpanel")
			.WithObject($"{postId}.png")
			.WithObjectSize(outputStream.Length)
			.WithStreamData(outputStream));

		await db.Posts.AddAsync(new Post
		{
			CreatorId = user.Id,
			Id = postId,
			Message = data.Message,
			Title = data.Title
		});

		await db.SaveChangesAsync();

		return Created();
	}

	[HttpDelete]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete([FromForm] [Required] Guid postId)
	{
		if (!Guid.TryParse(userManager.GetUserId(this.User), out var userId))
		{
			return Unauthorized();
		}

		// Use Include to eagerly load Posts and Ratings
		var user = await db.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return Unauthorized();

		if (!await db.Posts.AnyAsync(p => p.Id == postId))
			return NotFound("Post was not found");

		if ((await db.Posts.SingleAsync(p => p.Id == user.Id)).CreatorId == user.Id)
			return Unauthorized("You are not authorized to delete this post");
		
		await minio.RemoveObjectAsync(new RemoveObjectArgs()
			.WithBucket("foodpanel")
			.WithObject($"{postId}.png"));

		await db.Posts.Where(p => p.Id == postId).ExecuteDeleteAsync();
		await db.SaveChangesAsync();

		return Ok();
	}
}