namespace FoodPanel.Config;

public class MinioConfig
{
	public string Host { get; set; }
	public int Port { get; set; } = 9000;
	
	public string AccessKey { get; set; }
	public string SecretKey { get; set; }

	public bool SSL { get; set; } = false;
}