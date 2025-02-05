using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodPanel.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class RatingsController(ILogger<RatingsController> logger, DataContext context) : ControllerBase
{
    [HttpPost("createRating")]
    public async Task<IActionResult> CreateRating(
        [FromBody, Required] RatingInDto ratingInDto
    )
    {
        if (string.IsNullOrWhiteSpace(ratingInDto.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        var newRating = new Rating()
        {
            PostId = Guid.TryParse("cb45e25d-0140-4a1e-a110-6cb6fbffabb0", out Guid postId) ? postId : Guid.NewGuid(),
            CreatorId = Guid.TryParse("38954af2-c515-46ec-a501-cde342792d12", out Guid userGuid) ? userGuid : Guid.NewGuid(),
            Message = ratingInDto.Message
        };

        await context.Ratings.AddAsync(newRating);
        await context.SaveChangesAsync();

        return Created();
    }
    
    
    [HttpPost("deleteRating")]
    public async Task<IActionResult> DeleteRating(
        [FromBody, Required] RatingInDto ratingInDto
    )
    {
        if (string.IsNullOrWhiteSpace(ratingInDto.Message))
        {
            return BadRequest("Message cannot be empty.");
        }
        
        var RatingId = Guid.Parse("cb45e25d-0140-4a1e-a110-6cb6fbffabb0");
        var FinnBorchersId = Guid.Parse("38954af2-c515-46ec-a501-cde342792d12");

        await context.Ratings.Where(rating => rating.CreatorId == FinnBorchersId && rating.PostId == RatingId)
            .ExecuteDeleteAsync();
        await context.SaveChangesAsync();

        return Created();
    }
    
    [HttpGet("getRatingsByPostId")]
    public async Task<IActionResult> GetRatingsByPostId(
        [FromQuery, Required] Guid postId
    )
    {
        var posts = await context.Posts.FindAsync(postId);
        if (posts == null)
        {
            return NotFound("Post does not exist.");
        }

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