using System.Security.Claims;
using FoodPanel.Models;
using FoodPanel.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace FoodPanel.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(
	SignInManager<User> signInManager,
	UserManager<User> userManager,
	ILogger<AuthController> logger,
	DataContext db)
	: ControllerBase
{
	[HttpGet("@{handle}")]
	[ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetUserInfo([FromRoute] string handle)
	{
		var user = await db.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.UserHandle == handle);

		if (user == null) return Unauthorized();

		return Ok(new UserOutDto
		{
			Id = user.Id,
			Name = user.Name,
			UserHandle = user.UserHandle,
			PostCount = user.Posts.Count,
			RatingCount = user.Ratings.Count,
			AverageRating = user.Ratings.Count != 0 ? user.Ratings.Select(r => r.Stars).Average() : 0
		});
	}
	
	[HttpGet("@me")]
	[Authorize]
	[ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UserOutDto), StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetCurrentUser()
	{
		if (!Guid.TryParse(userManager.GetUserId(User), out var userId))
		{
			return Unauthorized();
		}

		// Use Include to eagerly load Posts and Ratings
		var user = await db.Users
			.Include(u => u.Posts)
			.Include(u => u.Ratings)
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return Unauthorized();

		return Ok(new UserOutDto
		{
			Id = user.Id,
			Name = user.Name,
			UserHandle = user.UserHandle,
			PostCount = user.Posts.Count,
			RatingCount = user.Ratings.Count,
			AverageRating = user.Ratings.Count != 0 ? user.Ratings.Select(r => r.Stars).Average() : 0
		});
	}

	[HttpGet("signin/{providerName}")]
	public IActionResult Login(string providerName, string? returnUrl = null)
	{
		var redirectUrl = Url.Action("ExternalCallback", new { returnUrl });
		var properties = signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);
		return new ChallengeResult(providerName, properties);
	}

	[Authorize]
	[HttpGet("signout")]
	[ProducesResponseType(typeof(UserOutDto), StatusCodes.Status307TemporaryRedirect)]
	[ProducesResponseType(typeof(UserOutDto), StatusCodes.Status204NoContent)]
	public async Task<IActionResult> SignOut(string? returnUrl = null)
	{
		await signInManager.SignOutAsync();

		return returnUrl != null ? Redirect(returnUrl) : NoContent();
	}

	[AllowAnonymous]
	[HttpGet("external/callback")]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> ExternalCallback(string? returnUrl = null)
	{
		if (User.Identity is { IsAuthenticated: true }) await signInManager.SignOutAsync();

		ExternalLoginInfo? info;
		try
		{
			info = await signInManager.GetExternalLoginInfoAsync();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error getting external login info");
			return StatusCode(500);
		}

		if (info == null)
		{
			logger.LogInformation("Info is null");
			return BadRequest();
		}

		var providerKey = info.ProviderKey;
		if (info.LoginProvider == "bosch")
			providerKey = info.Principal.Claims.Single(x => x.Type == ClaimConstants.ObjectId).Value;

		var claims = info.Principal.Claims.ToList();

		var firstNameClaim = claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
		var lastNameClaim = claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
		var nameClaim = claims.SingleOrDefault(x => x.Type == ClaimConstants.Name)?.Value;

		if (string.IsNullOrEmpty(firstNameClaim) || string.IsNullOrEmpty(lastNameClaim) ||
		    string.IsNullOrEmpty(nameClaim)) return UnprocessableEntity();

		var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, providerKey, true, true);
		if (result.Succeeded)
		{
			var externalUser = await userManager.FindByLoginAsync(info.LoginProvider, providerKey);

			if (externalUser == null) return StatusCode(500);

			var accessToken = info.AuthenticationTokens?.SingleOrDefault(x => x.Name == "access_token")?.Value;
			if (accessToken != null)
			{
				await userManager.SetAuthenticationTokenAsync(externalUser, info.LoginProvider, "access_token",
					accessToken);
			}

			externalUser.Name = nameClaim;
			externalUser.UserHandle = $"{firstNameClaim}.{lastNameClaim}".ToLower();

			await userManager.UpdateAsync(externalUser);

			await signInManager.SignOutAsync();
			await signInManager.SignInAsync(externalUser, true, info.LoginProvider);

			// Success
			return returnUrl != null ? Redirect(returnUrl) : NoContent();
		}

		if (result.IsLockedOut) return BadRequest("Locked Out");

		// Check if we are already signed in (should never be the case)
		if (User.Identity is { IsAuthenticated: true })
		{
			return BadRequest();
		}

		var user = new User
		{
			Email = claims.Single(x => x.Type == ClaimTypes.Email).Value,
			UserName = claims.SingleOrDefault(x => x.Type == ClaimConstants.PreferredUserName)?.Value,
			UserHandle =
				$"{claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)!.Value}.{claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)!.Value}"
					.ToLower(),
			Name = nameClaim
		};

		var userCreateResult = await userManager.CreateAsync(user);
		if (!userCreateResult.Succeeded) return BadRequest();

		var addedLoginResult = await userManager.AddLoginAsync(user,
			new UserLoginInfo(info.LoginProvider, providerKey, info.ProviderDisplayName));
		if (!addedLoginResult.Succeeded) return BadRequest();

		await signInManager.SignInAsync(user, true, info.LoginProvider);
		return returnUrl != null ? Redirect(returnUrl) : NoContent();
	}
}