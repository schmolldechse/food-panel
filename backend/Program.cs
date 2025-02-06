using FoodPanel;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
	options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL"));
});

builder.Services.AddMinio(configureClient => configureClient
	.WithEndpoint("localhost", 9000)
	.WithCredentials("foodpanel", "foodpanel")
	.WithSSL(false)
	.Build());

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "*", policy => policy.AllowAnyOrigin());
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

app.UseCors("*");

app.MapControllers();

app.Run();