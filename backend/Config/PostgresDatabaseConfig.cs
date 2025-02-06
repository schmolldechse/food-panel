namespace FoodPanel.Config;

public class PostgresDatabaseConfig
{
	public string Host { get; set; }
	public int Port { get; set; } = 5432;
	public string Database { get; set; }
	public string User { get; set; }
	public string Password { get; set; }
	
	public string ConnectionString => $"Host={Host};Port={Port};Username={User};Password={Password};Database={Database};";
}