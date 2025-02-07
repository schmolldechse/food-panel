using System.ComponentModel.DataAnnotations;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostController(IMinioClient minio, DataContext db) : ControllerBase
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
				CommentAmount = post.Ratings.Count(),
				AverageRating = post.Ratings.Count != 0 ? post.Ratings.Average(r => r.Stars) : 0
			})
			.ToListAsync();
		
		return Ok(posts);
	}

	[HttpGet("{userId:guid}")]
	public async Task<IActionResult> GetPostByUser(Guid userId)
	{
		if (!await db.Users.AnyAsync(p => p.Id == userId)) return NotFound("User was not found");

		var posts = await db.Posts
			.Where(post => post.CreatorId == userId)
			.GroupJoin(
				db.Ratings.Where(r => r.CreatorId == userId),
				post => post.Id,
				rating => rating.PostId,
				(post, ratings) => new { Post = post, Ratings = ratings }
			)
			.Select(result => new PostOutDto
			{
				Id = result.Post.Id,
				CreatorId = result.Post.CreatorId,
				Title = result.Post.Title,
				Message = result.Post.Message,
				CommentAmount = result.Ratings.Count(),
				AverageRating = result.Ratings.Any() ? result.Ratings.Average(r => r.Stars) : 0
			})
			.ToArrayAsync();

		return Ok(posts);
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromForm, Required] PostInDto data)
	{
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
			CreatorId = data.CreatorId,
			Id = postId,
			Message = data.Message,
			Title = data.Title
		});

		await db.SaveChangesAsync();

		return Ok();
	}

	[HttpDelete]
	public async Task<IActionResult> Delete([FromForm] [Required] Guid postId)
	{
		if (!await db.Posts.AnyAsync(p => p.Id == postId)) return NotFound("Post was not found");

		await minio.RemoveObjectAsync(new RemoveObjectArgs()
			.WithBucket("foodpanel")
			.WithObject($"{postId}.png"));

		await db.Posts.Where(p => p.Id == postId).ExecuteDeleteAsync();
		await db.SaveChangesAsync();

		return Ok();
	}
}