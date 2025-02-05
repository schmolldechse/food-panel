using System.ComponentModel.DataAnnotations;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FoodPanel.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class RatingsController(ILogger<RatingsController> logger, DataContext context) : ControllerBase
{
    [HttpPost(Name = "CreateRating")]
    public async Task<IActionResult> CreateRating(
        [FromBody, Required] RatingInDto ratingInDto
    )
    {
        if (string.IsNullOrWhiteSpace(ratingInDto.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = "Fonn Borchers",
            UserHandle = "finnistcool"
        };
        await context.Users.AddAsync(newUser);

        var newPost = new Post()
        {
            Id = Guid.NewGuid(),
            CreatorId = newUser.Id,
            Title = "Finn Borchers",
            Message = "Finn Borchers ist cool",
        };
        await context.Posts.AddAsync(newPost);

        var newRating = new Rating()
        {
            PostId = newPost.Id,
            CreatorId = newUser.Id,
            Message = ratingInDto.Message
        };

        await context.Ratings.AddAsync(newRating);
        await context.SaveChangesAsync();

        return Created();
    }
}