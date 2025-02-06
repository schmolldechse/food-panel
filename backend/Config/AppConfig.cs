namespace FoodPanel.Config;

public class AppConfig
{
	public PostgresDatabaseConfig Database { get; set; }
	public MinioConfig Minio { get; set; }
	
	public OAuthConfig OAuth { get; set; }
}
