using System.Security.Claims;
using FoodPanel;
using FoodPanel.Config;
using FoodPanel.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("APP_");

var config = new AppConfig();
builder.Configuration.Bind(config);
builder.Services.AddSingleton(config);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
	options.UseNpgsql(config.Database.ConnectionString));

builder.Services.AddMinio(configureClient => configureClient
	.WithEndpoint(config.Minio.Host, config.Minio.Port)
	.WithCredentials(config.Minio.AccessKey, config.Minio.SecretKey)
	.WithSSL(config.Minio.SSL)
	.Build());

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
	.AddCookie(IdentityConstants.ApplicationScheme, opt =>
	{
		opt.Cookie.Name = "Manager.Auth";

		opt.Cookie.IsEssential = true;
		opt.ExpireTimeSpan = TimeSpan.FromHours(10);

		opt.Cookie.Domain = null;

		opt.Events.OnRedirectToLogin = context =>
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return Task.CompletedTask;
		};
		opt.Events.OnRedirectToAccessDenied = context =>
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return Task.CompletedTask;
		};
	})
	.AddOpenIdConnect("bosch", "Bosch", opt =>
	{
		opt.MetadataAddress = config.OAuth.MetaDataAddress;
		opt.GetClaimsFromUserInfoEndpoint = true;
		opt.ClientId = config.OAuth.ClientId;
		opt.ClientSecret = config.OAuth.ClientSecret;
		opt.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
		opt.SignInScheme = IdentityConstants.ExternalScheme;
		opt.CallbackPath = "/api/v1/signin-oidc";
		foreach (var scope in config.OAuth.Scopes.Split(",").Select(x => x.Trim()))
			opt.Scope.Add(scope);
	}).AddCookie(IdentityConstants.ExternalScheme, opt => { opt.Cookie.Name = "Manager.External"; });

var ib = builder.Services.AddIdentityCore<User>(opt =>
	{
		opt.SignIn.RequireConfirmedAccount = false;
		opt.User.RequireUniqueEmail = true;
		opt.User.AllowedUserNameCharacters =
			"@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
		opt.Password.RequireDigit = false;
		opt.Password.RequiredLength = 6;
		opt.Password.RequireNonAlphanumeric = false;
		opt.Password.RequireUppercase = false;
		opt.Password.RequireLowercase = false;

		opt.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
	}).AddEntityFrameworkStores<DataContext>()
	.AddRoles<UserRole>()
	.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User, UserRole>>()
	.AddDefaultTokenProviders()
	.AddSignInManager();

builder.Services.AddScoped<IRoleStore<UserRole>, RoleStore<UserRole, DataContext, Guid>>();
builder.Services.AddScoped<IUserStore<User>, UserStore<User, UserRole, DataContext, Guid>>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("dev", policy =>
	{
		policy.WithOrigins("http://localhost:5173")
			.AllowCredentials()
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1",
		new OpenApiInfo
		{
			Title = "Foodpanel",
			Version = "v1",
			Description = "API Description for Project"
		});
	opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
	opt.MapType<DateOnly>(() => new OpenApiSchema
	{
		Type = "string",
		Format = "date"
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentName}/swagger.json");
	app.UseSwaggerUI(c => { c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Foodpanel v1"); });
}

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<DataContext>();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

	logger.LogInformation("Applying pending migrations...");
	context.Database.Migrate();
	logger.LogInformation("Applied pending migrations...");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("dev");

app.MapControllers();

app.Run();